using Microsoft.Extensions.DependencyInjection;
using TrarsUI.Shared.Helpers.Extensions;
using TrarsUI.Shared.Interfaces.UIComponents;
using TreePrompt2Json.Format.Interfaces;
using TreePrompt2Json.Format.Services;
using TreePrompt2Json.PromptBuilder.MVVM.Helpers;
using TreePrompt2Json.PromptBuilder.MVVM.ViewModels;

namespace TreePrompt2Json.PromptBuilder
{
    public class EntryService : IContentProviderService
    {
        private readonly IServiceProvider _serviceProvider;

        public EntryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IContentVM Create()
        {
            return _serviceProvider.GetRequiredService<PromptEditorVM>();
        }

        public static void Register(IServiceCollection services)
        {
            services.AddFormFactory<PromptEditorVM>();
            services.AddFormFactory<PromptViewerVM>();
            services.AddSingleton<Manager>();
            services.AddSingleton<IFirstChapterBuilderService, FirstChapterBuilderService>();
        }
    }
}