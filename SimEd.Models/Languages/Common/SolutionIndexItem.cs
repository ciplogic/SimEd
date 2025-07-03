using SimEd.Models.Languages.Lexer;

namespace SimEd.Models.Languages.Common;

public record struct SolutionIndexItem(Token Token, SolutionItem FileName, string Kind);