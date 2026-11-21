// 가로 스크롤 상태(data-left / data-right)를 표시해 화살표를 보이게 하고, 선택 칩을 화면 안으로 스크롤한다.
export function init(root) {
    const scroller = root.querySelector('.scroller');

    const update = () => {
        const max = scroller.scrollWidth - scroller.clientWidth;
        root.dataset.left = scroller.scrollLeft > 2 ? 'true' : 'false';
        root.dataset.right = scroller.scrollLeft < max - 2 ? 'true' : 'false';
    };

    scroller.addEventListener('scroll', update, { passive: true });
    const observer = new ResizeObserver(update);
    observer.observe(scroller);
    update();

    return {
        page(direction) {
            scroller.scrollBy({ left: direction * scroller.clientWidth * 0.7, behavior: 'smooth' });
        },
        reveal(smooth) {
            const chip = scroller.querySelector('[aria-checked="true"]');
            if (!chip) return;
            const left = chip.offsetLeft - (scroller.clientWidth - chip.offsetWidth) / 2;
            scroller.scrollTo({ left, behavior: smooth ? 'smooth' : 'auto' });
            update();
        },
        dispose() {
            scroller.removeEventListener('scroll', update);
            observer.disconnect();
        },
    };
}
