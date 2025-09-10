using NativeSharp.Operations;
using NativeSharp.Operations.Common;
using NativeSharp.Operations.Values;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Optimizations.BlockOptimizations;

class BlockBasedPropagation : BlockBasedOptimization
{
    public override bool OptimizeSegment(ArraySegment<BaseOp> segment)
    {
        Dictionary<IValueExpression, IValueExpression> updates = [];

        var result = false;
        for (var i = 0; i < segment.Count; i++)
        {
            var op = segment[i];
            result |= UpdateKnownOps(op, updates);
            if (op is not AssignOp assignOp)
            {
                updates.Clear();
                continue;
            }


            if (assignOp.Expression is ConstantValueExpression || assignOp.Expression is IndexedVariable)
            {
                updates[assignOp.Left] = assignOp.Expression;
            }
        }

        return result;
    }

    public bool UpdateKnownOps(BaseOp op, Dictionary<IValueExpression, IValueExpression> updates)
    {
        return op switch
        {
            AssignOp assignOp => UpdateExpressionT(assignOp, MatchAssign, updates),
            CallReturnOp callReturnOp => UpdateExpressionT(callReturnOp, MatchCallReturn, updates),
            CallOp callOp => UpdateExpressionT(callOp, MatchCall, updates),
            BranchOp branchOperation => UpdateExpressionT(branchOperation, MatchBranchOp, updates),
            BinaryOp binaryOp => UpdateExpressionT(binaryOp, MatchBinaryOp, updates),
            _ => false
        };
    }

    private bool MatchBinaryOp(BinaryOp op, IValueExpression from, IValueExpression to)
    {
        var target = UpdateTargetExpression(op.LeftExpression, from, to);
        op.LeftExpression = target.Mapped;

        var target2 = UpdateTargetExpression(op.RightExpression, from, to);
        op.RightExpression = target2.Mapped;
        return target.Changed || target2.Changed;
    }

    private bool MatchBranchOp(BranchOp op, IValueExpression from, IValueExpression to)
    {
        var target = UpdateTargetExpression(op.Condition, from, to);
        op.Condition = target.Mapped;
        return target.Changed;
    }

    private static bool MatchAssign(AssignOp op, IValueExpression from, IValueExpression to)
    {
        var target = UpdateTargetExpression(op.Expression, from, to);
        op.Expression = target.Mapped;
        return target.Changed;
    }

    private static bool MatchCallReturn(CallReturnOp op,
        IValueExpression from, IValueExpression to)
    {
        return UpdateUsagesArguments(from, to, op.Args);
    }

    private static bool MatchCall(CallOp op,
        IValueExpression from, IValueExpression to)
    {
        var parameters = op.Args;
        return UpdateUsagesArguments(from, to, op.Args);
    }

    private static bool UpdateUsagesArguments(IValueExpression from, IValueExpression to, IValueExpression[] parameters)
    {
        var result = false;
        for (var index = 0; index < parameters.Length; index++)
        {
            var arg = parameters[index];
            var target = UpdateTargetExpression(arg, from, to);
            parameters[index] = target.Mapped;
            result |= target.Changed;
        }

        return result;
    }

    private static bool UpdateExpressionT<T>(T op, Func<T, IValueExpression, IValueExpression, bool> match,
        Dictionary<IValueExpression, IValueExpression> updates) where T : BaseOp
    {
        var result = false;
        foreach (var kv in updates)
        {
            result |= match(op, kv.Key, kv.Value);
        }

        return result;
    }

    static (IValueExpression Mapped, bool Changed) UpdateTargetExpression(IValueExpression targetExpression,
        IValueExpression compareTo,
        IValueExpression replaceWith)
    {
        if (targetExpression == compareTo)
        {
            return (replaceWith, true);
        }

        return (targetExpression, false);
    }
}