namespace SimEd.ViewModels.Solution;

class GlobFilter
{
    string[] _operations = [];
    public bool ContainsDirectory { get; private set; } = false;

    public GlobFilter(string pattern)
    {
        ContainsDirectory = pattern.Contains('/');
        _operations = BuildOperations(pattern);
    }
    
    public override string ToString() => string.Join("", _operations);

    public bool Matches(string path)
    {
        if (ContainsDirectory)
        {
            return MatchOp(path, 0);
        }
        else
        {
            var fileInfo = new FileInfo(path);
            return MatchOp(fileInfo.Name, 0);
        }
    }

    private bool MatchOp(string fileInfoName, int step)
    {
        if (_operations.Length == 1)
        {
            return _operations[0] == fileInfoName;
        }

        if (step >= _operations.Length)
        {
            return true;
        }

        var operationStep = _operations[step];
        if (operationStep != "*")
        {
            return fileInfoName.StartsWith(operationStep) && MatchOp(fileInfoName[operationStep.Length..], step + 1);
        }

        if (step == _operations.Length - 1)
        {
            return true;
        }

        var nextItemToSearch = _operations[step + 1];
        var indexNext = fileInfoName.IndexOf(nextItemToSearch);
        if (indexNext == -1)
        {
            return false;
        }

        return MatchOp(fileInfoName.Substring(indexNext), step + 1);
    }

    private static string[] BuildOperations(string pattern)
    {
        var remainder = pattern;
        List<string> operations = [];
        do
        {
            var index = remainder.IndexOf('*');
            if (index == -1)
            {
                AddOp(remainder);
                return operations.ToArray();
            }

            var prefix = remainder.Substring(0, index);
            AddOp(prefix);
            AddOp("*");
            remainder = remainder.Substring(index + 1);
        } while (remainder.Length > 0);

        return operations.ToArray();

        void AddOp(string op)
        {
            if (!string.IsNullOrWhiteSpace(op))
            {
                operations.Add(op);
            }
        }
    }
}