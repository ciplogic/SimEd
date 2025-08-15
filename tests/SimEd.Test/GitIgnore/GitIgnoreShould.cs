using Shouldly;
using SimEd.ViewModels.Solution;

namespace SimEd.Test.GitIgnore;

public class GitIgnoreShould
{
    [Theory]
    [InlineData("/codegen/common/data.cob", "/*.c")]
    public void NotMatch(string path, string pattern)
    {
        var glob = new GlobFilter(pattern);
        glob.Matches(path).ShouldBeFalse();
    }
}