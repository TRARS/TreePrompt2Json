using System.Diagnostics;
using System.Windows;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.SourceGenerator.Attributes;

namespace TreePrompt2Json.MVVM.Views
{
    [UseChrome]
    public partial class MainWindow : Window, IMainWindow
    {
        ITokenProviderService _tokenProvider;
        IDebouncerService _debouncer;
        IStringEncryptorService _stringEncryptor;

        public MainWindow(ITokenProviderService tokenProvider, IDebouncerService debouncer, IStringEncryptorService stringEncryptor)
        {
            _tokenProvider = tokenProvider;
            _debouncer = debouncer;
            _stringEncryptor = stringEncryptor;

            InitializeComponent();
            InitWindowBorderlessBehavior(); // 无边框
            InitWindowMessageWithToken(new WindowMessageConfig()
            {
                UseCloseIntercept = true,
                OnCloseIntercept = (x) =>
                {
                    Debug.WriteLine($"OnCloseIntercept YesNo: {x}");
                },
            }); // 注册消息
        }
    }
}