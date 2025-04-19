using CommunityToolkit.Mvvm.ComponentModel;
using TreePrompt2Json.Format.DTOs;
using TreePrompt2Json.Format.Interfaces;

namespace TreePrompt2Json.Format.Services
{
    public partial class FirstChapterBuilderService : ObservableObject, IFirstChapterBuilderService
    {
        private Chapter temp;

        public FirstChapterBuilderService()
        {
            temp = new();
        }
    }

    public partial class FirstChapterBuilderService
    {
        public void SetName(string name) => temp.Name = name;
        public void SetYears(string years) => temp.Years = years;
        public void SetLocation(string location) => temp.Location = location;
        public void SetEvent(string @event) => temp.Event = @event;

        public Chapter Build()
        {
            return temp;
        }
    }
}
