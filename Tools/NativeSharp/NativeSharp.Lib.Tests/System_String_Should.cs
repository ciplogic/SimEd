using NativeSharp.Common;
using Shouldly;

namespace NativeSharp.Lib.Tests;

public class System_String_Should
{
    
    [Fact]
    public void Concat()
    {
        var text1 = "Hello ".ToSystemString();
        var text2 = "World".ToSystemString();
        var text3 = ResolvedMethods.System_String_Concat(text1, text2);
        string decoded = text3.ToClrString();
        decoded.ShouldBe("Hello World");
        
    }
}