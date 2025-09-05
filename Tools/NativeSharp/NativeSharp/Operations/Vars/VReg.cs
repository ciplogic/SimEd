namespace NativeSharp.Operations.Vars;

public class VReg : IndexedVariable
{
    public override string ToString() => $"vreg_{Index}";

    public override string GenCodeImpl() => $"vreg_{Index}";
}