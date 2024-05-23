using ReactiveUI;
using System.Reactive;

namespace poeditoravalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting { get; set; } = "Welcome to Avalonia!";
    public string ProjectId { get; set; } = "";
    public string ApiKey { get; set; } = "";

    public ReactiveCommand<Unit, Unit> LoadKeysCommand { get; }

    public MainViewModel()
    {
        LoadKeysCommand = ReactiveCommand.Create(LoadKeys);
    }

    private void LoadKeys()
    {
    }
}
