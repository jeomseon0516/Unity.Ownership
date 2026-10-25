# Jeomseon Unity Ownership

[한국어](Overview.md) · [Installation](../README.en.md)

## Purpose

Unifies replacement, ownership transfer, and release of acquired resources. A Source Generator creates safe access paths for fields in partial Components.

## Usage flow

Declare a domain Managed attribute, pass a handle to generated Set, borrow through the getter, transfer through Take or domain Retain, then release on replacement/owner GameObject destruction.

## Contracts and limitations

Existing OnDestroy is not rewritten. The host follows the shared GameObject lifetime, not individual Component destruction. Analyzer diagnostics cover supported syntax/source and are not whole-program proofs.

## Package requirements

- Package ID: `com.jeomseon.unity.ownership`
- Version: `0.1.0`
- Unity: `6000.6` / `0f1`

## Samples and verification

Import the following from Package Manager > Samples. Open the included Scene or Editor tool; follow any Setup step in its sample README.

- [Basic Usage](../Samples~/BasicUsage/README.md)
  - [OwnershipBasicUsage](../Samples~/BasicUsage/OwnershipBasicUsage.unity)

[Test sources](../Tests)

Add the package ID to manifest `testables`, then run the relevant Unity Test Runner assemblies. These links describe available tests, not a new successful test run.

## Source map

- [DoesNotCaptureAttribute](../Runtime/DoesNotCaptureAttribute.cs)
- [IOwnershipHandle](../Runtime/IOwnershipHandle.cs)
- [ManagedResourceAttribute](../Runtime/ManagedResourceAttribute.cs)
- [OwnershipLifetimeHost](../Runtime/OwnershipLifetimeHost.cs)
- [OwnershipSlot](../Runtime/OwnershipSlot.cs)
