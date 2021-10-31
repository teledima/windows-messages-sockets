using Dapper;
using System.Reflection;
using System.Text;

namespace HelperSockets
{
    [Table("source")]
    public class SourceGames
    {
        [Column("games_id")]
        public int GamesId { get; set; }

        [Column("games_name")]
        public string GamesName { get; set; }

        [Column("games_category_game_id")]
        public int GamesCategoryGameId { get; set; }

        [Column("games_category_category_id")]
        public int GamesCategoryCategoryId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("category_name")]
        public string CategoryName { get; set; }

        [Column("downloadable_contents_id")]
        public int? DownloadableContentsId { get; set; }

        [Column("downloadable_contents_name")]
        public string DownloadableContentsName { get; set; }

        [Column("downloadable_contents_game_id")]
        public int? DownloadableContentsGameId { get; set; }

        [Column("achievement_id")]
        public int? AchievementId { get; set; }

        [Column("achievement_name")]
        public string AchievementName { get; set; }

        [Column("achievement_game_id")]
        public int? AchievementGameId { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            foreach (PropertyInfo property in typeof(SourceGames).GetProperties())
            {
                stringBuilder.Append(property.GetValue(this)?.ToString()+";");
            }
            stringBuilder.Replace(";", "", stringBuilder.Length-1, 1);
            return stringBuilder.ToString();
        }
    }
}
