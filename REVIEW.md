# Review guide

- `IOwnershipHandle<T>`의 Dispose는 멱등이고 교체·양도·owner 파괴 경계가 정확히 한 번 해제되는지 확인합니다.
- Source Generator는 기존 lifecycle 메서드를 생성하거나 수정하지 않고 `OwnershipLifetimeHost`를 사용해야 합니다.
- 정적 ownership registry를 추가하지 않습니다.
- `JMO1021`~`JMO1025`가 로컬 사용을 escape로 오인하지 않는지 확인합니다.
- `[DoesNotCapture]`는 구현이 매개변수를 저장·반환·collection 추가·closure capture하지 않을 때만
  허용하며 계약 위반은 `JMO1013` 오류여야 합니다.
