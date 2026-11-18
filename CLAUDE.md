# Vivi

바이올린 연습용 Blazor WebAssembly PWA. 연습실에서 모바일 기기로 설치해서 쓰는 것을 기본 사용 환경으로 본다.
로드맵은 `docs/roadmap.md`, 결정 사항은 `docs/decisions.md`를 참고한다.

## 명령어

```bash
dotnet build Vivi.slnx                     # 전체 빌드 (경고도 에러로 처리)
dotnet test Vivi.slnx                      # 단위 테스트
dotnet watch --project src/Vivi            # 로컬 개발 서버 (핫 리로드)
dotnet publish src/Vivi -c Release -o out  # 배포 산출물 (out/wwwroot)
```

## 구조

```
src/Vivi/            Blazor WASM 앱 (UI, JS interop, 브라우저 API)
  Components/        재사용 컴포넌트
  Layout/            MainLayout, 하단 탭바(NavMenu)
  Pages/             라우트 페이지 (@page)
  Services/          앱 서비스 (설정, 저장소, 오디오 등)
  wwwroot/           정적 자산, PWA manifest, 서비스 워커
  Routes.cs          링크용 라우트 상수
src/Vivi.Core/       순수 C# 도메인 (음, 주파수, 포지션, 판정 로직). 브라우저 의존성 금지
tests/Vivi.Core.Tests/  Vivi.Core 단위 테스트 (xUnit)
```

## 컨벤션

- MS Blazor 템플릿 구조를 따른다: 페이지는 `Pages/`, 레이아웃은 `Layout/`, 공용 컴포넌트는 `Components/`.
- 링크 `href`에는 `Routes` 상수를 쓴다. 상대 경로여야 GitHub Pages의 `/vivi/` 하위 경로에서 동작한다. 앞에 `/`를 붙이지 않는다.
- 의존성 주입은 `@inject`를 쓴다. 페이지 클래스 이름과 겹치지 않게 서비스 이름 그대로 주입한다(예: `@inject SettingsService SettingsService`).
- 테스트 가능한 로직(음 계산, 판정, 퀴즈 생성)은 `Vivi.Core`에 둔다. 앱 프로젝트는 UI와 interop만 담당한다.
- 패키지 버전은 `Directory.Packages.props`(Central Package Management)에서만 관리한다.
- JS interop:
  - 컴포넌트 전용 JS는 collocated 모듈(`Foo.razor.js`)로 둔다.
  - 고빈도 오디오 처리(음 높이 검출, 메트로놈 스케줄링)는 JS의 Web Audio/AudioWorklet에서 처리한다. C#에는 결과값만 넘긴다.

## 디자인 / 스타일 컨벤션

브랜드 = 시안 5 **Doremi**: 경쾌함, 게임처럼 즐기는 연습, 성취감 (2026-11-18 확정. 이전의 Rosin+Etude 결정을 대체한다).
시안과 디자인 시스템 보드: https://claude.ai/artifact/7ZAQydgzUCMkVykC4KLzaF

- **CSS:** 직접 작성한 CSS만 쓰고 UI 라이브러리는 쓰지 않는다.
  - 전역 디자인 토큰과 공용 클래스(`.btn`, `.btn-primary`, `.chip`, `.num`, `.tone-g/d/a/e`)는 `wwwroot/css/app.css`에 둔다.
  - 컴포넌트 전용 스타일은 CSS isolation(`Foo.razor.css`)으로 쓴다. 자식 컴포넌트를 꾸밀 때는 `::deep`.
- **토큰만 사용:** 색상, 간격, 모서리, 테두리, 터치 영역에 hex나 px 값을 직접 쓰지 않는다. `--color-*`, `--string-*`, `--space-*`, `--radius-*`, `--border-width`, `--touch-target*` 토큰을 쓴다. 새 색은 `:root` 토큰으로 추가한다.
- **색상:**

  | 토큰 | 라이트 | 용도 |
  |---|---|---|
  | `--color-bg` / `--color-surface` | `#FFF8F0` / `#FFFFFF` | 페이지 배경 / 카드·탭바 |
  | `--color-text` | Ink `#2A2540` | 본문 |
  | `--color-accent` | Violet `#5B3FE0` | 링크, 활성 탭, 선택 상태 |
  | `--color-primary` | Ink `#2A2540` | 주요 버튼(`.btn-primary`, 아래 그림자로 눌리는 느낌) |
  | `--color-inverse-*` | Ink 바탕 | 연속 연습일, 오늘의 연습 같은 강조 카드 |
  | `--color-star` | `#FFD166` | 별·성취 장식 전용 |
  | `--color-success` / `--color-danger` | | 정확·통과 / 벗어남·실패. 색만으로 구분하지 않고 텍스트·아이콘을 함께 쓴다 |

