﻿using System.Windows;
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
            enableShadowLayer = false;

            _tokenProvider = tokenProvider;
            _debouncer = debouncer;
            _stringEncryptor = stringEncryptor;

            InitializeComponent();
            InitWindowBorderlessBehavior(); // 无边框
            InitWindowMessageWithToken(); // 注册消息
        }
    }
}