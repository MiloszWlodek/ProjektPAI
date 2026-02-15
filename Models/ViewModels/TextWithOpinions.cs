namespace InfoInfo.Models.ViewModels
{
    public class TextWithOpinions
    {
        public Text SelectedText { get; set; }
        public int ReadingTime { get; set; }
        public int CommentsCount { get; set; }
        public int RatingsCount { get; set; }
        public float AverageRating { get; set; }
        public string Description { get; set; }

        public Opinion NewOpinion { get; set; }

        public TextWithOpinions()
        {
            SelectedText = new Text();
            ReadingTime = 0;
            CommentsCount = 0;
            RatingsCount = 0;
            AverageRating = 0f;
            Description = string.Empty;
        }
    }
}