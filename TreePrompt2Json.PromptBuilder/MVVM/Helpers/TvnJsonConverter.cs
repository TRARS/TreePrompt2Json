using System.Diagnostics;
using System.Text.Json;
using TrarsUI.Shared.DTOs;
using TrarsUI.Shared.Helpers.Enums;
using TrarsUI.Shared.Helpers.Extensions;

namespace TreePrompt2Json.PromptBuilder.MVVM.Helpers
{
    internal partial class TvnJsonConverter
    {
        /// <summary>
        /// 避免使用，因为会造成更多困扰
        /// </summary>
        private bool arrayUp = false;

        /// <summary>
        /// 序列化预备
        /// </summary>
        public object GenerateTreeStructureFromTVN(ToggleTreeViewNode parent_node, int depth = 0)
        {
            var isRoot = depth == 0;

            depth++;

            // 没有子节点时，用 Value 作为值
            if (parent_node.Children == null || parent_node.Children.Count == 0)
            {
                // 根据JsonValueType返回对应类型
                return parent_node.JsonValueType switch
                {
                    JsonValueType.String => parent_node.JsonValue,
                    JsonValueType.Number => double.TryParse(parent_node.JsonValue, out var number) ? number : throw new FormatException($"Invalid number literal: '{parent_node.JsonValue.Shorten()}'"),
                    JsonValueType.Boolean => bool.TryParse(parent_node.JsonValue, out var boolean) ? boolean : throw new FormatException($"Invalid boolean literal: '{parent_node.JsonValue.Shorten()}'"),
                    JsonValueType.Null => parent_node.JsonValue == "null" ? null! : throw new FormatException($"Invalid null literal: expected 'null', got '{parent_node.JsonValue.Shorten()}'"),
                    JsonValueType.Object => throw new InvalidOperationException("Should not happen: Object should have children"),
                    JsonValueType.Array => throw new InvalidOperationException("Should not happen: Array should have children"),
                    _ => throw new InvalidOperationException("Should not happen: Undefined type")
                };

                //return parent_node.JsonValue;
            }

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
                var emptyKeyPrefix = Guid.NewGuid().ToString();

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
            }

            // 无法抵达的
            throw new NotImplementedException();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public ToggleTreeViewNode DeserializeTreeStructureForTVN(string json, string rootName = "")
        {
            using (var doc = JsonDocument.Parse(json))
            {
                var root = BuildNode(rootName, doc.RootElement);

                root.UseDelayRender = true; // 刚需
                root.ContentRenderType = ContentRenderType.ForJsonEditor; // 刚需

                return root;
            }
        }
        private ToggleTreeViewNode BuildNode(string name, JsonElement element, string indent = "")
        {
            var node = new ToggleTreeViewNode { Enable = true, JsonKey = name };
            var childList = new List<ToggleTreeViewNode>();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    node.Enable = true;
                    foreach (var prop in element.EnumerateObject())
                    {
                        var next = BuildNode(prop.Name, prop.Value);
                        childList.Add(next);
                    }
                    node.AddRange(childList); node.JsonValueType = JsonValueType.Object;
                    break;

                case JsonValueKind.Array:
                    node.Enable = false;
                    foreach (var item in element.EnumerateArray())
                    {
                        var next = BuildNode("", item); // 数组成员 没有key 只有value
                        childList.Add(next);
                    }

                    if (arrayUp)
                    {
                        foreach (var child in childList)
                        {
                            if (child.GateBaseList.Count == 1 && child.GateBaseList[0].GateBaseList.Count == 0)
                            {
                                child.JsonKey = child.GateBaseList[0].JsonKey;
                                child.JsonValue = child.GateBaseList[0].JsonValue;
                                child.Children.Clear();
                            }
                            if (child.GateBaseList.Count == 1 && child.GateBaseList[0].GateBaseList.Count > 1)
                            {
                                Debug.WriteLine($"parent:{node.JsonKey}, child:{child.GateBaseList[0].JsonKey}, children:{child.GateBaseList[0].GateBaseList.Count}");

                                child.JsonKey = child.GateBaseList[0].JsonKey;
                                child.JsonValue = "";

                                var temp = new List<ToggleTreeViewNode>();
                                foreach (var bk in child.GateBaseList[0].GateBaseList)
                                {
                                    temp.Add(bk);
                                }
                                child.Children.Clear();
                                child.AddRange(temp, true);
                            }
                        }
                    }

                    node.AddRange(childList); node.JsonValueType = JsonValueType.Array;
                    break;

                case JsonValueKind.String:
                    node.Enable = true; node.JsonValue = $"{element.GetString()}"; node.JsonValueType = JsonValueType.String;
                    break;

                case JsonValueKind.Number:
                    node.Enable = true; node.JsonValue = element.GetRawText(); node.JsonValueType = JsonValueType.Number;
                    break;

                case JsonValueKind.True:
                    node.Enable = true; node.JsonValue = element.GetRawText(); node.JsonValueType = JsonValueType.Boolean;
                    break;
                case JsonValueKind.False:
                    node.Enable = true; node.JsonValue = element.GetRawText(); node.JsonValueType = JsonValueType.Boolean;
                    break;

                case JsonValueKind.Null:
                    node.Enable = true; node.JsonValue = "null"; node.JsonValueType = JsonValueType.Null;
                    break;
            }

            return node;
        }
    }
}
