using Shouldly;
using SimEd.ViewModels.Search;

namespace SimEd.Test.SmartMatch;

public class SmartMatchShould
{
    [Theory]
    [InlineData("TaskEnumerator", "ted", false)]
    public void NotMatchSimpleCases(string input, string filter, bool isMatching)
    {
        var actualMatch = MatchExtensions.IsSmartMatch(input, filter);
        actualMatch.ShouldBe(isMatching);
    }

    [Theory]
    [InlineData("Allocator32", "all32", true)]
    [InlineData("rename_files_with_namespaces", "ref", true)]
    public void MatchSimpleCases(string input, string filter, bool isMatching)
    {
        var actualMatch = input.IsSmartMatch(filter);
        actualMatch.ShouldBe(isMatching);
    }
}