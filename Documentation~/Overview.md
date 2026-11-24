# Jeomseon Unity Ownership

[English](Overview.en.md) · [설치](../README.md)

## 목적

획득 완료된 리소스의 교체·소유권 양도·해제를 통일합니다. Source Generator가 partial Component의 필드에 안전한 접근 경로를 생성합니다.

## 사용 흐름

도메인 Managed Attribute 선언 → 생성 Set 메서드로 handle 전달 → getter로 borrow → 필요시 Take로 양도 또는 domain Retain → 교체/owner GameObject 파괴 시 해제.

## 계약과 제한

기존 OnDestroy는 수정하지 않습니다. Host는 Component 개별 파괴가 아닌 같은 GameObject 수명을 따릅니다. Analyzer는 지원하는 구문·소스 범위의 진단이며 전체 프로그램의 안전성 증명이 아닙니다.

## 패키지 요구 사항

- Package ID: `com.jeomseon.unity.ownership`
- Version: `0.1.0`
- Unity: `6000.6` / `0f1`

## 샘플과 검증

Package Manager > Samples에서 다음 예제를 Import하세요. 포함된 Scene 또는 Editor 도구를 열고 샘플 README의 Setup 절차를 먼저 수행합니다.

- [Basic Usage](../Samples~/BasicUsage/README.md)
  - [OwnershipBasicUsage](../Samples~/BasicUsage/OwnershipBasicUsage.unity)

[테스트 코드](../Tests)

소비 프로젝트 manifest의 `testables`에 패키지 ID를 추가하고 Unity Test Runner의 관련 어셈블리를 실행합니다. 위 링크는 테스트 위치이며 이번 문서 수정으로 새 테스트 실행을 주장하지 않습니다.

## 코드 지도

- [DoesNotCaptureAttribute](../Runtime/DoesNotCaptureAttribute.cs)
- [IOwnershipHandle](../Runtime/IOwnershipHandle.cs)
- [ManagedResourceAttribute](../Runtime/ManagedResourceAttribute.cs)
- [OwnershipLifetimeHost](../Runtime/OwnershipLifetimeHost.cs)
- [OwnershipSlot](../Runtime/OwnershipSlot.cs)
