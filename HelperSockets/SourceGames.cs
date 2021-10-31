namespace HelperSockets
{
    public class SourceGames
    {
        public int GamesId { get; set; }
        public string GamesName { get; set; }
        public int GamesCategoryGameId { get; set; }
        public int GamesCategoryCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int DownloadableContentsId { get; set; }
        public string DownloadableContentsName { get; set; }
        public int DownloadableContentsGameId { get; set; }
        public int AchievementId { get; set; }
        public string AchievementName { get; set; }
        public int AchievementGameId { get; set; }
    }
}
