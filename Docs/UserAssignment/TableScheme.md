# 정적 데이터 테이블 스키마 (TableScheme)

> 작성: 2026-03-30
> 실제 구현은 TSV 기반 커스텀 데이터 테이블 모듈을 통해 동작

---

## 컬럼 타입 정의

| 타입 | 설명 |
|------|------|
| `int` | 32비트 정수 |
| `long` | 64비트 정수 |
| `float` | 단정밀도 실수 |
| `string` | 문자열 |
| `int-list` | int 배열 |
| `long-list` | long 배열 |
| `float-list` | float 배열 |
| `string-list` | string 배열 |

> bool 타입 없음 — 0/1 int로 표현
> enum 타입 없음 — int로 표현, 코드 내 enum 정의와 대응

---

## 스키마 목록

### Item — 아이템 정의

```
Item-Id-int
Item-Name-string
Item-Description-string
Item-Category-int
Item-IsStackable-int
Item-MaxStackSize-int
Item-IconPath-string
```

> `Category`: 추후 enum 정의 필요 (소비템, 재료, 장비 등)
> `IsStackable`: 0 = 불가, 1 = 가능
> `MaxStackSize`: IsStackable이 0이면 1로 고정

---

### EnemyData — 적 유닛 정의

```
EnemyData-Id-int
EnemyData-Name-string
EnemyData-MaxHp-int
EnemyData-MoveSpeed-float
EnemyData-WanderRadius-float
EnemyData-DetectionRange-float
EnemyData-AttackRange-float
EnemyData-AttackDamage-float
EnemyData-AttackCooldown-float
EnemyData-PrefabPath-string
```

> AI 구조 미결로 향후 컬럼 추가 가능성 높음 (보스 행동 플래그, 특수 패턴 ID 등)

---

### MapData — 맵 정의

```
MapData-Id-int
MapData-Name-string
MapData-SceneName-string
```

> `SceneName`: Unity 씬 에셋 이름과 일치

---

### MissionData — 미션 정의

```
MissionData-Id-int
MissionData-Name-string
MissionData-MapId-int
MissionData-EnemySpawnGroupId-int
MissionData-LootGroupId-int
```

> `MapId`: MapData.Id 참조
> `EnemySpawnGroupId`: EnemySpawnTable.GroupId 참조
> `LootGroupId`: LootTable.GroupId 참조

---

### LootTable — 루트 테이블 항목

```
LootTable-Id-int
LootTable-GroupId-int
LootTable-ItemId-int
LootTable-MinCount-int
LootTable-MaxCount-int
LootTable-Weight-int
```

> `GroupId`로 그루핑. MissionData.LootGroupId가 이를 참조
> `Weight`: 해당 항목이 선택될 상대적 가중치

---

### EnemySpawnTable — 미션 적 스폰 구성

```
EnemySpawnTable-Id-int
EnemySpawnTable-GroupId-int
EnemySpawnTable-EnemyId-int
EnemySpawnTable-MinCount-int
EnemySpawnTable-MaxCount-int
EnemySpawnTable-Weight-int
```

> `GroupId`로 그루핑. MissionData.EnemySpawnGroupId가 이를 참조
> `Weight`: 해당 적 종류가 선택될 상대적 가중치

---

## 참조 관계 요약

```
MissionData
 ├─ MapId          → MapData.Id
 ├─ LootGroupId    → LootTable.GroupId
 │                     └─ ItemId → Item.Id
 └─ EnemySpawnGroupId → EnemySpawnTable.GroupId
                            └─ EnemyId → EnemyData.Id
```

---

## 미결 / 확장 예정

- 장비 시스템 확정 시 `EquipmentData` 테이블 추가 필요
- 스탯 시스템 확정 시 `CharacterStatData` 테이블 추가 필요
- 성장 구조 확정 시 관련 테이블 추가 필요 (스킬, 업그레이드 등)
- 보스 등 특수 AI 구조 확정 시 `EnemyData` 컬럼 확장 필요
