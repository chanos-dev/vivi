# 브랜드 에셋 (Doremi)

## 앱 아이콘

"v. 모노그램"에 Doremi 팔레트를 입힌 Cream 버전이다(2026-11-18 확정).

- **바탕:** Cream `#FFF8F0`
- **글자 `v`:** G현 보라 `#8B6CFF`. Fredoka Bold(700)의 글리프를 패스로 변환해서 폰트 없이 렌더링된다.
- **마침표:** A현 주황 `#FF9A3C`

| 파일 | 용도 |
|---|---|
| `app-icon.svg` | 둥근 모서리 버전. 원본이며 `wwwroot/favicon.svg`로 쓴다 |
| `app-icon-fullbleed.svg` | 모서리 없는 정사각형. PNG 생성용(OS가 직접 둥근 모양으로 자름) |
| `app-icon-1024.png` | 1024px 마스터 PNG |
| `app-icon-alt-ink.svg`, `app-icon-alt-violet.svg` | 대안 시안(Ink 바탕, Violet 바탕) |

앱에 들어가는 파일은 `wwwroot/`의 `icon-512.png`, `icon-192.png`, `apple-touch-icon.png`(180px, 홈 화면 아이콘용), `favicon.svg`이다.
`v`와 마침표는 maskable 안전 영역(중앙 80% 원) 안에 들어간다.

Fredoka는 SIL Open Font License 1.1 서체이며, 아이콘에는 글리프 윤곽만 그림으로 들어 있다.
