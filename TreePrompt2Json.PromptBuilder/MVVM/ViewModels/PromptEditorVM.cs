using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using TrarsUI.Shared.Collections;
using TrarsUI.Shared.DTOs;
using TrarsUI.Shared.Interfaces;
using TrarsUI.Shared.Interfaces.UIComponents;
using TrarsUI.Shared.Interfaces.UIControls;
using TrarsUI.Shared.Messages;
using TreePrompt2Json.PromptBuilder.MVVM.Helpers;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
    partial class PromptEditorVM : ObservableObject, IContentVM
    {
        [ObservableProperty]
        private bool fullMode;

        [ObservableProperty]
        private string title = "PromptEditor";

        private string Token { get; set; } = string.Empty; // 宿主Token
        private string ConfigName { get; set; } = "PromptEditorSettings.json"; // 配置文件名

        [ObservableProperty]
        private ObservableCollection<IPromptPacket> promptPacketList = new();
        [ObservableProperty]
        private ObservableCollection<IPromptPacket> promptPacketList2 = new();

        [ObservableProperty]
        private PromptPacket? selectedPromptPacket;

        [ObservableProperty]
        private PromptString finalOutput = new();

        public AlertMessageObservableCollection SystemMessages { get; set; } = new(3, 1);

        [ObservableProperty]
        private string localFolder;

        [ObservableProperty]
        private string defuaultAesIv;

        [ObservableProperty]
        private string defuaultAesKey;

        [ObservableProperty]
        private bool useAes;

        [ObservableProperty]
        private bool useJsonl;

        [ObservableProperty]
        private bool useSeparator;

        private IDispatcherService _dispatcher;
        private IJsonConfigManagerService _jsonConfigService;
        private Manager _manager;
        private ITokenProviderService _tokenProviderService;
        private IStringEncryptorService _stringEncryptorService;

        private PromptEditorSettings _config = new();

        private const string TextIcon = @"M909.3 269.3L658.8 18.8C646.7 6.7 630.5 0 613.5 0H144c-26.5 0-48 21.5-48 48v264c0 4.4-3.6 8-8 8H16c-8.8 0-16 7.2-16 16v544c0 8.8 7.2 16 16 16h80c0 35.3 14.3 67.3 37.5 90.5 23.2 23.2 55.2 37.5 90.5 37.5h656c26.5 0 48-21.5 48-48V314.5c0-17-6.7-33.2-18.7-45.2zM704 154.5l69.5 69.5H708c-2.2 0-4-1.8-4-4v-65.5zM188.5 527.1v-46.8h210.3v46.8h-76.6v221.5h-57.3V527.1h-76.4zM864 952c0 4.4-3.6 8-8 8H224c-35.3 0-64-28.7-64-64h400c8.8 0 16-7.2 16-16V336c0-8.8-7.2-16-16-16H168c-4.4 0-8-3.6-8-8V72c0-4.4 3.6-8 8-8h408c35.3 0 64 28.6 64 64v144c0 8.8 7.2 16 16 16h144c35.3 0 64 28.6 64 64v600z
                                          M796 512H628c-2.2 0-4-1.8-4-4v-56c0-2.2 1.8-4 4-4h168c2.2 0 4 1.8 4 4v56c0 2.2-1.8 4-4 4zM796 640H628c-2.2 0-4-1.8-4-4v-56c0-2.2 1.8-4 4-4h168c2.2 0 4 1.8 4 4v56c0 2.2-1.8 4-4 4zM796 768H628c-2.2 0-4-1.8-4-4v-56c0-2.2 1.8-4 4-4h168c2.2 0 4 1.8 4 4v56c0 2.2-1.8 4-4 4z";
        private const string JsonIcon = @"M1008 464h-72c-4.4 0-8-3.6-8-8V314.5c0-17-6.7-33.3-18.7-45.3L658.7 18.7C646.7 6.7 630.5 0 613.5 0H144c-26.5 0-48 21.5-48 48v408c0 4.4-3.6 8-8 8H16c-8.8 0-16 7.2-16 16v400c0 8.8 7.2 16 16 16h80c0 35.3 14.3 67.3 37.5 90.5 23.2 23.2 55.2 37.5 90.5 37.5h656c26.5 0 48-21.5 48-48v-64c0-8.8 7.2-16 16-16h64c8.8 0 16-7.2 16-16V480c0-8.8-7.2-16-16-16zM704 154.5l69.5 69.5H708c-2.2 0-4-1.8-4-4v-65.5zM160 72c0-4.4 3.6-8 8-8h408c35.3 0 64 28.7 64 64v144c0 8.8 7.2 16 16 16h144c35.3 0 64 28.7 64 64v104c0 4.4-3.6 8-8 8H168c-4.4 0-8-3.6-8-8V72z m492.3 603.5c0 32.5-9.5 58.9-28.4 79.1-18.9 20.3-43.7 30.4-74.3 30.4-29.9 0-54.2-9.8-72.9-29.4-18.5-19.6-27.9-44.9-27.9-75.8 0-32.7 9.5-59.3 28.6-79.8s44.3-30.8 75.6-30.8c29.8 0 53.8 9.9 72 29.8 18.2 19.8 27.3 45.4 27.3 76.5z m-365.9 53.3c16.9 13.9 35.9 20.9 57.3 20.9 12.1 0 21.2-2.1 27.3-6.3 6.1-4.2 9.2-9.5 9.2-16.1 0-5.7-2.4-11-7.3-16s-17.6-11.9-38.4-20.5c-32.6-13.8-48.9-33.9-48.9-60.4 0-19.4 7.4-34.5 22.2-45.2s34.3-16.1 58.7-16.1c20.4 0 37.5 2.7 51.3 7.9v41.8c-14-9.5-30.4-14.3-49.2-14.3-11 0-19.7 2-26.3 6-6.6 4-9.8 9.4-9.8 16.1 0 5.4 2.2 10.4 6.7 14.9s15.6 10.7 33.3 18.4c20.7 8.9 35 18.3 42.8 28.2 7.8 9.9 11.7 21.6 11.7 35.3 0 20-7.1 35.3-21.3 45.8S371.3 785 345.2 785c-23.9 0-43.5-3.9-58.7-11.6l-0.1-44.6c0.1 0 0 0 0 0z m-139.6 8.9c7.7 5.8 16.5 8.8 26.4 8.8 21.4 0 32.1-16.2 32.1-48.6V572.7h44.5v126.8h-0.1c0 27.6-6.4 48.7-19.1 63.5-12.7 14.6-30.9 22-54.6 22-10.5 0-20.3-1.8-29.2-5.4v-41.9zM864 952c0 4.4-3.6 8-8 8H224c-35.3 0-64-28.7-64-64h696c4.4 0 8 3.6 8 8v48z m9.2-170.6h-45.5l-88.4-135c-4.6-7-7.9-12.7-10-17h-0.7c0.8 7.2 1.2 18.3 1.2 33.3v118.7h-42.2V572.7h48.5l85.1 131.2c5.7 8.7 9.1 14.2 10.4 16.6h0.7c-0.9-5-1.3-14.6-1.3-28.8v-119h42.2v208.7zM286.5 728.8s-0.1 0 0 0z
                                          M605.8 678.2c0 20.9-4.8 37.5-14.5 49.8s-23.3 18.5-40.7 18.5c-17.1 0-30.5-6.4-40.3-19.3-9.8-12.9-14.7-29.5-14.7-49.8 0-20.6 5-37.3 15-50.2 10.1-12.9 23.8-19.4 41.2-19.4 17.2 0 30.5 6.3 39.9 18.9 9.4 12.6 14.1 29.8 14.1 51.5z";

        private string separatorLine = "==";

        // 构造
        public PromptEditorVM(IDispatcherService dispatcher,
                              IJsonConfigManagerService jsonConfigService,
                              Manager manager,
                              ITokenProviderService tokenProviderService,
                              IStringEncryptorService stringEncryptorService)
        {
            _dispatcher = dispatcher;
            _jsonConfigService = jsonConfigService;
            _manager = manager;
            _tokenProviderService = tokenProviderService;
            _stringEncryptorService = stringEncryptorService;

            // 初始化配置
            InitConfig();

            // 载入示例
            CreateTemplate();
        }
    }

    // 命令
    partial class PromptEditorVM
    {
        [RelayCommand]
        private void OnLoaded(object para)
        {
            this.Token = ((IToken)para).Token;
        }

        [RelayCommand]
        private void OnSaveConfig()
        {
            if (IsValidAesKey(this.DefuaultAesKey))
            {
                // 从具体属性保存
                _config.DefuaultAesIv = this.DefuaultAesIv;
                _config.DefuaultAesKey = this.DefuaultAesKey;
                _config.LocalFolder = this.LocalFolder;

                // 序列化
                _jsonConfigService.SaveConfig(ConfigName, _config);
            }
            else
            {
                Debug.WriteLine("AES Key is invalid. It must be exactly 32 characters.");
            }

            this.Print(true, " OnSaveConfig");
        }
        [RelayCommand]
        private void OnLoadConfig()
        {
            // 反序列化
            var flag = _jsonConfigService.LoadConfig<PromptEditorSettings>(ConfigName, (x) =>
            {
                _config = x;

                // 读取至具体属性
                this.DefuaultAesIv = x.DefuaultAesIv;
                this.DefuaultAesKey = x.DefuaultAesKey;
                this.LocalFolder = x.LocalFolder;

                if (string.IsNullOrEmpty(this.DefuaultAesIv)) { this.DefuaultAesIv = _stringEncryptorService.GetInnerAesIv(); }

                this.Print(true, " OnLoadConfig");
            });

            if (flag is false)
            {
                this.DefuaultAesIv = _tokenProviderService.GetRandomToken(16);
                this.DefuaultAesKey = _tokenProviderService.GetRandomToken(32);
                this.LocalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PromptEditor");
            }
        }

        [RelayCommand]
        private void OnRefreshAesIv()
        {
            this.DefuaultAesIv = _tokenProviderService.GetRandomToken(16);
        }
        [RelayCommand]
        private void OnRefreshAesKey()
        {
            this.DefuaultAesKey = _tokenProviderService.GetRandomToken(32);
        }

        [RelayCommand]
        private void OnFileDrop(object para) //
        {
            if (para is DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string firstFile = (e.Data.GetData(DataFormats.FileDrop) as Array)?.GetValue(0)?.ToString() ?? "";
                    string content = File.ReadAllText(firstFile).Trim();

                    try
                    {
                        // 尝试反序列化为 ToggleTreeViewNode 嵌套
                        using (JsonDocument doc = JsonDocument.Parse(content))
                        {
                            var root = new ToggleTreeViewNode() { UseDelayRender = true, ContentRenderType = ContentRenderType.ForJsonEditor, Enable = true, JsonKey = "Root" };
                            DeserializeTreeStructureForTVN(0, root, doc.RootElement);
                            if (root.HasChildren)
                            {
                                var firstNode = root.Children.Count == 1 ? root[0] : root;
                                this.PromptPacketList.Add(new PromptPacket(JsonIcon, firstNode.JsonKey, firstNode));
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        // 反序列化失败，视为 string
                        bool isPngText = false;

                        // 如果载入的是png，则尝试读取tEXt数据块
                        if (Path.GetExtension(firstFile).ToLower() == ".png")
                        {
                            using (var stream = new FileStream(firstFile, FileMode.Open, FileAccess.Read))
                            using (var reader = new BinaryReader(stream))
                            {
                                var chunk = FindFirstTextChunk(reader);
                                if (chunk != null)
                                {
                                    content = chunk.DataString;

                                    if (IsBase64String(content))
                                    {
                                        byte[] bytes = Convert.FromBase64String(content);
                                        string decodedText = Encoding.UTF8.GetString(bytes);

                                        this.PromptPacketList.Add(new PromptPacket(JsonIcon, "text", new PromptString()
                                        {
                                            Text = decodedText
                                        }));
                                    }
                                    else
                                    {
                                        this.PromptPacketList.Add(new PromptPacket(JsonIcon, "text", new PromptString()
                                        {
                                            Text = $"{content}"
                                        }));
                                    }

                                    isPngText = true;
                                }
                            }
                        }

                        // 原样添加
                        if (isPngText is false)
                        {
                            this.PromptPacketList.Add(new PromptPacket(TextIcon, "text", new PromptString()
                            {
                                Text = $"{content}"
                            }));
                        }

                        Debug.WriteLine($"{ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 清除所有PromptPacketList
        /// </summary>
        [RelayCommand]
        private async Task OnClearAllPrompt()
        {
            Action? yesnoCallback = null;
            if (await WeakReferenceMessenger.Default.Send(new DialogYesNoMessage("ClearAllPrompt ?", (x) => { yesnoCallback = x; }), this.Token))
            {
                this.PromptPacketList.Clear();
                this.PromptPacketList2.Clear();
                //this.FinalOutput.Text = string.Empty;
                this.Print(true, " OnClear");
            }
            yesnoCallback?.Invoke();
        }

        /// <summary>
        /// 重载于内部
        /// </summary>
        [RelayCommand]
        private async Task OnReloadFromInternalAsync()
        {
            Action? yesnoCallback = null;
            if (await WeakReferenceMessenger.Default.Send(new DialogYesNoMessage("ReloadFromInternal ?", (x) => { yesnoCallback = x; }), this.Token))
            {
                // 重新载入示例
                CreateTemplate(false);
                this.Print(true, " OnReloadFromInternal");
            }
            yesnoCallback?.Invoke();
        }
        /// <summary>
        /// 重载于桌面
        /// </summary>
        [RelayCommand]
        private async Task OnReloadFromDesktopAsync()
        {
            Action? yesnoCallback = null;
            if (await WeakReferenceMessenger.Default.Send(new DialogYesNoMessage("ReloadFromDesktop ?", (x) => { yesnoCallback = x; }), this.Token))
            {
                OnLoadFromDesktop();
                this.Print(true, " OnReloadFromDesktop");
            }
            yesnoCallback?.Invoke();
        }

        /// <summary>
        /// 添加JsonPrompt
        /// </summary>
        [RelayCommand]
        private void OnAddJsonPrompt()
        {
            this.Print(true, " OnAddJsonPrompt");
            this.PromptPacketList.Add(new PromptPacket(JsonIcon, "json", new ToggleTreeViewNode()
            {
                Enable = true,
                Text = "Root",
                UseDelayRender = true,
                ContentRenderType = ContentRenderType.ForJsonEditor
            }));
        }
        /// <summary>
        /// 添加TxtPrompt
        /// </summary>
        [RelayCommand]
        private void OnAddTxtPrompt()
        {
            this.Print(true, " OnAddTxtPrompt");
            this.PromptPacketList.Add(new PromptPacket(TextIcon, "text", new PromptString()
            {
                Text = "text",
            }));
        }

        /// <summary>
        /// 删除
        /// </summary>
        [RelayCommand]
        private async Task OnRemovePromptAsync(object para)
        {
            await _dispatcher.BeginInvoke(() =>
            {
                if (!(para is IPromptPacket item)) { return; }

                var list = this.PromptPacketList;
                var index = list.IndexOf(item);

                if (index > -1)
                {
                    list.RemoveAt(index);
                    this.Print(true, $" Remove: {item.PromptContent.GetType().Name}");
                }
            });
        }
        /// <summary>
        /// 上移
        /// </summary>
        [RelayCommand]
        private async Task OnMoveUpAsync(object para)
        {
            await _dispatcher.BeginInvoke(() =>
            {
                if (!(para is IPromptPacket item)) { return; }

                var list = this.PromptPacketList;
                var index = list.IndexOf(item);
                var count = list.Count;

                if (count > 1 && index > 0)
                {
                    var previous = list[index - 1]; // 获取 list[index - 1]
                    list.RemoveAt(index - 1); // 移除 previous
                    list.Insert(index, previous); // 将 previous 插入到 item 的旧位置
                }
            });
        }
        /// <summary>
        /// 下移
        /// </summary>
        [RelayCommand]
        private async Task OnMoveDownAsync(object para)
        {
            await _dispatcher.BeginInvoke(() =>
            {
                if (!(para is IPromptPacket item)) { return; }

                var list = this.PromptPacketList;
                var index = list.IndexOf(item);
                var count = list.Count;

                if (count > 1 && index > -1 && index < count - 1)
                {
                    var next = list[index + 1]; // 获取 list[index + 1]
                    list.RemoveAt(index + 1); // 移除 next
                    list.Insert(index, next); // 将 next 插入到 item 的旧位置
                }
            });
        }
        /// <summary>
        /// 启用/禁用
        /// </summary>
        [RelayCommand]
        private async Task OnUnusedAsync(object para)
        {
            await _dispatcher.BeginInvoke(() =>
            {
                if (!(para is IPromptPacket item)) { return; }

                item.Unused = !item.Unused;
            });
        }


        /// <summary>
        /// 打开PromptViewer
        /// </summary>
        [RelayCommand]
        private async Task OnOpenPromptViewerAsync()
        {
            this.OnPrintAllItems();
            await _manager.OpenPromptViewer(this.FinalOutput);
        }

        /// <summary>
        /// 打印选中项
        /// </summary>
        [RelayCommand]
        private void OnPrintSelectedItems()
        {
            var result = new StringBuilder();

            if (UseSeparator) { result.AppendLine(separatorLine); }

            if (SelectedPromptPacket?.PromptContent is PromptString str)
            {
                result.AppendLine(str.Text.Trim());
            }
            if (SelectedPromptPacket?.PromptContent is ToggleTreeViewNode node)
            {
                result.AppendLine(RootNodeTrim(node).Trim());
            }

            result = result.Replace("\\r\\n", "\n");

            var final = result.ToString().Trim();
            var trimLen = final.Replace("\r", "").Replace("\n", "").Replace(" ", "").Length;
            this.FinalOutput.Text = final;
            this.Print(true, $" Prompt Length: {this.FinalOutput.Text.Length}（Trimmed(\\r\\n␣): {trimLen}）");
        }
        /// <summary>
        /// 打印所有项
        /// </summary>
        [RelayCommand]
        private void OnPrintAllItems()
        {
            var result = new StringBuilder();
            foreach (var item in this.PromptPacketList)
            {
                var isLast = item == this.PromptPacketList.Last();

                if (item.Unused)
                {
                    Debug.WriteLine($"忽略【{item.ButtonText}】");
                    continue;
                }

                if (UseSeparator) { result.AppendLine(separatorLine); }

                if (item.PromptContent is PromptString prompt)
                {
                    result.AppendLine(prompt.Text.Trim());
                    if (isLast is false) { result.AppendLine(); }
                }
                if (item.PromptContent is ToggleTreeViewNode node)
                {
                    result.AppendLine(RootNodeTrim(node).Trim());
                    if (isLast is false) { result.AppendLine(); }
                }

                if (UseSeparator && item == this.PromptPacketList.Last())
                {
                    { result.Append(separatorLine); }
                }
            }

            result = result.Replace("\\r\\n", "\n");

            var final = result.ToString().Trim();
            var trimLen = final.Replace("\r", "").Replace("\n", "").Replace(" ", "").Length;
            this.FinalOutput.Text = final;
            this.Print(true, $" Prompt Length: {this.FinalOutput.Text.Length}（Trimmed(\\r\\n␣): {trimLen}）");
        }

        /// <summary>
        /// 自动打印选中项
        /// </summary>
        [RelayCommand]
        private void OnAutoPrintSelectedItems() => OnPrintSelectedItems();
        /// <summary>
        /// 自动打印所有项
        /// </summary>
        [RelayCommand]
        private void OnAutoPrintAllItems() => OnPrintAllItems();

        /// <summary>
        /// 储存至桌面 - 选中项
        /// </summary>
        [RelayCommand]
        private async Task OnSaveSelectedToDesktopAsync()
        {
            //var space2x = "        ";
            var space3x = "            ";
            var folderPath = this.LocalFolder;

            if (IsValidDirectoryPath(folderPath) is false) { return; }
            if (Directory.Exists(folderPath) is false) { Directory.CreateDirectory(folderPath); }

            if (this.SelectedPromptPacket is null) { return; }
            var selectedPrompt = GetPromptList(new List<IPromptPacket>() { this.SelectedPromptPacket }, ignoreUnused: true);

            // 储存 提示词主体 至本地
            await Task.Run(async () =>
            {
                if (selectedPrompt.Length == 0) { return; }

                var temp = string.Empty;
                var count = 0;
                foreach (var item in selectedPrompt)
                {
                    var realPath = Path.Combine(folderPath, $"selected_{count++}.txt");

                    using (StreamWriter sw = new StreamWriter($"{realPath}", false, Encoding.Unicode))
                    {
                        var response = item;
                        if (this.UseAes)
                        {
                            response = await WeakReferenceMessenger.Default.Send(new StringEncryptMessage()
                            {
                                AesIv = this.DefuaultAesIv,
                                AesKey = this.DefuaultAesKey,
                                TextSrc = item
                            }, this.Token);
                        }
                        sw.WriteLine(response);
                        temp += $"\n{space3x}\"{response}\",";
                    }
                }

            });
        }
        /// <summary>
        /// 储存至桌面 - 所有项
        /// </summary>
        [RelayCommand]
        private async Task OnSaveAllToDesktopAsync()
        {
            var space2x = "        ";
            var space3x = "            ";
            var folderPath = this.LocalFolder;

            if (IsValidDirectoryPath(folderPath) is false) { return; }
            if (Directory.Exists(folderPath) is false) { Directory.CreateDirectory(folderPath); }

            var mainPrompt = GetPromptList(this.PromptPacketList);
            var subPrompt = GetPromptList(this.PromptPacketList2);

            // 储存 提示词主体 至本地
            await Task.Run(async () =>
            {
                if (mainPrompt.Length == 0) { return; }

                // 删除所有 "数字.txt" 文件
                foreach (var file in Directory.GetFiles(folderPath, "*.txt"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (Regex.IsMatch(fileName, @"^\d+$")) // 检查是否是纯数字
                    {
                        File.Delete(file);
                    }
                }

                //
                var temp = string.Empty;
                var count = 0;
                foreach (var item in mainPrompt)
                {
                    var realPath = Path.Combine(folderPath, $"{count++:D2}.txt");

                    using (StreamWriter sw = new StreamWriter($"{realPath}", false, Encoding.Unicode))
                    {
                        var response = item;
                        if (this.UseAes)
                        {
                            response = await WeakReferenceMessenger.Default.Send(new StringEncryptMessage()
                            {
                                AesIv = this.DefuaultAesIv,
                                AesKey = this.DefuaultAesKey,
                                TextSrc = item
                            }, this.Token);
                        }
                        sw.WriteLine(response);
                        temp += $"\n{space3x}\"{response}\",";
                    }
                }
                temp = $"\n{space2x}private string[] prompt {{ get; set; }} = [{temp}\n{space2x}];";
                Debug.WriteLine(temp);
            });

            // 储存 提示词接续语 至本地
            await Task.Run(async () =>
            {
                if (subPrompt.Length == 0) { return; }

                var continuePrompt = subPrompt[0];
                var realPath = Path.Combine(folderPath, $"ContinuePrompt.txt");

                using (StreamWriter sw = new StreamWriter($"{realPath}", false, Encoding.Unicode))
                {
                    var response = continuePrompt;
                    if (this.UseAes)
                    {
                        response = await WeakReferenceMessenger.Default.Send(new StringEncryptMessage()
                        {
                            AesIv = this.DefuaultAesIv,
                            AesKey = this.DefuaultAesKey,
                            TextSrc = response
                        }, this.Token);
                    }
                    sw.WriteLine(response);
                    Debug.WriteLine($"{space2x}private string @continue {{ get; set; }} = \"{response}\";\n");
                }
            });
        }

        /// <summary>
        ///  从桌面载入
        /// </summary>
        [RelayCommand]
        private void OnLoadFromDesktop()
        {
            var folderPath = this.LocalFolder;

            ClearAllPromptPacketList();

            // 不存在直接返回
            if (!Directory.Exists(folderPath)) { return; }

            // 储存结果
            var result = new List<string>();

            // 获取文件夹中的所有 .txt 文件
            var files = Directory.GetFiles(folderPath, "*.txt");

            // 使用正则表达式筛选出文件名是纯数字的txt文件
            var regex = new Regex(@"^\d+\.txt$");

            // 
            _dispatcher.BeginInvoke(async () =>
            {
                // 遍历纯数字名的txt并读取内容
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file); // 获取文件名
                    if (regex.IsMatch(fileName))  // 判断文件名是否为纯数字
                    {
                        try
                        {
                            var response = string.Empty;
                            var content = File.ReadAllText(file);  // 读取文件内容
                            //Debug.WriteLine(content);
                            if (this.UseAes)
                            {
                                response = await WeakReferenceMessenger.Default.Send(new StringDecryptMessage()
                                {
                                    AesIv = this.DefuaultAesIv,
                                    AesKey = this.DefuaultAesKey,
                                    TextSrc = content
                                }, this.Token);
                            }
                            else
                            {
                                response = content;
                            }
                            result.Add(response);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"无法读取文件 {fileName}: {ex.Message}");
                        }
                    }
                }

                // 读取为 MainPrompt
                this.PromptPacketList.Clear();
                foreach (var item in result)
                {
                    var temp = RemoveEqualsFromFirstLine(item);
                    try
                    {
                        // 尝试反序列化为 ToggleTreeViewNode 嵌套
                        using (JsonDocument doc = JsonDocument.Parse(temp))
                        {
                            var root = new ToggleTreeViewNode() { UseDelayRender = true, ContentRenderType = ContentRenderType.ForJsonEditor, Enable = true, JsonKey = "Root" };
                            DeserializeTreeStructureForTVN(0, root, doc.RootElement);
                            if (root.HasChildren)
                            {
                                var firstNode = root.Children.Count == 1 ? root[0] : root;
                                this.PromptPacketList.Add(new PromptPacket(JsonIcon, firstNode.JsonKey, firstNode));
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        // 序列化失败视为 string
                        this.PromptPacketList.Add(new PromptPacket(TextIcon, "text", new PromptString()
                        {
                            Text = $"{temp}"
                        }));
                        Debug.WriteLine($"{ex.Message}");
                    }
                }

                // 读取为 ContinuePrompt
                this.PromptPacketList2.Clear();
                if (files.FirstOrDefault(file => Path.GetFileName(file) == "ContinuePrompt.txt") is string cp)
                {
                    var response = string.Empty;
                    var content = $"{File.ReadAllText(cp)}".Trim();
                    this.PromptPacketList2.Clear();
                    if (this.UseAes)
                    {
                        response = await WeakReferenceMessenger.Default.Send(new StringDecryptMessage()
                        {
                            AesIv = this.DefuaultAesIv,
                            AesKey = this.DefuaultAesKey,
                            TextSrc = content
                        }, this.Token);
                    }
                    else
                    {
                        response = content;
                    }
                    this.PromptPacketList2.Add(new PromptPacket(TextIcon, "接续词", new PromptString() { Text = response }));
                }

            });
        }

        /// <summary>
        /// 获取选中项的CSharp代码
        /// </summary>
        [RelayCommand]
        private async Task OnGetCSharpCodeFromSelectedItemAsync()
        {
            await Task.Yield();

            if (this.SelectedPromptPacket?.PromptContent is ToggleTreeViewNode node)
            {
                var sb = new StringBuilder();
                PrintNodeInternal(node, "", "", 1, sb); // Root层是 '角色设定'。传入数字1意思是 第一层是Add方法的起点

                var final = sb.ToString(); Debug.WriteLine(final);
                this.FinalOutput.Text = final;

                this.Print(true, " OnGetCSharpCodeFromSelectedItemAsync");
            }
        }
    }

    // 创建模板
    partial class PromptEditorVM
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            WriteIndented = true,
        };
        JsonSerializerOptions jsonlOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            WriteIndented = false,
        };

        ToggleTreeViewNode currentGate = null;

        private void ClearAllPromptPacketList()
        {
            this.PromptPacketList.Clear();
            this.PromptPacketList2.Clear();
        }
    }

    // 辅助方法
    partial class PromptEditorVM
    {
        /// <summary>
        /// 本地打印
        /// </summary>
        private void Print(bool useTimestamp, string msg)
        {
            this.SystemMessages.Add(new($"{DateTime.Now.ToString("HH:mm:ss")}{msg}"));
        }

        /// <summary>
        /// 递归打印 原始节点
        /// </summary>
        private void PrintNodeInternal(ToggleTreeViewNode gate, string prefix, string parentDepth, int startDepth, StringBuilder sb)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                //Debug.WriteLine($"ToggleTreeViewNode (Text: {gate.Text}, {gate.IsChecked})");
                sb.AppendLine("var gate = new ToggleTreeViewNode() { Text = \"角色设定\", Enable = true, UseDelayRender = true, ContentRenderType = ContentRenderType.ForJsonEditor };");
            }

            prefix += "    ";
            startDepth--;

            var childDepth = 0;

            foreach (var node in gate.Children.OfType<GateNode>())
            {
                if (node.Type == GateNodeType.GateBase)
                {
                    var item = (ToggleTreeViewNode)node.Content;

                    var jsonKey = item.JsonKey;
                    var isChecked = item.IsChecked ? "●" : "○";
                    var jsonValue = item.JsonValue.Substring(0, Math.Min(16, item.JsonValue.Length));
                    var hasChildren = item.HasChildren;

                    var hasParentDepth = !string.IsNullOrWhiteSpace(parentDepth);
                    var pdstr = $"{(hasParentDepth ? $"{parentDepth}" : "")}[{childDepth}]";
                    var pdstrReal = Regex.Replace(pdstr, @"\[\d+\]$", ""); //砍掉最右边一组方括号

                    //Debug.WriteLine($"{prefix}{pdstr} ToggleTreeViewNode (Text: {jsonKey}, {isChecked}, {jsonValue})");
                    sb.AppendLine($"{prefix}gate{pdstrReal}.Add(CreateTVN(\"{jsonKey}\"{(item.IsChecked ? "" : ", false")}));{(hasChildren ? "" : $" currentGate.JsonValue = \"{item.JsonValue}\";")}");

                    PrintNodeInternal(item, prefix, $"{pdstr}", startDepth, sb); childDepth++;
                }

                if (node.Type == GateNodeType.Delegate)
                {
                    var item = (DelegateNode)node.Content;

                    Debug.WriteLine($"{prefix} {item.Type}");
                }
            }
        }

        /// <summary>
        /// 序列化预备
        /// </summary>
        private object GenerateTreeStructureFromTVN(ToggleTreeViewNode parent_node, int depth = 0)
        {
            var isRoot = depth == 0;

            depth++;

            // 没有子节点时，用 Value 作为值
            if (parent_node.Children == null || parent_node.Children.Count == 0) { return parent_node.JsonValue; }

            // 待操作列表
            var childrenList = parent_node.Children.OfType<GateNode>().Where(node => node.Type == GateNodeType.GateBase);

            // 父节点ON  -> 返回字典 Dictionary<string, object>
            // 父节点OFF -> 返回列表 List<object>
            var useListOfObject = parent_node.IsChecked;

            // 转换为 字典 Dictionary<string, object>
            if (useListOfObject)
            {
                // 容器
                var result = new Dictionary<string, object>();
                // 字典用于记录当前层的名称及其出现次数
                var nameCount = new Dictionary<string, int>();

                // 第一阶段：生成唯一的 Name
                foreach (var node in childrenList)
                {
                    var item = (ToggleTreeViewNode)node.Content;

                    // 重名处理
                    var newJsonKey = item.JsonKey;
                    // 确保字典中有当前名称的计数
                    if (!nameCount.ContainsKey(newJsonKey))
                    {
                        nameCount[newJsonKey] = 0;
                    }
                    else
                    {
                        nameCount[newJsonKey]++;// 如果已经存在，增加计数
                    }
                    // 为名称添加后缀
                    newJsonKey = $"{newJsonKey}{nameCount[newJsonKey]}";

                    // 下一层
                    var value = GenerateTreeStructureFromTVN(item, depth);

                    // 写入字典
                    if (!result.ContainsKey(newJsonKey))
                    {
                        result[newJsonKey] = value;
                    }
                }

                // 第二阶段：移除绝无仅有的 Key 的序号
                foreach (var kvp in nameCount.Where(kvp => kvp.Value == 0).ToList())
                {
                    var newResultKey = kvp.Key;
                    var oldResultKey = kvp.Key + kvp.Value;
                    var oldResultValue = result[oldResultKey];
                    if (string.IsNullOrWhiteSpace(newResultKey)) { newResultKey = oldResultKey; }
                    result.Remove(oldResultKey);
                    result.Add(newResultKey, oldResultValue);
                }

                //
                return isRoot ? new Dictionary<string, object>() { { parent_node.JsonKey, result } } : result;
                //return (isRoot && !string.IsNullOrWhiteSpace(parent_node.JsonKey)) ? new Dictionary<string, object>() { { parent_node.JsonKey, result } } : result;
            }

            // 转换为 列表 List<object>
            else
            {
                // 子无key = 子返回jsonValue
                // 子有key = 子返回Dictionary<string, object>
                var objectList = new List<object>();

                // 容器
                var result = new Dictionary<string, object>();
                // 字典用于记录当前层的名称及其出现次数
                var nameCount = new Dictionary<string, int>();

                // 第零阶段：声明变量
                var emptyKeyIndex = 0;
                var emptyKeyPrefix = "vQ$3ju6W$5YV%rpxkRWSMk5A3@2z&hB4";

                // 第一阶段：生成唯一的 Name
                foreach (var node in childrenList)
                {
                    var item = (ToggleTreeViewNode)node.Content;

                    // 重名处理
                    var newJsonKey = item.JsonKey;
                    // 判断空白Key
                    var isEmptyKey = string.IsNullOrWhiteSpace(newJsonKey);
                    // 确保字典中有当前名称的计数（跳过空白Key）
                    if (isEmptyKey is false)
                    {
                        if (!nameCount.ContainsKey(newJsonKey))
                        {
                            nameCount[newJsonKey] = 0;
                        }
                        else
                        {
                            nameCount[newJsonKey]++;// 如果已经存在，增加计数
                        }

                        // 为名称添加后缀
                        newJsonKey = $"{newJsonKey}{nameCount[newJsonKey]}";
                    }
                    // 确保用于EmptyKey的字符串仅限前缀相同
                    if (isEmptyKey) { newJsonKey = emptyKeyPrefix + $"_{emptyKeyIndex++}"; }

                    // 下一层
                    var value = GenerateTreeStructureFromTVN(item, depth);

                    // 写入字典
                    if (!result.ContainsKey(newJsonKey))
                    {
                        result[newJsonKey] = value;
                    }
                }

                // 第二阶段：移除绝无仅有的 Key 的序号
                foreach (var kvp in nameCount.Where(kvp => kvp.Value == 0).ToList())
                {
                    var newResultKey = kvp.Key;
                    var oldResultKey = kvp.Key + kvp.Value;
                    var oldResultValue = result[oldResultKey];
                    if (string.IsNullOrWhiteSpace(newResultKey)) { newResultKey = oldResultKey; }
                    result.Remove(oldResultKey);
                    result.Add(newResultKey, oldResultValue);
                }

                // 第三阶段：写入列表
                foreach (var x in result)
                {
                    if (x.Key.StartsWith(emptyKeyPrefix))
                    {
                        objectList.Add(x.Value);
                    }
                    else
                    {
                        objectList.Add(new Dictionary<string, object>() { { x.Key, x.Value }, });
                    }
                }

                // 返回
                return isRoot ? new Dictionary<string, object>() { { parent_node.JsonKey, objectList } } : objectList;
                //return (isRoot && !string.IsNullOrWhiteSpace(parent_node.JsonKey)) ? new Dictionary<string, object>() { { parent_node.JsonKey, objectList } } : objectList;
            }

            // 无法抵达的
            throw new NotImplementedException();
        }

        /// <summary>
        /// <para><see cref="ObservableCollection&lt;IPromptPacket&gt;"/> -> <see cref="string"/>[]</para>
        /// </summary>
        private string[] GetPromptList<T>(IList<T> list, bool ignoreUnused = false) where T : IPromptPacket
        {
            var tempList = new List<string>();
            foreach (var item in list)
            {
                if (ignoreUnused is false)
                {
                    if (item.Unused)
                    {
                        //Debug.WriteLine($"忽略【{item.ButtonText}】");
                        continue;
                    }
                }

                if (item.PromptContent is PromptString prompt)
                {
                    tempList.Add(prompt.Text.Trim());
                }
                if (item.PromptContent is ToggleTreeViewNode node)
                {
                    tempList.Add(RootNodeTrim(node).Trim());
                }
            }
            return tempList.ToArray();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        private void DeserializeTreeStructureForTVN(int depth, ToggleTreeViewNode parent_node, JsonElement element, string indent = "")
        {
            var isRoot = depth == 0; depth++;

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var count = element.EnumerateObject().Count();
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        //Debug.WriteLine($"A-{indent}{property.Name}:");
                        var child_node = new ToggleTreeViewNode()
                        {
                            Enable = true,
                            JsonKey = $"{property.Name}",
                            JsonValue = string.Empty
                        };
                        DeserializeTreeStructureForTVN(depth, child_node, property.Value, indent + "  ");
                        parent_node.Add(child_node);
                        //Debug.WriteLine($"{indent} {child_node.JsonKey}:{child_node.JsonValue}");

                        // 简化单个成员的情况
                        if (child_node.GateBaseList.Count == 1 && !child_node.GateBaseList[0].HasChildren)
                        {
                            child_node.JsonValue = child_node.GateBaseList[0].JsonValue;
                            child_node.Children.Clear();
                            //Debug.WriteLine($"{child_node.GateBaseList}");
                        }
                    }
                    break;

                case JsonValueKind.Array:
                    parent_node.Enable = false;
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Object || item.ValueKind == JsonValueKind.Array)
                        {
                            var child_node = new ToggleTreeViewNode()
                            {
                                Enable = true,
                                JsonKey = "",
                                JsonValue = string.Empty
                            };
                            DeserializeTreeStructureForTVN(depth, child_node, item, indent + "  ");
                            parent_node.Add(child_node);
                        }
                        else
                        {
                            // 直接加到父节点，不包一层
                            parent_node.Add(new ToggleTreeViewNode()
                            {
                                Enable = false,
                                JsonKey = "",
                                JsonValue = item.ToString()  // 你可以根据类型精细化处理
                            });
                        }
                    }
                    break;

                case JsonValueKind.String:
                    parent_node.Add(new ToggleTreeViewNode() { Enable = false, JsonKey = "", JsonValue = $"{element.GetString()}" });
                    break;

                case JsonValueKind.Number:
                    parent_node.Add(new ToggleTreeViewNode() { Enable = false, JsonKey = "", JsonValue = $"{element.GetRawText()}" });
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    parent_node.Add(new ToggleTreeViewNode() { Enable = false, JsonKey = "", JsonValue = $"{element.GetBoolean()}" });
                    break;

                case JsonValueKind.Null:
                    parent_node.Add(new ToggleTreeViewNode() { Enable = false, JsonKey = "", JsonValue = $"" });
                    break;

                default:
                    Debug.WriteLine($"{indent}Unsupported value type");
                    break;
            }
        }

        /// <summary>
        /// 创建JSON提示词（专用节点）
        /// </summary>
        private ToggleTreeViewNode CreateTVN(string jsonKey, bool enable = true)
        {
            if (this.FullMode)
            {
                currentGate = new ToggleTreeViewNode()
                {
                    Enable = enable,
                    JsonKey = jsonKey,
                };

                return currentGate;
            }
            else
            {
                currentGate ??= new ToggleTreeViewNode();

                return new ToggleTreeViewNode()
                {
                    Enable = enable,
                    JsonKey = jsonKey,
                };
            }
        }
        /// <summary>
        /// 创建纯文本提示词（专用容器）
        /// </summary>
        private PromptString CreatePS(string text)
        {
            return new PromptString()
            {
                Text = this.FullMode ? text : string.Empty
            };
        }

        /// <summary>
        ///  删除行首的==
        /// </summary>
        private string RemoveEqualsFromFirstLine(string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            // 获取第一行
            var lines = input.Split(['\n'], 2);
            if (lines[0].StartsWith("=="))
            {
                // 删除"=="并拼接回其余部分
                return lines.Length > 1
                    ? lines[0][2..] + "\n" + lines[1]
                    : lines[0][2..];
            }

            return input; // 如果第一行不是以"=="开头，返回原始输入
        }

        /// <summary>
        /// 基础防呆
        /// </summary>
        private bool IsValidAesKey(string aesKey)
        {
            // 判断是否为32个字符
            return !string.IsNullOrEmpty(aesKey) && aesKey.Length == 32;
        }

        /// <summary>
        /// 基础防呆
        /// </summary>
        private bool IsValidDirectoryPath(string path)
        {
            // 检查路径是否为空或空白
            if (string.IsNullOrWhiteSpace(path)) return false;

            // 检查路径是否包含非法字符
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0) return false;

            // 检查路径是否是有效的绝对路径
            try
            {
                Path.GetFullPath(path);  // 如果路径无效会抛出异常
            }
            catch (ArgumentException)
            {
                return false;
            }

            // 检查路径是否已存在且是文件
            if (File.Exists(path)) return false;

            return true;
        }


        /// <summary>
        /// Root节点JsonKey为空值时，抛弃JsonKey，然后直接返回JsonValue
        /// </summary>
        /// <returns></returns>
        private string RootNodeTrim(ToggleTreeViewNode rootNode)
        {
            var rootObj = GenerateTreeStructureFromTVN(rootNode);

            // to Json/Jsonl
            if (rootObj is Dictionary<string, object> obj && string.IsNullOrWhiteSpace(obj.Keys.First()))
            {
                return (JsonSerializer.Serialize(obj.Values.First(), this.UseJsonl ? jsonlOptions : jsonOptions) + "");
            }
            else
            {
                return (JsonSerializer.Serialize(rootObj, this.UseJsonl ? jsonlOptions : jsonOptions) + "");
            }
        }

        /// <summary>
        /// 查找第一个 tEXt Chunk
        /// </summary>
        private PngChunk FindFirstTextChunk(BinaryReader reader)
        {
            // 跳过 PNG 文件头（8字节签名）
            reader.BaseStream.Position = 8;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // 读取 Length (4 bytes, big endian)
                byte[] lenBytes = reader.ReadBytes(4);
                if (lenBytes.Length < 4) break;  // EOF
                int length = (lenBytes[0] << 24) | (lenBytes[1] << 16) | (lenBytes[2] << 8) | lenBytes[3];

                // 读取 Name (4 bytes)
                byte[] nameBytes = reader.ReadBytes(4);
                if (nameBytes.Length < 4) break;
                string name = System.Text.Encoding.ASCII.GetString(nameBytes);

                // 读取 Data
                byte[] dataBytes = reader.ReadBytes(length);
                if (dataBytes.Length < length) break;

                // 读取 CRC
                byte[] crcBytes = reader.ReadBytes(4);
                if (crcBytes.Length < 4) break;

                if (name == "tEXt")
                {
                    // 处理 keyword + 0x00 + text
                    int sepIndex = Array.IndexOf(dataBytes, (byte)0x00);

                    string text = "";
                    if (sepIndex >= 0 && sepIndex + 1 < dataBytes.Length)
                    {
                        text = Encoding.UTF8.GetString(dataBytes, sepIndex + 1, dataBytes.Length - sepIndex - 1);
                    }
                    else if (sepIndex < 0)
                    {
                        // 没有找到 separator → 整个当作 text
                        text = Encoding.UTF8.GetString(dataBytes);
                    }
                    // else: 如果 sepIndex 刚好是最后一个字节，text = 空

                    var chunk = new PngChunk
                    {
                        Length = length,
                        Name = name,
                        DataBytes = dataBytes,
                        DataString = text,
                        CrcBytes = crcBytes,
                        FullChunk = new byte[4 + 4 + length + 4]
                    };

                    // 组装完整 Chunk
                    Buffer.BlockCopy(lenBytes, 0, chunk.FullChunk, 0, 4);
                    Buffer.BlockCopy(nameBytes, 0, chunk.FullChunk, 4, 4);
                    Buffer.BlockCopy(dataBytes, 0, chunk.FullChunk, 8, length);
                    Buffer.BlockCopy(crcBytes, 0, chunk.FullChunk, 8 + length, 4);

                    return chunk;  // 找到第一个 tEXt，返回
                }
                else
                {
                    // 不是 tEXt，继续下一个 Chunk
                    // （指针已经自动移动，无需复位）
                }
            }

            return null;  // 没找到 tEXt
        }

        /// <summary>
        /// 检查字符串是否为有效的Base64编码
        /// </summary>
        private bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return false;

            base64 = base64.Trim();

            if (base64.Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

}
