# 변경 기록

## [0.1.0] - Unreleased

- 도메인 중립 ownership handle, 교체 가능한 slot과 GameObject 수명 host를 추가했습니다.
- `[ManagedResource]` 파생 어트리뷰트용 Source Generator와 `JMO1010`~`JMO1012` Analyzer를 추가했습니다.
- 같은 Host에 동일 handle을 중복 등록하면 즉시 실패하도록 이중 소유를 차단했습니다.
- Edit Mode에서 owner 또는 host를 제거해도 추적 리소스를 해제하도록 lifetime host를 항상 실행합니다.
- 유효한 managed-resource 선언에 생성 값의 borrowed 수명 계약을 알리는 `JMO1020` 경고를 추가했습니다.
- borrowed 값의 persistent 저장, 반환, collection 저장과 closure capture를 진단하는
  `JMO1021`~`JMO1023`을 추가했습니다.
- 로컬 별칭을 따라 같은 escape를 판정하고 `await` 이후 사용을 `JMO1024`로 진단합니다.
- 조건 분기의 가능한 대입을 병합하고 조건식과 null 병합식의 borrowed 출처를 추적합니다.
- `await` 전 별도 대입과 이후 재대입을 구분하고 반복문·예외 분기의 가능한 borrowed 값을 추적합니다.
- 생성된 `Take`는 요청 타입을 먼저 검증해 잘못된 Take가 lifetime host 등록을 제거하지 않도록 했습니다.
- 소스가 보이는 호출 대상의 parameter capture를 `JMO1025`로 진단하고 `[DoesNotCapture]` 계약 위반을
  `JMO1013` 오류로 진단합니다.
- 의도적 escape는 근거가 포함된 표준 `SuppressMessage`를 사용하도록 문서화했습니다.
- 생성 필드에서 lease ownership을 빼내는 `Take{Name}()`과 Addressables 단일 asset 및 collection을 독립 보유하는
  `Retain{Name}()`을 추가했습니다.
