using System.Text.Json;
using System.Text.RegularExpressions;
using TrarsUI.Shared.DTOs;
using TreePrompt2Json.PromptBuilder.MVVM.ViewModels.PartialFunc.PublicBuild.TemplateCreater;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
#if PUBLIC_BUILD // 防误裁剪
    partial class PromptEditorVM
    {
        static readonly Type[] _publicBuild = {
            typeof(Regex),
            typeof(JsonDocument),
        };
    }
#else
    partial class PromptEditorVM
    {
        static readonly Type[] _publicBuild = {
            typeof(AlertProxy),
            typeof(CharacterTemplate),
            typeof(Regex),
            typeof(JsonDocument),
        };
    }
#endif

#if PUBLIC_BUILD
    partial class PromptEditorVM
    {
        CharacterTemplate characterTemplate = new();

        private partial void CreateTemplate(bool autoSelect = true)
        {
            var system = CreateSystemRule();
            var charA = characterTemplate.GetPrompt(CharacterTemplate.CharacterIdx.charA);
            var charB = characterTemplate.GetPrompt(CharacterTemplate.CharacterIdx.charB);
            var charC = characterTemplate.GetPrompt(CharacterTemplate.CharacterIdx.charC);
            var story = CreateStory();
            var plot = CreatePlot();
            var @continue = CreateContinuePrompt();

            ClearAllPromptPacketList();

            this.PromptPacketList = new()
            {
                CreatePromptPacket(JsonIcon, "系统规则", system),
                CreatePromptPacket(JsonIcon, "角色A", charA, isChecked: autoSelect),
                CreatePromptPacket(JsonIcon, "角色B", charB),
                CreatePromptPacket(JsonIcon, "角色C", charC),
                CreatePromptPacket(JsonIcon, "故事设定", story),
                CreatePromptPacket(JsonIcon, "故事楔子", plot),
            };

            this.PromptPacketList2 = new()
            {
                CreatePromptPacket(TextIcon, "接续词", @continue),
            };
        }
    }

    // 生成默认提示词
    partial class PromptEditorVM
    {
        private partial ToggleTreeViewNode CreateSystemRule()
        {
            return CreateRootNode("SystemRule");
        }
        private partial ToggleTreeViewNode CreateStory()
        {
            return CreateRootNode("Story");
        }
        private partial ToggleTreeViewNode CreatePlot()
        {
            return CreateRootNode("Plot");
        }

        private partial PromptString CreateContinuePrompt()
        {
            return new() { Text = "ContinuePrompt" };
        }

        private ToggleTreeViewNode CreateRootNode(string jsonKey)
        {
            var root = new ToggleTreeViewNode()
            {
                Enable = true,
                JsonKey = string.Empty,
                UseDelayRender = true,
                ContentRenderType = ContentRenderType.ForJsonEditor
            };
            root.Add(new ToggleTreeViewNode() { Enable = true, JsonKey = $"{jsonKey}" });
            return root;
        }
    }
#endif
}
