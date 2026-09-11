# Jeomseon Unity Ownership

Addressables와 GameObject Pooling처럼 획득 방식이 다른 리소스의 **교체·양도·해제** 수명을 통일하는
도메인 중립 계약입니다.

- `IOwnershipHandle<T>`: 획득이 끝난 값과 정확히 한 번의 해제 책임
- `OwnershipSlot<T>`: 새 handle 대입 시 이전 handle 자동 해제, `Take()`로 명시적 소유권 양도
- `OwnershipLifetimeHost`: owner GameObject 파괴 시 등록된 handle 일괄 해제
- `[ManagedResource]` 파생 어트리뷰트: partial `Component`의 리소스 필드에 값 getter와 안전한
  `Set{Name}`/`Clear{Name}` 메서드를 Source Generator로 생성
- Analyzer: `partial` 누락(`JMO1010`), 잘못된 handle(`JMO1011`), 지원하지 않는 owner(`JMO1012`)를
  컴파일 오류로 진단합니다. borrowed 선언(`JMO1020`), persistent 저장(`JMO1021`), 반환(`JMO1022`),
  collection 저장·closure capture(`JMO1023`)를 경고합니다.
  로컬 borrowed 값이 `await` 경계를 넘어 사용되면 `JMO1024`로 경고합니다.
  서로 다른 조건 분기의 대입과 조건식·null 병합식도 borrowed 출처로 추적합니다.
  반복문과 예외 분기의 가능한 대입도 합치며, `await` 이후 확정 재대입은 이전 borrow 경고를 제거합니다.
  소스가 보이는 메서드가 매개변수를 저장·반환·컬렉션 저장·캡처하면, borrowed 인자 전달을
  `JMO1025`로 경고합니다. 즉시 소비 매개변수에는 `[DoesNotCapture]`를 선언할 수 있으며 실제 구현이
  계약을 위반하면 `JMO1013` 컴파일 오류가 발생합니다.

정적 Registry를 사용하지 않습니다. 도메인 패키지가 이미 제공하는 lease/handle을 생성 코드가 직접
보관하므로, ownership 전용 Wrapper도 필요하지 않습니다. Getter로 얻은 값은 owner에게 빌린 참조입니다.

```csharp
public partial class EnemyView : MonoBehaviour
{
    [ManagedPooledObject] private GameObject _enemy;

    public void Spawn(GameObjectPoolHandle pool) => SetEnemy(pool.SpawnOwned());
    public GameObject BorrowEnemy() => Enemy;
}
```

생성된 setter는 같은 GameObject의 `OwnershipLifetimeHost`에 handle을 등록합니다. 기존 `OnDestroy`
메서드는 수정하거나 대체하지 않습니다. 생성된 `Take{Name}()`은 해제하지 않고 ownership을 호출자에게
이동합니다. Addressables 단일 asset과 collection은 `Retain{Name}()`으로 독립 lease를 만들 수 있습니다.
여러 handle 종류를 받을 수 있는 필드에서 잘못된 Take를 호출하면 소유권을 분리하기 전에 예외가 발생합니다.

```csharp
private void InspectNow([DoesNotCapture] Sprite sprite)
{
    Debug.Log(sprite.name); // 이 메서드 밖에 보관하지 않는 계약
}
```

의도적으로 owner와 같은 수명 안에서 borrowed 값을 전달하는 예외는 표준 suppression으로 근거를
코드에 남깁니다. 별도 no-op Wrapper는 제공하지 않습니다.

```csharp
[SuppressMessage("Ownership", "JMO1022",
    Justification = "호출자가 이 owner와 같은 수명에 묶여 있습니다.")]
public Sprite BorrowPortrait() => Portrait;
```
