using CommunityToolkit.Mvvm.ComponentModel;
using TrarsUI.Shared.DTOs;
using TrarsUI.Shared.Interfaces.UIComponents;
using TreePrompt2Json.PromptBuilder.MVVM.Helpers;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
    partial class PromptViewerVM : ObservableObject, IContentVM
    {
        [ObservableProperty]
        private string title = "PromptViewer";

        [ObservableProperty]
        private PromptString finalOutput;

        public PromptViewerVM(Manager manager)
        {

        }

        public PromptViewerVM()
        {

        }
    }
}
