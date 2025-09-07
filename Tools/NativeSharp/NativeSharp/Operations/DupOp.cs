using NativeSharp.Operations.Common;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Operations;

internal class DupOp : BaseOp
{
    public VReg Vreg1 { get; }
    public VReg Vreg2 { get; }
    public IValueExpression Original { get; }

    public DupOp(VReg vreg1, VReg vreg2, IValueExpression original)
    {
        Vreg1 = vreg1;
        Vreg2 = vreg2;
        Original = original;
    }

    public override string GenCode()
    {
        return $"{Vreg1.GenCode()} = {Original.Code()}; {Vreg2.GenCode()} = {Vreg1.Code()};";
    }
}