using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HelperSockets
{
    public static class SourceGamesHelper
    {
        public async static Task<IList<SourceGames>> GetSource(string filepath)
        {
            using var db = new SourceGamesContext(filepath);

            return await db.SourceGames.ToListAsync();
        }

        public static byte[] GetBytes(IEnumerable<SourceGames> sourceGames)
        {
            StringBuilder stringBuilder = new();
            foreach (SourceGames game in sourceGames)
            {
                stringBuilder.AppendLine(game.ToString());
            }
            stringBuilder.Append("<EOF>");
            return Encoding.ASCII.GetBytes(stringBuilder.ToString()); ;
        }

        public static SourceGames FromString(string data)
        {
            string[] values = data.Split(';');
            return new SourceGames()
            {
                GamesName = string.IsNullOrEmpty(values[0]) ? null : values[0],
                CategoriesName = string.IsNullOrEmpty(values[1]) ? null : values[1],
                DownloadableContentsName = string.IsNullOrEmpty(values[2]) ? null : values[2],
                AchievementsName = string.IsNullOrEmpty(values[3]) ? null : values[3],
            };
        }

        public static void ExportToPostgres(List<SourceGames> sourceGames)
        {
            using var db = new GamesContext();

            foreach (var sourceGame in sourceGames) 
            {
                Category category = new();
                if (!string.IsNullOrEmpty(sourceGame.CategoriesName))
                {
                    category = db.Categories.ToList().Find(category => category.Name == sourceGame.CategoriesName);

                    if (category == null)
                        category = db.Categories.Add(new Category() { Name = sourceGame.CategoriesName }).Entity;
                }

                if (string.IsNullOrEmpty(sourceGame.GamesName))
                    continue;

                var game = db.Games.ToList().Find(game => game.Name == sourceGame.GamesName);

                if (game == null)
                    game = db.Games.Add(new Games() { Name = sourceGame.GamesName }).Entity;

                // db.SaveChanges();

                var gameCategory = db.GamesCategories
                    .Include(gameCategory => gameCategory.Game)
                    .Include(gameCategory => gameCategory.Category)
                    .ToList()
                    .Find(gameCategory => gameCategory.Game == game && gameCategory.Category == category);

                if (gameCategory == null)
                    gameCategory = db.GamesCategories.Add(new GamesCategory() { Game = game, Category = category }).Entity;


                if (!string.IsNullOrEmpty(sourceGame.AchievementsName))
                {
                    var achievement = db.Achievements.ToList().Find(achievement => achievement.Name == sourceGame.AchievementsName);

                    if (achievement == null)
                        db.Achievements.Add(new Achievement() { Name = sourceGame.AchievementsName, Game = game });
                }

                if (!string.IsNullOrEmpty(sourceGame.DownloadableContentsName))
                {
                    var downloadableContent = db.DownloadableContents.ToList().Find(downloadableContent => downloadableContent.Name == sourceGame.DownloadableContentsName);

                    if (downloadableContent == null)
                        db.DownloadableContents.Add(new DownloadableContents() { Name = sourceGame.DownloadableContentsName, Game = game });
                }

                db.SaveChanges();
            }

            db.SaveChanges();
        }
    }
}
