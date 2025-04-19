namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
#if PUBLIC_BUILD
    partial class PromptEditorVM
    {
        private partial void InitConfig()
        {
            // 设置bool默认值
            this.FullMode = this.UseAes = this.UseJsonl = this.UseSeparator = false;

            // 从本地配置中加载
            OnLoadConfig();
        }
    }
#endif
}
