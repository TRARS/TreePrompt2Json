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
            var charA = characterTemplate.GetPrompt(CharacterTemplate.CharacterIdx.charA);
            var charB = characterTemplate.GetPrompt(CharacterTemplate.CharacterIdx.charB);
            var charC = characterTemplate.GetPrompt(CharacterTemplate.CharacterIdx.charC);

            ClearAllPromptPacketList();

            this.PromptPacketList = new()
            {
                CreatePromptPacket(JsonIcon, "charA", charA, isChecked: autoSelect),
                CreatePromptPacket(JsonIcon, "charB", charB),
                CreatePromptPacket(JsonIcon, "charC", charC),
            };

            this.PromptPacketList2 = new()
            {

            };
        }
    }

    // 生成默认提示词
    partial class PromptEditorVM
    {
        private partial ToggleTreeViewNode CreateSystemRule()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateCharacterA()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateCharacterB()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateCharacterC()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateCharacterD()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateCharacterE()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateStory()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreatePlot()
        {
            return new();
        }
        private partial PromptString CreateOutputFormat()
        {
            return new();
        }
        private partial PromptString CreateEnd()
        {
            return new();
        }

        private partial PromptString CreateContinuePrompt()
        {
            return new();
        }
    }
#endif
}
