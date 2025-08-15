namespace SimEd.ViewModels.Solution;

public readonly struct GlobFilter
{
    private readonly string[] _operations = [];
    private bool ContainsDirectory => _operations[0][0] == '/';

    public GlobFilter(string pattern)
    {
        _operations = pattern.BuildOperations();
    }

    public override string ToString() => string.Join("", _operations);

    public bool Matches(string path)
    {
        if (ContainsDirectory)
        {
            return MatchOp(path);
        }

        var fileInfo = new FileInfo(path);
        return MatchOp(fileInfo.Name);
    }

    private bool MatchOp(string fileInfoName)
    {
        if (_operations.Length == 1)
        {
            return _operations[0] == fileInfoName;
        }

        return MatchOpRecursive(fileInfoName, _operations);
    }

    private bool MatchOpRecursive(string fileInfoName, ReadOnlySpan<string> operations)
    {
        if (operations.Length == 0)
        {
            return true;
        }

        var operationStep = _operations[0];
        if (operationStep != "*")
        {
            return fileInfoName.StartsWith(operationStep) &&
                   MatchOpRecursive(fileInfoName[operationStep.Length..], operations[1..]);
        }

        if (operations.Length == 1)
        {
            //it means that it is just a remaining "*"
            return true;
        }

        if (operations.Length == 2)
        {
            return fileInfoName.EndsWith(operations[1]);
        }

        var midText = operations[1];
        var indexOfMidText= fileInfoName.IndexOf(midText);
        if (indexOfMidText == -1)
        {
            return false;
        }
        
        return MatchOpRecursive(fileInfoName[(indexOfMidText + midText.Length)..], operations[2..]);

    }
}

static class GlobFilterExtensions
{
    public static string[] BuildOperations(this string pattern)
    {
        var result = new List<string>();

        var defaultSplit = ExtractOperationsBySeparator(pattern, "*");
        foreach (var op in defaultSplit)
        {
            var opsSplitBySlash = ExtractOperationsBySeparator(op, "/");
            result.AddRange(opsSplitBySlash);
        }


        return result.ToArray();
    }

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