using SimEd.ViewModels.Documents;

namespace SimEd;

public class HelpAboutWindowViewModel : BaseFileViewModel
{
    public string VersionValue => GetVersionString();

    private string GetVersionString()
    {
        try
        {
            var assembly = typeof(HelpAboutWindowViewModel).Assembly;

            var version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
        catch
        {
            return "0.0.0-UNKNOWN";
        }
    }
}