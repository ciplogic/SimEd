using NativeSharp.Extensions;
using NativeSharp.Operations;
using NativeSharp.Operations.Vars;

namespace NativeSharp.FrontEnd;

static class PhiFixup
{
    public static void FixupMerges(BaseOp[] ops)
    {
        var indicesGotos = VregGotos(ops);
        foreach (var index in indicesGotos)
        {
            var gotoAssign = (AssignOp)ops[index - 1];
            var targetGoto = (GotoOp)ops[index];
            int indexLabel = ops.IndexOfOp<LabelOp>(label => label.Offset == targetGoto.Offset);
            if (indexLabel == -1)
            {
                throw new Exception("Expected label not found");
            }

            var labelAssign = (AssignOp)ops[indexLabel - 1];
            var afterLabelAssign = (AssignOp)ops[indexLabel + 1];
            var sourceVreg = (VReg)gotoAssign.Left;
            labelAssign.Left = sourceVreg;
            afterLabelAssign.Expression = sourceVreg;
        }
    }

    private static int[] VregGotos(BaseOp[] ops)
    {
        var indices = new List<int>();
        for (var index = 1; index < ops.Length; index++)
        {
            var op = ops[index];
            var prevOp = ops[index - 1];

            if (op is GotoOp gotoOp && prevOp is AssignOp assignOp && assignOp.Left is VReg)
            {
                indices.Add(index);
            }
        }

        return indices.ToArray();
    }
}