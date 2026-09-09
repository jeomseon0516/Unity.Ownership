using System;

namespace Jeomseon.Unity.Ownership
{
    /// <summary>Marks a handle field for ownership source generation and analysis.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ManagedResourceAttribute : Attribute
    {
    }
}
