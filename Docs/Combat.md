# Combat System

## 설계 철학

모든 공격은 **투사체(Projectile)** 를 생성하는 방식으로 통합된다.  
근접/원거리를 별도 로직으로 분리하지 않고, 투사체의 속도와 수명 파라미터로 구분한다.

| 공격 유형 | 속도 | 수명 | 비고 |
|-----------|------|------|------|
| 근접 (Melee) | 느림 | 긺 | 몸 바로 앞에서 천천히 소멸 |
| 원거리 (Ranged) | 빠름 | 짧음 | 멀리 날아가지만 금방 소멸 |

플레이어와 적 모두 동일한 Projectile 시스템을 사용한다.

---

## Projectile 데이터 구조

```csharp
// Assets/Scripts/Combat/ProjectileData.cs
[CreateAssetMenu]
public class ProjectileData : ScriptableObject
{
    public float speed;         // 투사체 이동 속도
    public float lifetime;      // 투사체 유효 시간 (초)
    public float damage;        // 피해량
    public LayerMask hitLayers; // 충돌 대상 레이어
}
```

### 기본값 가이드라인

| 파라미터 | 근접 | 원거리 |
|----------|------|--------|
| `speed` | 1 ~ 3 | 10 ~ 20 |
| `lifetime` | 0.3 ~ 0.6 | 0.1 ~ 0.2 |

---

## Projectile 컴포넌트

```
Assets/Scripts/Combat/
├── ProjectileData.cs       // ScriptableObject — 투사체 스탯 정의
├── Projectile.cs           // MonoBehaviour — 이동, 충돌, 수명 처리
└── AttackController.cs     // MonoBehaviour — 공격 입력 → 투사체 발사
```

### Projectile.cs 책임

- `ProjectileData`를 받아 이동 벡터와 수명 타이머를 구동
- 충돌 시 `IDamageable.TakeDamage(damage)` 호출
- 수명 만료 또는 충돌 시 오브젝트 제거 (풀링 대응 가능하도록 `Deactivate()` 래핑)

### AttackController.cs 책임

- 공격 입력(또는 AI 명령)을 받아 지정된 `ProjectileData`로 투사체 Instantiate
- 발사 방향은 캐릭터가 바라보는 방향 기준
- 플레이어와 적 모두 이 컴포넌트를 재사용

---

## IDamageable 인터페이스

```csharp
// Assets/Scripts/Combat/IDamageable.cs
public interface IDamageable
{
    void TakeDamage(float amount);
}
```

체력을 가진 모든 엔티티(플레이어, 적)가 구현한다.

---

## 공격 흐름

```
입력 / AI 명령
    │
    ▼
AttackController
    │  ProjectileData 선택 (근접 or 원거리)
    ▼
Projectile Instantiate (발사 방향, speed, lifetime 주입)
    │
    ├─ 수명 만료 → Deactivate
    └─ hitLayers 충돌 → IDamageable.TakeDamage → Deactivate
```

---

## 레이어 규칙

| 레이어 | 설명 |
|--------|------|
| `PlayerProjectile` | 플레이어가 쏜 투사체 — 적에게만 충돌 |
| `EnemyProjectile` | 적이 쏜 투사체 — 플레이어에게만 충돌 |

투사체 `hitLayers`를 `ProjectileData`에서 설정해 팀 피해를 방지한다.

---

## 미결 사항

- [ ] 오브젝트 풀링 도입 여부
- [ ] 투사체 관통(Piercing) 옵션 지원
- [ ] 넉백 처리 위치 (Projectile vs IDamageable 구현체)
- [ ] 투사체 스프라이트 / 애니메이션 연동 방식
