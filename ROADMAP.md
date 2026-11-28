# Ownership 로드맵

## 0.1

- 공용 `IOwnershipHandle<T>`와 `OwnershipSlot<T>` Runtime 계약
- `OwnershipLifetimeHost` owner 파괴 자동 해제
- Addressables 및 GameObjectPooling 도메인 어댑터 실증
- `[ManagedResource]` Source Generator와 partial/handle/owner 기초 Analyzer
- persistent field/property 대입, return, collection Add, closure capture escape 진단
- 로컬 별칭 추적과 await 경계 escape 진단
- 조건 분기의 가능한 대입 병합 및 조건식·null 병합식 추적
- 반복문·예외 분기의 가능한 대입과 await 전후 재대입 추적
- 호출 대상의 parameter capture 분석과 `[DoesNotCapture]` 계약 검증
- 생성된 ownership transfer와 Addressables 단일 asset 및 collection retain 경로

## 이후

- 반복문·예외 흐름을 포함하는 Control Flow Graph 기반 별칭 분석
- partial/set/retain/transfer Code Fix
- 실제 프로젝트 사용 결과를 바탕으로 Ownership History/Graph 필요성 재평가

## 작업 기록

### OWN-GEN-COLLISIONS — 생성 멤버 이름 충돌 진단

- 브랜치: `fix/generated-member-collisions`
- 기준: `8537a22509b8e24ec0a42679c21527fe965e000b`
- 상태: `_status`, `status`, `__status` 정규화 충돌과 사용자 작성 `Status` 충돌을 `JMO1014`로
  결정적으로 진단하며, 충돌 필드는 생성에서 제외합니다.
- 검증: Roslyn 생성기/분석기 프로젝트 빌드와 독립 진단 probe 통과. 최종 커밋 SHA 및 Unity
  EditMode/PlayMode 결과는 게시 체크포인트에서 갱신합니다.
- 재시작 지점: 충돌 PR 완료 뒤 아래 접근 제어 API를 별도 작업으로 확정하고 구현합니다.

### 접근 제어 API 제안(별도 후속 작업)

- 충돌 진단은 기존 공개 API의 안전성 수정이므로 먼저 독립 배포하고, 접근 제어는 새 공개 계약이라
  별도 변경으로 검토합니다.
- `ManagedResourceAttribute`에 기본값이 `Public`인 접근 옵션을 두면 파생 어트리뷰트에도 이름 지정
  인자로 전달됩니다. 기존 코드는 현재와 동일하게 공개 멤버를 생성합니다.
- 값 getter와 mutator 접근을 분리합니다. mutator 옵션은 `Set`, `Clear`, `Take`, `Retain` 전체에
  일관되게 적용해 일부 경로만 우회 공개되는 일을 막습니다.
- `Private` mutator를 선택하면 같은 partial 형식에 사용자가 작성한 공개 도메인 메서드가 private
  생성 setter를 호출할 수 있습니다. enum 값, 상속된 어트리뷰트의 옵션 해석, 각 mutator 종류별
  생성 결과와 기존 기본값 호환성을 계약 테스트로 고정한 뒤 구현합니다.
