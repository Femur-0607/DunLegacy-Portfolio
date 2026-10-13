# DunLegacy - 던전 생성부터 보스전까지 게임 루프를 처음 완성한 2D 로그라이트

![Unity](https://img.shields.io/badge/Unity-222222?style=flat-square&logo=unity&logoColor=white) ![C#](https://img.shields.io/badge/C%23-68217A?style=flat-square&logo=csharp&logoColor=white) ![Solo](https://img.shields.io/badge/1인-개발-555555?style=flat-square) [![YouTube](https://img.shields.io/badge/YouTube-플레이_영상-FF0000?style=flat-square&logo=youtube&logoColor=white)](https://youtu.be/-EYzNawFw4I)

> 본 저장소는 포트폴리오 공개용 **코드 열람 저장소**입니다. 원본 프로젝트의 외부 에셋, 사운드, 스프라이트, 씬 파일은 저작권과 용량 문제로 제외했으므로 Unity에서 그대로 실행되지 않습니다. 실제 플레이는 아래 영상으로 확인해 주세요.

[![DunLegacy 플레이 영상](https://img.youtube.com/vi/-EYzNawFw4I/maxresdefault.jpg)](https://youtu.be/-EYzNawFw4I)

▶︎ [플레이 영상](https://youtu.be/-EYzNawFw4I) · 📄 [기술문서 PDF](Docs/Unity_기술문서_던레거시_양지섭.pdf) · 👤 [GitHub 프로필](https://github.com/Femur-0607)

## 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 장르 | 2D 횡스크롤 액션 로그라이트 |
| 개발 기간 | 2024.12 – 2025.02 (7주) |
| 개발 형태 | 1인 프로젝트 (첫 Unity 완성작) |
| 개발 환경 | Unity 2022.3.21f1, C# |
| 플랫폼 | PC |
| 주요 기술 | 그리드 랜덤 방 생성, 몬스터 FSM, 보스 패턴 코루틴, ScriptableObject, Object Pooling, Cinemachine |
| 상태 | 완료 (플레이 가능 빌드 기준, 영상 참고) |

마을에서 던전에 들어가 랜덤하게 이어진 방을 탐험하고 몬스터와 보스를 상대하는 게임입니다. 이동·전투·UI·대화·사운드·씬 전환이 하나의 루프로 돌아가도록 만든 첫 프로젝트이며, 이후 KrameLife와 MiniDriller에서 매니저 의존을 줄이는 기준이 됐습니다.

## 담당 범위

1인 프로젝트로 기획·구현·테스트를 모두 진행했습니다.

- `RoomManager`의 Queue 기반 랜덤 방 생성과 미니맵, 문 연결
- `Monster` 상태머신(Idle / Tracking / Attacking / Die)과 원거리·근접·돌진 파생
- `Boss`의 레이저·검·탄막 패턴 코루틴
- 플레이어 이동·점프·단방향 플랫폼, 장비·체력, 업그레이드
- Player / Monster / Weapon 데이터 ScriptableObject
- 데미지 텍스트·탄환·사망 이펙트 오브젝트 풀, UI·대화·사운드 매니저
- 외부 에셋: 스프라이트·타일맵·사운드(공개본에서 제외)

## 핵심 구현

### 1. Queue로 프레임마다 확장 후보 하나를 처리하는 랜덤 던전

**문제.** 방을 한 번에 전부 배치하면 겹침·고립을 사후에 걸러내야 하고, 최소 방 수를 못 채우는 경우 처리가 복잡해집니다.

**선택.** `RoomManager`가 시작 방과 두 번째 방 좌표를 `Queue<Vector2Int>`에 넣고, `Update`마다 후보 하나를 꺼내 상하좌우 네 방향에 방 생성을 시도했습니다. 생성한 방은 다시 큐에 넣고 이웃 방과 양쪽 문을 연결했습니다. 새 방으로 최대 방 수에 도달할 때는 생성 시 종료 방 프리팹을 선택하고, 최소 방 수를 못 채우면 전체를 지우고 다시 생성했습니다. 완료 후 같은 좌표로 미니맵을 만들고 `Dictionary<Vector2Int, GameObject>`에 저장했습니다.

**결과.** 기존 방의 이웃에 새 방을 생성하고 양쪽 문을 열어 논리적인 연결을 구성했습니다. 방과 미니맵에 같은 그리드 좌표를 사용했습니다.

- [RoomManager.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Manager/RoomManager.cs) · [Room.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/RandomMap/Room.cs) · [MiniMaps.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/RandomMap/MiniMaps.cs)

### 2. enum 상태머신과 파생 클래스별 동작으로 나눈 몬스터

**문제.** 원거리·근접·돌진 몬스터가 추적·공격·사망 처리를 각자 다르게 구현하면 같은 버그를 세 번 고치게 됩니다.

**선택.** `Monster`가 `FixedUpdate`에서 감지 범위를 확인하고, `MonsterState` enum과 `UpdateState`의 `switch`로 공통 상태 흐름을 처리했습니다. 공격 전후 대기는 `AttackAfterDelay`, 체력·기본 비활성화는 `HealthManager`에서 공유했습니다. 파생 클래스는 `ExecuteAttackPattern`뿐 아니라 필요한 추적·사망 연출도 확장하고, `RushingEnemy`는 별도 `Update`에서 돌진 거리·피해를 처리했습니다.

**결과.** 공통 FSM과 공격 대기를 재사용하면서 종류별 이동·공격·사망 처리를 파생 클래스에 나눴습니다.

- [Monster.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Monster/Monster.cs) · [RushingEnemy.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Monster/RushingEnemy.cs) · [HealthManager.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Manager/HealthManager.cs)

### 3. 코루틴으로 순서를 제어한 보스 패턴

**문제.** 레이저·다중 레이저·검 발사·나선형 탄막처럼 시간차가 있는 패턴을 `Update`의 타이머로 쓰면 분기와 플래그가 빠르게 늘어납니다.

**선택.** `PatternCoroutine`에서 2초 대기 후 패턴을 고르고, `Boss.Attack`의 `switch`로 실행했습니다. 레이저·검·탄막의 대기 동작은 코루틴으로 나눴으며, 검은 생성·발사·회수를 별도 코루틴으로 처리했습니다. 검 발사 후 다음 패턴 선택을 예약해 회수 완료를 기다리지는 않습니다. 탄막은 `index % 4`로 네 방향을 고르고, `bulletCount`에 따른 각도로 회전시켰습니다.

**결과.** 패턴 선택과 시간차가 있는 공격 동작을 코루틴으로 나눠 표현했습니다.

- [Boss.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Boss/Boss.cs) · [BossLaser.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Boss/BossLaser.cs) · [BulletPool.cs](https://github.com/Femur-0607/DunLegacy-Portfolio/blob/c95ad8052be422dd47f82a6242b4aaf8a05b880f/Assets/03.Script/Boss/BulletPool.cs)

## 트러블슈팅 — 절차적 맵 생성을 최소 단위로 쪼개서 검증

처음 랜덤 던전을 만들 때 생성·연결·미니맵을 한 번에 구현하니 어느 단계에서 방이 고립되는지 알 수 없었습니다. 생성 과정을 **좌표 한 칸 생성 → 인접 방 수 확인 → 문 연결 → 미니맵 반영**으로 나누고, 각 단계를 Gizmo와 로그로 확인하며 진행했습니다. 그 결과 한 프레임에 후보 좌표 하나의 네 방향을 검사하는 현재 구조가 됐고, 문제가 생겼을 때 어느 단계의 좌표가 잘못됐는지 바로 볼 수 있었습니다.

## 프로젝트 구조

```text
Assets/
├── 03.Script/
│   ├── Boss          # Boss 패턴, BossLaser, BossSword, BulletPool
│   ├── Dungeon       # Door, GateController, StartRoom
│   ├── Item          # Weapons, WeaponsAttack
│   ├── Manager       # Game, Room, Dungeon, Health, Sound, UI, Upgrade, SceneTransition
│   ├── Monster       # Monster FSM과 CloseUp / Flying / Rushing 파생
│   ├── NPC           # 마을 NPC와 대화 트리거
│   ├── Player        # Controller, Input, Health, Equipment
│   ├── RandomMap     # Room, MiniMaps, RandomManager
│   ├── Scriptdata    # Monster / Player / Weapons ScriptableObject 정의
│   ├── UI            # DialogSystem, UpgradeUI, 스크롤 배경
│   └── etc           # ObjectPool, DamageText, SpawnZone
├── 03-1.ScriptableData   # 데이터 에셋
└── Docs/                 # 기술문서 PDF
```

스프라이트·씬·타일맵·오디오 폴더는 공개본에서 제외했습니다.

## 알고 있는 한계와 다음 과제

첫 프로젝트라 이후 작업에서 고친 기준이 몇 가지 있습니다.

- `RoomManager`는 최소 방 수를 못 채우면 재생성을 반복하고 최대 시도 횟수가 없습니다. 불가능한 설정(최소 > 최대)을 초기에 거부하고 시도 횟수를 제한하는 쪽이 안전합니다.
- `Monster.OnDamage`는 `damage - defense`를 그대로 체력에서 빼서, 방어력이 피해보다 크면 체력이 늘어나는 경로가 있습니다. 0 하한을 두는 것이 의도에 맞습니다.
- 매니저들이 싱글턴과 `DontDestroyOnLoad`에 의존합니다. KrameLife에서는 `GameContext`와 이벤트 채널로 접근 경로를 하나로 모았습니다.
- 위 항목은 코드 검토에서 찾은 것이며 실제 플레이에서 재현한 기록은 아닙니다.

## 공개 범위와 확인 방법

- 이 저장소는 **코드 열람용**입니다. 씬·프리팹·에셋이 없어 Unity Hub에서 열어도 플레이할 수 없습니다.
- 플레이 결과는 [영상](https://youtu.be/-EYzNawFw4I), 구현 설명은 [기술문서 PDF](Docs/Unity_기술문서_던레거시_양지섭.pdf)를 참고해 주세요.
- 코드 링크는 커밋 `c95ad80`에 고정돼 있어 이후 변경과 무관하게 같은 내용을 가리킵니다.

© 2026 Yang Ji-seop. Portfolio use only.
