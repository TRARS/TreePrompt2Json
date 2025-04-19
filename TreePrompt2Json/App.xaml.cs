using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using TrarsUI.Shared.Helpers.Extensions;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Services;
using TreePrompt2Json.MVVM.ViewModels;
using TreePrompt2Json.MVVM.Views;

namespace TreePrompt2Json
{
    public partial class App : Application
    {
        private static IHost AppHost { get; set; } = GetHostBuilder().Build();

        public App()
        {
            //使SelectionTextBrush生效
            AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();
            AppHost.Services.GetRequiredService<IAbstractFactory<IMainWindow>>().Create().Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }

        private static IHostBuilder GetHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                       .ConfigureServices(sc =>
                       {
                           // Entry
                           TreePrompt2Json.PromptBuilder.EntryService.Register(sc);

                           // Service
                           sc.AddSingleton<ICreateChildFormService, CreateChildFormService>();
                           sc.AddTransient<IDebouncerService, DebouncerService>();
                           sc.AddTransient<IDialogYesNoService, DialogYesNoService>();
                           sc.AddTransient<IDispatcherService, DispatcherService>();
                           sc.AddSingleton<IJsonConfigManagerService, JsonConfigManagerService>();
                           sc.AddSingleton<IMessageBoxService, MessageBoxService>();
                           sc.AddTransient<IStringEncryptorService, StringEncryptorService>();
                           sc.AddScoped<ITokenProviderService, TokenProviderService>();
                           sc.AddSingleton<IContentProviderService, TreePrompt2Json.PromptBuilder.EntryService>();

                           // UI组件VM
                           sc.AddFormFactory<IuTitleBarVM, uTitleBarVM>();
                           sc.AddFormFactory<IuRainbowLineVM, uRainbowLineVM>();
                           sc.AddFormFactory<IuClientVM, uClientVM>();

                           // MainWindow
                           sc.AddFormFactory<IMainWindow, IMainWindowEmpty, MainWindow>(sp =>
                           {
                               using (var scope = sp.CreateScope())
                               {
                                   var mainwindow = (MainWindow)(scope.ServiceProvider.GetRequiredService<IMainWindowEmpty>());
                                   {
                                       mainwindow.DataContext = scope.ServiceProvider.GetRequiredService<IMainWindowVM>();
                                       mainwindow.SizeToContent = SizeToContent.WidthAndHeight;
                                       mainwindow.Width = 720;
                                       mainwindow.Height = 540;
                                       mainwindow.MinWidth = 720;
                                       mainwindow.MinHeight = 540;
                                   }
                                   return mainwindow;
                               }
                           });
                           // MainWindowVM
                           sc.AddTransient<IMainWindowVM, MainWindowVM>();

                           // ChildForm
                           sc.AddFormFactory<IChildForm, IChildFormEmpty, ChildForm>(sp =>
                           {
                               using (var scope = sp.CreateScope())
                               {
                                   var childForm = (ChildForm)scope.ServiceProvider.GetRequiredService<IChildFormEmpty>();
                                   {
                                       childForm.DataContext = scope.ServiceProvider.GetRequiredService<IChildFormVM>();
                                       childForm.SizeToContent = SizeToContent.WidthAndHeight;
                                   }
                                   return childForm;
                               }
                           });
                           // ChildFormVM
                           sc.AddTransient<IChildFormVM, ChildFormVM>();
                       });
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            return AppHost.Services.GetRequiredService<T>();
        }
    }
}
