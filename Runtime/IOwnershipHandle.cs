using System;

namespace Jeomseon.Unity.Ownership
{
    /// <summary>Owns one acquired value until disposed or transferred.</summary>
    public interface IOwnershipHandle<out T> : IDisposable
    {
        T Value { get; }
        bool IsValid { get; }
    }
}
