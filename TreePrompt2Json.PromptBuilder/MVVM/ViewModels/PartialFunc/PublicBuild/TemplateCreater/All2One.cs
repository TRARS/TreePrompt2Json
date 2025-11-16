using System.Text.Json;
using TrarsUI.Shared.DTOs;
using TreePrompt2Json.PromptBuilder.MVVM.Helpers;

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels.PartialFunc.PublicBuild.TemplateCreater
{
    static class TemplateHelper
    {
        private static TvnJsonConverter _helper = new();
        public static ToggleTreeViewNode GetTemplate<T>(T index, List<string> tempList) where T : Enum
        {
            var idx = Convert.ToInt32(index);

            if (idx < 0 || idx > tempList.Count() - 1)
            {
                idx = 0;
            }

            var root = new ToggleTreeViewNode() { UseDelayRender = true, ContentRenderType = ContentRenderType.ForJsonEditor, Enable = true, JsonKey = "Root" };
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(tempList[idx]))
                {
                    _helper.DeserializeTreeStructureForTVN(0, root, doc.RootElement);
                    if (root.HasChildren)
                    {
                        var firstNode = root.Children.Count == 1 ? root[0] : root;
                        return firstNode;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return root;
        }
        public static string GetTemplateAtString<T>(T index, List<string> tempList)
        {
            var idx = Convert.ToInt32(index);

            if (idx < 0 || idx > tempList.Count() - 1)
            {
                idx = 0;
            }

            return tempList[idx];
        }
    }
}

namespace TreePrompt2Json.PromptBuilder.MVVM.ViewModels.PartialFunc.PublicBuild.TemplateCreater
{
    // 角色模板，多个。
    class CharacterTemplateCharA
    {
        public enum CharacterIdx
        {
            Latest,
        }
        List<string> characterList =
            [
            @"
{""character"":{""name"":""CharacterA"",""core"":{""age"":"""",""race"":"""",""class"":"""",""epithet"":""""},""appearance"":{""core"":"""",""unique"":""""},""body"":{""height"":"""",""figure"":""""},""wardrobe"":{""casual"":"""",""intimate"":"""",""ceremonial"":"""",""battle"":"""",""travel"":"""",""formal"":""""},""personality"":"""",""abilities"":["""",""""],""racial_features"":{""scent"":"""",""climax_phenomenon"":"""",""special_period"":"""",""grooming_instinct"":"""",""constitution"":""""},""intimacy"":{""preferences"":["""","""","""",""""],""response_curve"":"""",""secret_knowledge"":""""},""relationships"":""""}}
",
            ];
        public string GetPrompt(CharacterIdx index) => TemplateHelper.GetTemplateAtString(index, characterList).Trim();
    }
    class CharacterTemplateCharB
    {
        public enum CharacterIdx
        {
            Latest,
        }
        List<string> characterList =
            [
            @"
{""character"":{""name"":""CharacterB"",""core"":{""age"":"""",""race"":"""",""class"":"""",""epithet"":""""},""appearance"":{""core"":"""",""unique"":""""},""body"":{""height"":"""",""figure"":""""},""wardrobe"":{""casual"":"""",""intimate"":"""",""ceremonial"":"""",""battle"":"""",""travel"":"""",""formal"":""""},""personality"":"""",""abilities"":["""",""""],""racial_features"":{""scent"":"""",""climax_phenomenon"":"""",""special_period"":"""",""grooming_instinct"":"""",""constitution"":""""},""intimacy"":{""preferences"":["""","""","""",""""],""response_curve"":"""",""secret_knowledge"":""""},""relationships"":""""}}
",
            ];
        public string GetPrompt(CharacterIdx index) => TemplateHelper.GetTemplateAtString(index, characterList).Trim();
    }
    class CharacterTemplateCharC
    {
        public enum CharacterIdx
        {
            Latest,
        }
        List<string> characterList =
            [
            @"
{""character"":{""name"":""CharacterC"",""core"":{""age"":"""",""race"":"""",""class"":"""",""epithet"":""""},""appearance"":{""core"":"""",""unique"":""""},""body"":{""height"":"""",""figure"":""""},""wardrobe"":{""casual"":"""",""intimate"":"""",""ceremonial"":"""",""battle"":"""",""travel"":"""",""formal"":""""},""personality"":"""",""abilities"":["""",""""],""racial_features"":{""scent"":"""",""climax_phenomenon"":"""",""special_period"":"""",""grooming_instinct"":"""",""constitution"":""""},""intimacy"":{""preferences"":["""","""","""",""""],""response_curve"":"""",""secret_knowledge"":""""},""relationships"":""""}}
",
            ];
        public string GetPrompt(CharacterIdx index) => TemplateHelper.GetTemplateAtString(index, characterList).Trim();
    }

    // 总角色模板
    class CharacterTemplate
    {
        public enum CharacterIdx
        {
            charA, charB, charC
        }

        CharacterTemplateCharA charA = new();
        CharacterTemplateCharB charB = new();
        CharacterTemplateCharC charC = new();

        List<string> characterList;

        public CharacterTemplate()
        {
            characterList = new List<string>
            {
                charA.GetPrompt(CharacterTemplateCharA.CharacterIdx.Latest),
                charB.GetPrompt(CharacterTemplateCharB.CharacterIdx.Latest),
                charC.GetPrompt(CharacterTemplateCharC.CharacterIdx.Latest),
            };
        }

        public ToggleTreeViewNode GetPrompt(CharacterIdx index) => TemplateHelper.GetTemplate(index, characterList);
    }
}
