namespace NativeSharp.Operations.Vars;

public class ArgumentVariable : IndexedVariable
{
    public override string GenCodeImpl() => $"arg_{Index}";
}