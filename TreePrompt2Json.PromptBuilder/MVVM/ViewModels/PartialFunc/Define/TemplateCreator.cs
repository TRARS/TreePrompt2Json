using TrarsUI.Shared.DTOs;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
    partial class PromptEditorVM
    {
        /// <summary>
        /// 创建模板
        /// </summary>
        private partial void CreateTemplate(bool autoSelect = true);

        private partial ToggleTreeViewNode CreateSystemRule();
        private partial ToggleTreeViewNode CreateStory();
        private partial ToggleTreeViewNode CreatePlot();
        private partial PromptString CreateContinuePrompt();
    }
}
