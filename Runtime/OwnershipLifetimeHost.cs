using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jeomseon.Unity.Ownership
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class OwnershipLifetimeHost : MonoBehaviour
    {
        private readonly HashSet<IDisposable> _resources = new();
        private bool _destroying;

        public int TrackedCount => _resources.Count;

        public static OwnershipLifetimeHost GetOrAdd(Component owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            return owner.GetComponent<OwnershipLifetimeHost>() ??
                   owner.gameObject.AddComponent<OwnershipLifetimeHost>();
        }

        public IDisposable Track(IDisposable resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (_destroying) throw new ObjectDisposedException(nameof(OwnershipLifetimeHost));
            if (!_resources.Add(resource))
                throw new InvalidOperationException("The resource is already tracked by this lifetime host.");
            return new Registration(this, resource);
        }

        private void Untrack(IDisposable resource) => _resources.Remove(resource);

        private void OnDestroy()
        {
            _destroying = true;
            var resources = new IDisposable[_resources.Count];
            _resources.CopyTo(resources);
            _resources.Clear();

            foreach (var resource in resources)
            {
                try
                {
                    resource.Dispose();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }
        }

        private sealed class Registration : IDisposable
        {
            private OwnershipLifetimeHost _host;
            private IDisposable _resource;

            public Registration(OwnershipLifetimeHost host, IDisposable resource)
            {
                _host = host;
                _resource = resource;
            }

            public void Dispose()
            {
                var host = _host;
                var resource = _resource;
                _host = null;
                _resource = null;
                if (host != null) host.Untrack(resource);
            }
        }
    }
}
