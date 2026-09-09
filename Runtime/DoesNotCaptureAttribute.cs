using System;

namespace Jeomseon.Unity.Ownership
{
    /// <summary>
    /// Declares that a parameter is consumed during the call and is not stored or returned.
    /// 매개변수를 호출 중에만 사용하며 저장하거나 반환하지 않음을 선언합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class DoesNotCaptureAttribute : Attribute
    {
    }
}
