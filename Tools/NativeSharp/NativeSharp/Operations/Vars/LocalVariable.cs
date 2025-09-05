namespace NativeSharp.Operations.Vars;

public class LocalVariable : IndexedVariable
{
    public override string ToString() => GenCodeImpl();
    public override string GenCodeImpl() => $"local_{Index}";
}