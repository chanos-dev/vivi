// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff2?$/, /\.png$/, /\.svg$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.webmanifest$/, /\.mp3$/, /\.m4a$/, /\.ogg$/, /\.wav$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// 서비스 워커의 scope(= 배포 경로, 예: /vivi/)를 기준으로 자산 URL을 계산한다.
// 로컬(/)과 GitHub Pages(/vivi/) 모두 별도 설정 없이 동작한다.
const baseUrl = new URL(self.registration.scope);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

// Google Fonts(Fredoka, Jua, 고운돋움)는 처음 받을 때 캐시해 두고 오프라인에서도 쓴다.
const fontCacheName = 'google-fonts';
const fontHosts = ['fonts.googleapis.com', 'fonts.gstatic.com'];

async function onFetch(event) {
    if (event.request.method === 'GET' && fontHosts.includes(new URL(event.request.url).hostname)) {
        const fontCache = await caches.open(fontCacheName);
        const cachedFont = await fontCache.match(event.request);
        if (cachedFont) {
            return cachedFont;
        }
        const response = await fetch(event.request);
        if (response.ok || response.type === 'opaque') {
            await fontCache.put(event.request, response.clone());
        }
        return response;
    }

    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache,
        // unless that request is for an offline resource.
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !manifestUrlList.some(url => url === event.request.url);

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    return cachedResponse || fetch(event.request);
}
