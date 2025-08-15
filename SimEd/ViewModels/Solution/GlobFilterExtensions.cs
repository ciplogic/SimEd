namespace SimEd.ViewModels.Solution;

internal static class GlobFilterExtensions
{
    public static string[] BuildOperations(this string pattern)
        => ExtractOperationsBySeparator(pattern, "*");

    private static string[] ExtractOperationsBySeparator(string pattern, string separator)
    {
        var remainder = pattern;
        List<string> operations = [];
        do
        {
            var index = remainder.IndexOf(separator);
            if (index == -1)
            {
                AddOp(operations, remainder);
                return operations.ToArray();
            }

            var prefix = remainder[..index];
            AddOp(operations, prefix);
            AddOp(operations, separator);
            remainder = remainder[(index + 1)..];
        } while (remainder.Length > 0);

        return operations.ToArray();
    }

    private static void AddOp(List<string> operations, string op)
    {
        if (!string.IsNullOrWhiteSpace(op))
        {
            operations.Add(op);
        }
    }
}