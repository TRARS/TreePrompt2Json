using CommunityToolkit.Mvvm.ComponentModel;

namespace TreePrompt2Json.PromptBuilder.MVVM.Helpers
{
    internal partial class PromptEditorSettings : ObservableObject
    {
        [ObservableProperty]
        private string defuaultAesIv = string.Empty;

        [ObservableProperty]
        private string defuaultAesKey = string.Empty;

        [ObservableProperty]
        private string localFolder = string.Empty;
    }
}
