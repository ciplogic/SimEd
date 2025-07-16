using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using SimEd.Common.Interfaces;
using SimEd.Common.Mediator;
using SimEd.Events;
using SimEd.Models.Languages;
using SimEd.Models.Languages.Common;
using SimEd.Models.Languages.Lexer;
using SimEd.ViewModels.Solution;

namespace SimEd.ViewModels.Search;

public class ShowGenericFinderWindowViewModel : ObservableObject
{
    private readonly SolutionLanguageExtractors _extractions;
    private readonly IMiniPubSub _miniPubSub;
    private string _typesText = string.Empty;
    private int _selectedIndex;

    public ShowGenericFinderWindowViewModel(
        SolutionViewModel solution, 
        SolutionLanguageExtractors extractions,
        IMiniPubSub miniPubSub)
    {
        _extractions = extractions;
        _miniPubSub = miniPubSub;
        BuildIndexTask = BuildIndex(solution).GetAwaiter().GetResult();
        UpdateFilter();
    }

    public SolutionIndex BuildIndexTask { get; set; }

    public string TypesText
    {
        get => _typesText;
        set
        {
            if (!SetProperty(ref _typesText, value))
            {
                return;
            }

            UpdateFilter();
            SelectedIndex = 0;
        }
    }

    private void UpdateFilter()
    {
        SolutionIndexItem[] values =
            BuildIndexTask
                .Items
                .Where(it => it.Token.AText.IsSmartMatch(TypesText.ToLower()))
                .ToArray();
        FoundTypes.Clear();
        foreach (SolutionIndexItem value in values)
        {
            FoundTypes.Add(new FindItemViewModel()
            {
                SolutionItem = value
            });
        }
    }

    public ObservableCollection<FindItemViewModel> FoundTypes { get; } = [];

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetProperty(ref _selectedIndex, value);
    }

    public void OnChosenItem()
    {
        if (SelectedIndex == -1)
        {
            return;
        }

        FindItemViewModel index = FoundTypes[SelectedIndex];
        
        _miniPubSub.Command<OpenFileFromAnywhere>(new (index.SolutionItem.FileName.Path, index.SolutionItem.Token.Position));
    }

    private async Task<SolutionIndex> BuildIndex(SolutionViewModel solution)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Task<SolutionIndexItem[]>[] tasks = await IndexAllFilesForDeclarationsTasks(solution)
            .ConfigureAwait(false);

        SolutionIndex result = new();
        foreach (Task<SolutionIndexItem[]> solutionItem in tasks)
        {
            result.Items.AddRange(solutionItem.Result);
        }

        sw.Stop();

        Console.WriteLine(sw.Elapsed);

        return result;
    }

    private async Task<Task<SolutionIndexItem[]>[]> IndexAllFilesForDeclarationsTasks(SolutionViewModel solution,
        bool processInParallel = true)
    {
        SolutionItem[] files = solution.Nodes.Leafs(it => it.Children).ToArray();
        List<Task<SolutionIndexItem[]>> tasks = [];
        tasks.AddRange(files.Select(solutionItem => _extractions.Parse(solutionItem)));
        if (processInParallel)
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        else
        {
            foreach (Task<SolutionIndexItem[]> task in tasks)
            {
                await task.ConfigureAwait(false);
            }
        }

        return tasks.ToArray();
    }
}