- **현 색상(핵심 규칙):** G·D·A·E 현마다 고유 색이 있다. 지판 지도, 운지 퀴즈, 튜너의 현 선택 등 **현을 나타낼 때는 항상 이 색을 쓴다.**

  | 현 | 면(`--string-x`) | 면 위 텍스트(`-on`) | 흰 바탕 위 텍스트(`-text`) |
  |---|---|---|---|
  | G | `#8B6CFF` 보라 | `#1E1640` | `#3F27B5` |
  | D | `#2BB5A0` 청록 | `#0D2E29` | `#0D5C50` |
  | A | `#FF9A3C` 주황 | `#3A1D04` | `#9A4A0C` |
  | E | `#FF6B9A` 분홍 | `#3D0B1C` | `#A3214D` |

  - 현 색 면 위의 글자는 흰색이 아니라 `-on`(어두운 색)을 쓴다. 대비 때문이다.
  - 면 자체를 텍스트 색으로 쓰지 않는다. 텍스트에는 `-text`를 쓴다.
  - 홈의 기능 카드(`FeatureCard Tone="g"` 등)도 이 톤을 빌려 쓴다.
- **서체:**
  - `--font-heading` = Jua: 제목, 버튼, 카드 제목. 단일 굵기라 `font-weight`를 지정하지 않는다.
  - `--font-body` = 고운돋움: 본문과 설명.
  - `--font-latin` = Fredoka: 워드마크, 음 이름(A4), Hz, 센트, 손가락 번호, 레벨, 통계 숫자. `.num` 클래스를 쓴다.
  - 폰트는 Google Fonts로 불러오고, 서비스 워커가 런타임에 캐시한다.
- **워드마크:** `<Wordmark />` 컴포넌트를 쓴다. Fredoka Bold 소문자 `vivi`이고, 글자마다 G·D·A·E 색을 순서대로 입힌다. 텍스트로 쓸 때도 항상 소문자 `vivi`로 쓴다.
- **앱 아이콘:** Cream 바탕에 G현 보라 둥근 `v`(Fredoka Bold, 패스로 변환)와 A현 주황 마침표. 원본, 대안, 생성 방법은 `docs/brand/`에 있다.
- **형태:**
  - 크게 둥근 모서리: `--radius-md` 18px, `--radius-lg` 24px, 버튼과 칩은 `--radius-pill`.
  - 테두리는 2px(`--border-width`).
  - 주요 버튼은 아래쪽 그림자가 있고, 누르면 그만큼 내려간다.
  - 그라데이션과 이모지는 쓰지 않는다. 별·체크 같은 아이콘은 인라인 SVG(선 굵기 2.4, 둥근 끝)로 그린다.
- **게임 요소:** 레벨, 별, 연속 연습일 같은 성취 표현을 적극적으로 쓴다. 다만 숫자는 과장 없이 실제 기록만 보여준다.
- **테마:** Cream 라이트 단일 테마다. 기기가 다크 모드여도 따라가지 않는다(`color-scheme: light`). `prefers-color-scheme: dark` 스타일을 추가하지 않는다.
- **사용성:** 터치 영역은 최소 44px, 주요 동작은 56px. 텍스트 대비는 4.5:1 이상.

## PWA / 브라우저 주의점

- AudioContext와 마이크는 반드시 사용자 입력(탭·클릭) 이벤트 안에서 시작한다(브라우저 자동재생 정책).
- 마이크는 HTTPS(또는 localhost)에서만 동작한다. 실제 기기 테스트는 GitHub Pages 배포본으로 한다.
- 서비스 워커가 자산을 캐시하므로 배포 후 앱을 완전히 닫았다 열어야 새 버전이 반영된다.
- `index.html`의 base 경로는 런타임 스크립트로 설정한다. 빌드 후 파일을 수정하면 서비스 워커의 무결성 해시가 깨진다.
