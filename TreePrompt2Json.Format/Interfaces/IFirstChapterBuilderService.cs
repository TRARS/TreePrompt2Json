using TreePrompt2Json.Format.DTOs;

namespace TreePrompt2Json.Format.Interfaces
{
    public interface IFirstChapterBuilderService
    {
        /// <summary>
        /// 获取 ChapterBuilder 对象的实例
        /// </summary>
        Chapter Build();
    }
}
