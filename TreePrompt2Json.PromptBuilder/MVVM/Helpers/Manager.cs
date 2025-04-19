using CommunityToolkit.Mvvm.Messaging;
using TrarsUI.Shared.DTOs;
using TrarsUI.Shared.Helpers.Extensions;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Messages;
using TreePrompt2Json.PromptBuilder.MVVM.ViewModels;

namespace TreePrompt2Json.PromptBuilder.MVVM.Helpers
{
    internal sealed partial class Manager
    {
        readonly Dictionary<string, string> _windows = new();
        readonly List<string> _tokens = new();

        readonly IAbstractFactory<PromptViewerVM> _promptViewerVMFactory;

        public Manager(IAbstractFactory<PromptViewerVM> promptViewerVMFactory)
        {
            _promptViewerVMFactory = promptViewerVMFactory;
        }

        public async Task OpenDialog(string msg, string token)
        {
            Action? yesnoCallback = null;
            var yesno = await WeakReferenceMessenger.Default.Send(new DialogYesNoMessage(msg, (x) => { yesnoCallback = x; }), token);
            yesnoCallback?.Invoke();
        }

        public async Task OpenPromptViewer(PromptString finalOutput)
        {
            await this.OpenWith<PromptViewerVM>(async () =>
            {
                var token = await WeakReferenceMessenger.Default.Send(new OpenChildFormMessage(new()
                {
                    ViewModel = _promptViewerVMFactory.Create().Init(x => { x.FinalOutput = finalOutput; }),
                    WindowInfo = new WindowInfo()
                    {
                        Width = 640,
                        Height = 480,
                        MinWidth = 640,
                        MinHeight = 480,
                        MaxWidth = 1920,
                        MaxHeight = 1080,
                        SizeToContent = System.Windows.SizeToContent.Manual,
                        ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip
                    }
                }));
                return token;
            });
        }
    }

    internal sealed partial class Manager
    {
        private async Task OpenWith<T>(Func<Task<string>> action)
        {
            var type = typeof(T).Name;
            if (this.Begin(type)) { return; }
            var token = await action.Invoke();
            this.After(type, token);
        }

        private bool Begin(string type)
        {
            // 限制打开数量
            if (_windows.ContainsKey(type))
            {
                this.ShowChildForm(_windows[type]); return true;
            }

            return false;
        }

        private void After(string type, string token)
        {
            // VM宿主关闭时反注册
            WeakReferenceMessenger.Default.Register<WindowClosingMessage, string>(this, token, (r, m) =>
            {
                _windows.Remove(type);
                WeakReferenceMessenger.Default.Unregister<WindowClosingMessage, string>(this, token);
            });

            // 内部维护
            _tokens.Add(token);
            _windows.TryAdd(type, token);
        }
    }

    internal sealed partial class Manager
    {
        public void ShowAllChildForm()
        {
            _tokens.ForEach(token =>
            {
                WeakReferenceMessenger.Default.Send(new WindowNormalizeMessage("ShowAllChildForm"), token);
            });
        }

        public void CloseAllChildForm()
        {
            _tokens.ForEach(token =>
            {
                WeakReferenceMessenger.Default.Send(new WindowCloseMessage("CloseAllChildForm"), token);
            });
            _tokens.Clear();
        }

        public void ShowChildForm(string token)
        {
            WeakReferenceMessenger.Default.Send(new WindowNormalizeMessage("ShowChildForm"), token);
        }

        public void CloseChildForm(string token)
        {
            WeakReferenceMessenger.Default.Send(new WindowCloseMessage("CloseAllChildForm"), token);
            _tokens.Remove(token);
        }
    }
}
