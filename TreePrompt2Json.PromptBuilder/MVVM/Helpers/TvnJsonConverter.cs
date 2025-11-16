using System.Diagnostics;
using System.Text.Json;
using TrarsUI.Shared.DTOs;

namespace TreePrompt2Json.PromptBuilder.MVVM.Helpers
{
    internal partial class TvnJsonConverter
    {
        /// <summary>
        /// 序列化预备
        /// </summary>
        public object GenerateTreeStructureFromTVN(ToggleTreeViewNode parent_node, int depth = 0)
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
        /// 反序列化
        /// </summary>
        public void DeserializeTreeStructureForTVN(int depth, ToggleTreeViewNode parent_node, JsonElement element, string indent = "")
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

    }
}
