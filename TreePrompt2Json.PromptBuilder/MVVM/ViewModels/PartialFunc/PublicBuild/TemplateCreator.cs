using TrarsUI.Shared.DTOs;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels
{
#if PUBLIC_BUILD
    partial class PromptEditorVM
    {
        private partial void CreateTemplate(bool autoSelect = true)
        {
            var charA = CreateCharacterA();

            ClearAllPromptPacketList();

            this.PromptPacketList = new()
            {
                new PromptPacket(JsonIcon, "角色设定A", charA) { IsChecked = autoSelect },
            };

            this.PromptPacketList2 = new()
            {

            };
        }
    }

    // 生成默认提示词
    partial class PromptEditorVM
    {
        private partial PromptString CreateStart()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateSystemRule()
        {
            return new();
        }
        private partial ToggleTreeViewNode CreateCharacterA()
        {
            var gate = new ToggleTreeViewNode() { Text = "角色设定", Enable = true, UseDelayRender = true, ContentRenderType = ContentRenderType.ForJsonEditor };
            gate.Add(CreateTVN("character"));
            gate[0].Add(CreateTVN("name")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("age")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("race")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("class")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("epithet")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("appearance"));
            gate[0][5].Add(CreateTVN("general")); currentGate.JsonValue = "";
            gate[0][5].Add(CreateTVN("unique")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("body"));
            gate[0][6].Add(CreateTVN("height")); currentGate.JsonValue = "";
            gate[0][6].Add(CreateTVN("weight")); currentGate.JsonValue = "";
            gate[0][6].Add(CreateTVN("figure")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("clothing"));
            gate[0][7].Add(CreateTVN("regular"));
            gate[0][7][0].Add(CreateTVN("general")); currentGate.JsonValue = "";
            gate[0][7][0].Add(CreateTVN("details"));
            gate[0][7][0][1].Add(CreateTVN("dress")); currentGate.JsonValue = "";
            gate[0][7][0][1].Add(CreateTVN("bra")); currentGate.JsonValue = "";
            gate[0][7][0][1].Add(CreateTVN("panties")); currentGate.JsonValue = "";
            gate[0][7].Add(CreateTVN("intimate"));
            gate[0][7][1].Add(CreateTVN("general")); currentGate.JsonValue = "";
            gate[0][7][1].Add(CreateTVN("details"));
            gate[0][7][1][1].Add(CreateTVN("nightgown")); currentGate.JsonValue = "";
            gate[0][7][1][1].Add(CreateTVN("bra")); currentGate.JsonValue = "";
            gate[0][7][1][1].Add(CreateTVN("panties")); currentGate.JsonValue = "";
            gate[0][7].Add(CreateTVN("battle")); currentGate.JsonValue = "";
            gate[0][7].Add(CreateTVN("travel")); currentGate.JsonValue = "";
            gate[0][7].Add(CreateTVN("formal_gown")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("voice")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("personality"));
            gate[0][9].Add(CreateTVN("general")); currentGate.JsonValue = "";
            gate[0][9].Add(CreateTVN("sexual"));
            gate[0][9][1].Add(CreateTVN("preferences", false));
            gate[0][9][1][0].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][9][1][0].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][9][1][0].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][9][1][0].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][9][1].Add(CreateTVN("reactions"));
            gate[0][9][1][1].Add(CreateTVN("physical"));
            gate[0][9][1][1][0].Add(CreateTVN("ears")); currentGate.JsonValue = "";
            gate[0][9][1][1][0].Add(CreateTVN("neck")); currentGate.JsonValue = "";
            gate[0][9][1][1][0].Add(CreateTVN("breasts")); currentGate.JsonValue = "";
            gate[0][9][1][1][0].Add(CreateTVN("vulva_arousal")); currentGate.JsonValue = "";
            gate[0][9][1][1][0].Add(CreateTVN("climax")); currentGate.JsonValue = "";
            gate[0][9][1][1][0].Add(CreateTVN("afterglow")); currentGate.JsonValue = "";
            gate[0][9][1][1].Add(CreateTVN("emotional"));
            gate[0][9][1][1][1].Add(CreateTVN("initial")); currentGate.JsonValue = "";
            gate[0][9][1][1][1].Add(CreateTVN("foreplay")); currentGate.JsonValue = "";
            gate[0][9][1][1][1].Add(CreateTVN("intercourse")); currentGate.JsonValue = "";
            gate[0][9][1][1][1].Add(CreateTVN("climax")); currentGate.JsonValue = "";
            gate[0][9][1][1][1].Add(CreateTVN("afterglow")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("physiology"));
            gate[0][10].Add(CreateTVN("vulva"));
            gate[0][10][0].Add(CreateTVN("entrance")); currentGate.JsonValue = "";
            gate[0][10][0].Add(CreateTVN("clitoris")); currentGate.JsonValue = "";
            gate[0][10][0].Add(CreateTVN("labia_minora")); currentGate.JsonValue = "";
            gate[0][10][0].Add(CreateTVN("labia_majora")); currentGate.JsonValue = "";
            gate[0][10][0].Add(CreateTVN("pubic_hair")); currentGate.JsonValue = "";
            gate[0][10][0].Add(CreateTVN("hymen")); currentGate.JsonValue = "";
            gate[0][10].Add(CreateTVN("vaginal")); currentGate.JsonValue = "";
            gate[0][10].Add(CreateTVN("uterus")); currentGate.JsonValue = "";
            gate[0][10].Add(CreateTVN("climax")); currentGate.JsonValue = "";
            gate[0][10].Add(CreateTVN("recovery")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("relationships")); currentGate.JsonValue = "";
            gate[0].Add(CreateTVN("miscellaneous", false));
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";
            gate[0][12].Add(CreateTVN("", false)); currentGate.JsonValue = "";

            return gate;
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
