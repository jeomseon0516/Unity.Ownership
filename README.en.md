# Jeomseon Unity Ownership

This package provides domain-neutral ownership handles, replaceable slots, and Unity lifetime hosting.

- `IOwnershipHandle<T>` owns an acquired value and one release responsibility.
- `OwnershipSlot<T>` disposes the previous handle when replaced and transfers it with `Take()`.
- `OwnershipLifetimeHost` disposes tracked handles when its GameObject is destroyed.
- The bundled source generator creates a borrowed-value property and `Set{Name}`/`Clear{Name}` methods for fields
  marked with an attribute derived from `ManagedResourceAttribute`.
- The analyzer reports missing `partial` declarations (`JMO1010`), invalid handle fields (`JMO1011`), and
  unsupported owners (`JMO1012`).
- `JMO1020` warns at each valid managed-resource declaration that the generated value is borrowed and must not
  outlive its owner without transferring the handle.
- `JMO1021`-`JMO1023` detect direct persistent assignment, return, persistent collection storage, and closure
  capture of generated borrowed values.
- `JMO1024` warns when a borrowed local is used after an `await` boundary.
- Alias tracking merges possible assignments from conditional branches and follows conditional and coalescing expressions.
- Loop and exception branches are treated as possible sources. A definite reassignment after `await` prevents a stale crossing warning.
- `JMO1025` reports borrowed arguments passed to source-visible methods that store, return, collect, or capture
  their parameter. `[DoesNotCapture]` declares immediate consumption; `JMO1013` is an error when the method body
  violates that contract.

Generated setters store the domain package's existing lease or handle directly and register it with a host on the
same GameObject. No ownership-only wrapper is introduced. The generator does not modify an existing `OnDestroy`.
Values returned by generated properties are borrowed references.
Generated `Take{Name}()` methods transfer ownership without disposal. Addressables assets also receive
`Retain{Name}()` for an independent lease.
For fields that accept multiple handle kinds, a mismatched take fails before ownership is detached.
Intentional exceptions use the standard `SuppressMessage` attribute with a justification. The package does not
add a no-op unsafe-borrow wrapper.
