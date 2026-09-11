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
