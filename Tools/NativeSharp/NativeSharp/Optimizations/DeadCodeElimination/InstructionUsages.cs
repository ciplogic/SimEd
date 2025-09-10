using NativeSharp.Operations;
using NativeSharp.Operations.FieldsAndIndexing;
using NativeSharp.Operations.Vars;

namespace NativeSharp.Optimizations.DeadCodeElimination;

static class InstructionUsages
{
    public static IEnumerable<string> GetUsagesOf(BaseOp instruction)
    {
        return instruction switch
        {
            AssignOp assignOp => UsagesOfAssign(assignOp),
            CallOp callOp => callOp.Args.Select(arg => arg.Code()),
            CallReturnOp callReturnOp => callReturnOp.Args.Select(arg => arg.Code()),
            StoreElementOp storeElementOp => [storeElementOp.ArrPtr.Code(), storeElementOp.Index.Code(), storeElementOp.ValueToSet.Code()],
            LoadElementOp loadElementOp => [loadElementOp.Array.Code(), loadElementOp.Index.Code()],
            StoreFieldOp storeFieldOp => [storeFieldOp.ThisPtr.Code(), storeFieldOp.ValueToSet.Code()],
            LoadFieldOp loadFieldOp => [loadFieldOp.ThisPtr.Code()],
            NewArrayOp newArray => [newArray.Count.Code()],
            BranchOp branchOperation => [branchOperation.Condition.Code()],
            BinaryOp binaryOp => [binaryOp.LeftExpression.Code(), binaryOp.RightExpression.Code()],
            RetOp retOp => [retOp.ValueExpression?.Code() ?? "",],
            _ => []
        };
    }

    private static IEnumerable<string> UsagesOfAssign(AssignOp assignOp)
    {
        if (assignOp.Expression is IndexedVariable)
        {
            return [assignOp.Expression.Code()];
        }

        return [];
    }
}