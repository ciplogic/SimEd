using Avalonia.Controls.Templates;
using Jab;
using SimEd.Common.Interfaces;
using SimEd.Common.Mediator;
using SimEd.IoC.Tools;
using SimEd.Models;
using SimEd.Models.FileChoosers;
using SimEd.Models.Languages;
using SimEd.Models.Languages.CsharpLang;
using SimEd.ViewModels;
using SimEd.ViewModels.Documents;
using SimEd.ViewModels.Settings;
using SimEd.ViewModels.Search;
using SimEd.ViewModels.Solution;

namespace SimEd.IoC;

[ServiceProvider]
[Singleton<IMiniPubSub, MiniPubSub>]
[Singleton<IDataTemplate, ViewLocator>]
[Singleton(typeof(NotepadFactory))]
[Singleton<IInjector, InjectorHost>]
[Singleton<IAppSettingsReader, AppSettingsReader>]
[Singleton(typeof(SolutionViewModel))]
[Singleton(typeof(MainWindowViewModel))]
[Singleton<IFileDialogChooser, FileDialogChooser>]
[Singleton<SolutionLanguageExtractors>]
//[Singleton<IFileDialogChooser, ClassicFileDialogChooser>]
[Transient<FileViewModel>]
[Singleton(typeof(OptionsDialogViewModel))]
[Transient<ShowGenericFinderWindowViewModel>]
public partial class ServiceProvider;