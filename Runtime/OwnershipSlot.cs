using System;

namespace Jeomseon.Unity.Ownership
{
    /// <summary>Owns at most one handle and disposes the previous handle when replaced.</summary>
    public sealed class OwnershipSlot<T> : IDisposable
    {
        private readonly OwnershipLifetimeHost _host;
        private IOwnershipHandle<T> _handle;
        private IDisposable _hostRegistration;
        private bool _disposed;

        public OwnershipSlot(OwnershipLifetimeHost host = null) => _host = host;

        public bool HasValue => _handle is { IsValid: true };

        public T Value => HasValue
            ? _handle.Value
            : throw new InvalidOperationException("The ownership slot has no valid value.");

        public void Set(IOwnershipHandle<T> handle)
        {
            ThrowIfDisposed();
            if (ReferenceEquals(_handle, handle)) return;
            if (handle != null && !handle.IsValid)
                throw new ArgumentException("The ownership handle must be valid.", nameof(handle));

            var nextRegistration = handle == null ? null : _host?.Track(handle);
            var previousHandle = _handle;
            var previousRegistration = _hostRegistration;
            _handle = handle;
            _hostRegistration = nextRegistration;
            previousRegistration?.Dispose();
            previousHandle?.Dispose();
        }

        public bool TryGetValue(out T value)
        {
            if (HasValue)
            {
                value = _handle.Value;
                return true;
            }

            value = default;
            return false;
        }

        public IOwnershipHandle<T> Take()
        {
            ThrowIfDisposed();
            var handle = _handle;
            _handle = null;
            _hostRegistration?.Dispose();
            _hostRegistration = null;
            return handle;
        }

        public void Clear() => Set(null);

        public void Dispose()
        {
            if (_disposed) return;
            Clear();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(OwnershipSlot<T>));
        }
    }
}
