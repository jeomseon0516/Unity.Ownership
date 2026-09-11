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
