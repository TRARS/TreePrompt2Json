namespace TreePrompt2Json.Format.DTOs
{
    public sealed class Chapter
    {
        /// <summary>
        /// 章节名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 章节年份
        /// </summary>
        public string Years { get; set; }

        /// <summary>
        /// 章节地点
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 章节事件
        /// </summary>
        public string @Event { get; set; }
    }
}
