using TrarsUI.Shared.DTOs;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
    partial class PromptEditorVM
    {
        /// <summary>
        /// 创建模板
        /// </summary>
        private partial void CreateTemplate(bool autoSelect = true);

        //private partial PromptString CreateStart();
        private partial ToggleTreeViewNode CreateSystemRule();
        private partial ToggleTreeViewNode CreateCharacterA();
        private partial ToggleTreeViewNode CreateCharacterB();
        private partial ToggleTreeViewNode CreateCharacterC();
        private partial ToggleTreeViewNode CreateCharacterD();
        private partial ToggleTreeViewNode CreateCharacterE();
        private partial ToggleTreeViewNode CreateStory();
        private partial ToggleTreeViewNode CreatePlot();

        private partial PromptString CreateOutputFormat();
        private partial PromptString CreateEnd();

        private partial PromptString CreateContinuePrompt();
    }
}
