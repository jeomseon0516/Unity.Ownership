# Jeomseon Unity Ownership

[한국어](README.md) | English

Unifies replacement, ownership transfer, and release of acquired resources. A Source Generator creates safe access paths for fields in partial Components.

## Quick start

Declare a domain Managed attribute, pass a handle to generated Set, borrow through the getter, transfer through Take or domain Retain, then release on replacement/owner GameObject destruction.

Complete sample: [OwnershipBasicUsageSample.cs](Samples~/BasicUsage/OwnershipBasicUsageSample.cs).

```csharp
using System;
using UnityEngine;

namespace Jeomseon.Unity.Ownership.Samples.BasicUsage
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ManagedDemoResourceAttribute : ManagedResourceAttribute
    {
    }

    public partial class OwnershipBasicUsageSample : MonoBehaviour
    {
        [ManagedDemoResource] private DemoHandle _statusHandle;
        private int _created;
        private int _disposed;

        private void Start() => Replace();

        private void Replace() => SetStatus(new DemoHandle($"Resource #{++_created}", () => _disposed++));

        private void Clear() => SetStatus(null);

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20f, 20f, 460f, 180f), GUI.skin.box);
            GUILayout.Label("Ownership Source Generator — Basic Usage");
            GUILayout.Label($"Borrowed value: {Status ?? "<none>"}");
            GUILayout.Label($"Created: {_created}   Disposed: {_disposed}");
            if (GUILayout.Button("Replace owned resource")) Replace();
            if (GUILayout.Button("Clear owned resource")) Clear();
            GUILayout.Label("Stop Play Mode or delete this object to test host disposal.");
            GUILayout.EndArea();
        }

        public sealed class DemoHandle : IOwnershipHandle<string>
        {
            private Action _release;
            public DemoHandle(string value, Action release) { Value = value; _release = release; }
            public string Value { get; }
            public bool IsValid => _release != null;
            public void Dispose() { var release = _release; _release = null; release?.Invoke(); }
        }
    }
}
```

## Contracts and limitations

Existing OnDestroy is not rewritten. The host follows the shared GameObject lifetime, not individual Component destruction. Analyzer diagnostics cover supported syntax/source and are not whole-program proofs.

## Requirements

- Unity `6000.6.0f1`
- Package: `com.jeomseon.unity.ownership` `0.1.0`

## Installation

Merge this into your project's `Packages/manifest.json`; preserve existing dependencies and registries.

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.jeomseon"
      ]
    }
  ],
  "dependencies": {
    "com.jeomseon.unity.ownership": "0.1.0"
  }
}
```

Or use **Install package from git URL** in Package Manager:

```text
https://github.com/jeomseon0516/Unity.Ownership.git#v0.1.0
```

## API usage and details

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

## Samples

Import the desired entry from **Package Manager > Samples**. Use the included Scene/Editor tool and complete its Setup step first, if required.

- [Basic Usage](Samples~/BasicUsage/README.md)
  - [OwnershipBasicUsage](Samples~/BasicUsage/OwnershipBasicUsage.unity)

## Tests

[Test sources](Tests)

Add the package ID to manifest `testables`, then execute its relevant EditMode/PlayMode assemblies in Unity Test Runner.

## Further reading

[Complete source map and package metadata](Documentation~/Overview.en.md)
- [Changelog](CHANGELOG.md)

## License

[License](LICENSE.md)
