using System;
using NUnit.Framework;
using UnityEngine;

namespace Jeomseon.Unity.Ownership.Tests
{
    public sealed class OwnershipSlotTests
    {
        [Test]
        public void Set_ReplacesAndDisposesPreviousHandle()
        {
            var first = new FakeHandle("First");
            var second = new FakeHandle("Second");
            using var slot = new OwnershipSlot<string>();

            slot.Set(first);
            slot.Set(second);

            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(slot.Value, Is.EqualTo("Second"));
        }

        [Test]
        public void Take_TransfersWithoutDisposing()
        {
            var handle = new FakeHandle("Value");
            using var slot = new OwnershipSlot<string>();
            slot.Set(handle);

            var taken = slot.Take();

            Assert.That(taken, Is.SameAs(handle));
            Assert.That(handle.DisposeCount, Is.Zero);
            Assert.That(slot.HasValue, Is.False);
        }

        [Test]
        public void HostDestroy_DisposesTrackedHandle()
        {
            var owner = new GameObject("Owner");
            var host = owner.AddComponent<OwnershipLifetimeHost>();
            var handle = new FakeHandle("Value");
            var slot = new OwnershipSlot<string>(host);
            slot.Set(handle);

            UnityEngine.Object.DestroyImmediate(owner);

            Assert.That(handle.DisposeCount, Is.EqualTo(1));
            slot.Dispose();
            Assert.That(handle.DisposeCount, Is.EqualTo(1));
        }

        [Test]
        public void Set_InvalidHandle_ThrowsArgumentException()
        {
            using var slot = new OwnershipSlot<string>();
            var handle = new FakeHandle("Value");
            handle.Dispose();

            Assert.Throws<ArgumentException>(() => slot.Set(handle));
        }

        [Test]
        public void Track_SameHandleTwice_ThrowsInvalidOperationException()
        {
            var owner = new GameObject("Owner");
            var host = owner.AddComponent<OwnershipLifetimeHost>();
            var handle = new FakeHandle("Value");
            var registration = host.Track(handle);

            Assert.Throws<InvalidOperationException>(() => host.Track(handle));

            registration.Dispose();
            UnityEngine.Object.DestroyImmediate(owner);
            Assert.That(handle.DisposeCount, Is.Zero);
        }

        private sealed class FakeHandle : IOwnershipHandle<string>
        {
            public FakeHandle(string value) => Value = value;

            public string Value { get; }
            public bool IsValid { get; private set; } = true;
            public int DisposeCount { get; private set; }

            public void Dispose()
            {
                if (!IsValid) return;
                IsValid = false;
                DisposeCount++;
            }
        }
    }
}
