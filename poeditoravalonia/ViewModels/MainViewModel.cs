using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace poeditoravalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string termSearch = "";
    private LanguagesListLong_languages? currentLanguage;
    private ProjectList_projects? currentProject;

    public string ApiKey { get; set; } = "";
    public string TermSearch
    {
        get => termSearch; 
        set
        {
            termSearch = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(Terms));
        }
    }
    public ProjectList_projects[] Projects { get; set; } = Array.Empty<ProjectList_projects>();
    public Task<LanguagesListLong_languages[]> Languages => LoadProjectLanguages();

    public ProjectList_projects? CurrentProject
    {
        get => currentProject;
        set
        {
            currentProject = value;
            this.RaisePropertyChanged(nameof(CurrentProject));
            this.RaisePropertyChanged(nameof(Languages));
            this.RaisePropertyChanged(nameof(Terms));
        }
    }

    public LanguagesListLong_languages? CurrentLanguage
    {
        get => currentLanguage;
        set
        {
            currentLanguage = value;
            this.RaisePropertyChanged(nameof(CurrentLanguage));
            this.RaisePropertyChanged(nameof(Terms));
        }
    }
    public Task<TermsListFull_terms[]> Terms => LoadTerms();

    public ReactiveCommand<Unit, Unit> LoadKeysCommand { get; }

    public MainViewModel()
    {
        LoadKeysCommand = ReactiveCommand.CreateFromTask(LoadKeys);
    }

    private async Task LoadKeys()
    {
        var client = new POEditorClient();
        var projects = await client.ProjectsListAsync(new Authentication()
        {
            Api_token = ApiKey,
        });
        this.Projects = projects.Result.Projects.ToArray();
        this.RaisePropertyChanged(nameof(Projects));
    }

    private async Task<LanguagesListLong_languages[]> LoadProjectLanguages()
    {
        var currentProject = CurrentProject;
        if (currentProject is null)
        {
            return [];
        }

        var client = new POEditorClient();
        var projects = await client.LanguagesListAsync(new Languages_list_body
        {
            Api_token = ApiKey,
            Id = currentProject.Id,
        });
        return projects.Result.Languages.ToArray();
    }

    private async Task<TermsListFull_terms[]> LoadTerms()
    {
        var currentProject = CurrentProject;
        if (currentProject is null)
        {
            return [];
        }

        var currentLanguage = CurrentLanguage;
        if (currentLanguage is null)
        {
            return [];
        }

        var client = new POEditorClient();
        var projects = await client.TermsListAsync(new ()
        {
            Api_token = ApiKey,
            Id = currentProject.Id,
            Language = currentLanguage.Code,
        });
        return projects.Result.Terms.Where(_ => string.IsNullOrWhiteSpace(TermSearch) || _.Term.Contains(TermSearch)).ToArray();
    }
}
