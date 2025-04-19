using System.Collections.ObjectModel;
using TrarsUI.Shared.Interfaces.UIComponents;

namespace TreePrompt2Json.MVVM.DesignTimeViewModels
{
    internal class MainWindowVM : IMainWindowVM
    {
        public ObservableCollection<IToken> SubViewModelList { get; init; } = new()
        {
            App.GetRequiredService<IuTitleBarVM>(),
            App.GetRequiredService<IuRainbowLineVM>(),
            App.GetRequiredService<IuClientVM>(),
        };
    }
}
