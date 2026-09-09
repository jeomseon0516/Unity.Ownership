using System;
using System.Collections.Immutable;
using System.Linq;
using Jeomseon.Unity.Ownership.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

const string source = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace UnityEngine { public class Object {} public class Component : Object {} public class MonoBehaviour : Component {} public class Sprite : Object {} }
namespace Jeomseon.Unity.Ownership { public abstract class ManagedResourceAttribute : Attribute {} [AttributeUsage(AttributeTargets.Parameter)] public sealed class DoesNotCaptureAttribute : Attribute {} public sealed class OwnershipLifetimeHost { public static OwnershipLifetimeHost GetOrAdd(UnityEngine.Component owner) => new OwnershipLifetimeHost(); public IDisposable Track(IDisposable resource) => resource; } }
namespace Jeomseon.Unity.Addressables.Ownership { public sealed class ManagedAssetAttribute : Jeomseon.Unity.Ownership.ManagedResourceAttribute {} public sealed class ManagedAssetCollectionAttribute : Jeomseon.Unity.Ownership.ManagedResourceAttribute {} }
namespace Jeomseon.Unity.Addressables { public sealed class AddressableAssetLease<T> : IDisposable where T : UnityEngine.Object { public T Asset => null; public bool IsValid => true; public AddressableAssetLease<T> Retain() => this; public void Dispose() {} } public sealed class AddressableAssetCollectionLease<T> : IDisposable where T : UnityEngine.Object { public IReadOnlyList<T> Assets => null; public bool IsValid => true; public AddressableAssetCollectionLease<T> Retain() => this; public void Dispose() {} } }
public partial class Owner : UnityEngine.MonoBehaviour
{
    [Jeomseon.Unity.Addressables.Ownership.ManagedAsset] private UnityEngine.Sprite _sprite;
    [Jeomseon.Unity.Addressables.Ownership.ManagedAssetCollection] private IReadOnlyList<UnityEngine.Sprite> _sprites;
    [Jeomseon.Unity.Addressables.Ownership.ManagedAssetCollection] private IReadOnlyList<string> _invalidAssets;
    private UnityEngine.Sprite _stored;
    private List<UnityEngine.Sprite> _items = new List<UnityEngine.Sprite>();
    public void Store(Owner other) { var local = other.Sprite; _stored = local; _items.Add(local); Action capture = () => Use(local); }
    public void BranchEscape(Owner other, bool condition) { UnityEngine.Sprite branch = null; if (condition) { branch = other.Sprite; } else { branch = null; } _stored = branch; }
    public void DefiniteOverwrite(Owner other) { var overwritten = other.Sprite; overwritten = null; _stored = overwritten; }
    public void ConditionalEscape(Owner other, bool condition) { var conditional = condition ? other.Sprite : null; _stored = conditional; }
    public void LoopEscape(Owner other, bool condition) { UnityEngine.Sprite loop = null; while (condition) { loop = other.Sprite; condition = false; } _stored = loop; }
    public void ExceptionEscape(Owner other) { UnityEngine.Sprite value = null; try { value = other.Sprite; } catch { value = null; } _stored = value; }
    public UnityEngine.Sprite Escape(Owner other) { var local = other.Sprite; return local; }
    [System.Diagnostics.CodeAnalysis.SuppressMessage(""Ownership"", ""JMO1022"", Justification = ""Caller is bound to the same owner lifetime."")]
    public UnityEngine.Sprite IntentionalBorrow(Owner other) => other.Sprite;
    public async Task CrossAwait(Owner other) { var local = other.Sprite; await Task.Yield(); Use(local); }
    public async Task AssignedBeforeAwait(Owner other) { UnityEngine.Sprite local = null; local = other.Sprite; await Task.Yield(); Use(local); }
    public async Task ReassignedAfterAwait(Owner other) { var local = other.Sprite; await Task.Yield(); local = null; Use(local); }
    public void CallCapture(Owner other) { Capture(other.Sprite); Consume(other.Sprite); }
    private void Capture(UnityEngine.Sprite value) { var alias = value; _stored = alias; }
    private void Consume([Jeomseon.Unity.Ownership.DoesNotCapture] UnityEngine.Sprite value) { _ = value; }
    private void InvalidContract([Jeomseon.Unity.Ownership.DoesNotCapture] UnityEngine.Sprite value) { var alias = value; _stored = alias; }
    private UnityEngine.Sprite InvalidReturn([Jeomseon.Unity.Ownership.DoesNotCapture] UnityEngine.Sprite value) => value;
    private void InvalidClosure([Jeomseon.Unity.Ownership.DoesNotCapture] UnityEngine.Sprite value) { Action action = () => Use(value); }
    private void Use(UnityEngine.Sprite value) {}
}";

var syntaxTree = CSharpSyntaxTree.ParseText(source);
var references = AppDomain.CurrentDomain.GetAssemblies()
    .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
var compilation = CSharpCompilation.Create(
    "AnalyzerProbe",
    new[] { syntaxTree },
    references,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
CSharpGeneratorDriver.Create(new ManagedResourceGenerator()).RunGeneratorsAndUpdateCompilation(
    compilation,
    out var generatedCompilation,
    out var generatorDiagnostics);
if (generatorDiagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
    throw new InvalidOperationException(string.Join(Environment.NewLine, generatorDiagnostics));
compilation = (CSharpCompilation)generatedCompilation;
var compilerErrors = compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
if (compilerErrors.Length > 0)
    throw new InvalidOperationException(string.Join(Environment.NewLine, compilerErrors.Select(diagnostic => diagnostic.ToString())));
var diagnostics = compilation.WithAnalyzers(
    ImmutableArray.Create<DiagnosticAnalyzer>(new ManagedResourceAnalyzer()))
    .GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();

Require("JMO1021", 5);
Require("JMO1022", 1);
Require("JMO1023", 2);
Require("JMO1024", 2);
Require("JMO1025", 1);
Require("JMO1013", 3);
Require("JMO1011", 1);
Require("JMO1020", 2);
if (diagnostics.Any(diagnostic => diagnostic.Id == "JMO1021" && diagnostic.GetMessage().Contains("local")))
    throw new InvalidOperationException("A local borrowed value was incorrectly classified as a persistent escape.");

Console.WriteLine("Analyzer escape probes passed: " + string.Join(", ", diagnostics.Select(diagnostic => diagnostic.Id).OrderBy(id => id)));

void Require(string id, int count)
{
    var actual = diagnostics.Count(diagnostic => diagnostic.Id == id);
    if (actual != count)
        throw new InvalidOperationException($"Expected {count} {id} diagnostic(s), but found {actual}.\n{string.Join(Environment.NewLine, diagnostics)}");
}
