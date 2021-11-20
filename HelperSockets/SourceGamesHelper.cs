using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System;

namespace HelperSockets
{
    public static class SourceGamesHelper
    {
        public static IEnumerable<SourceGames> GetSource(string filepath)
        {
            using var db = new SourceGamesContext(filepath);
            return db.SourceGames.ToList().Where(game => game != null);
        }

        public static byte[] Encrypt(IEnumerable<SourceGames> sourceGames, AesService desService)
        {
            StringBuilder stringBuilder = new();
            foreach (SourceGames game in sourceGames)
            {
                if (game != null)
                    stringBuilder.AppendLine(game.ToString());
            }

            return desService.Encrypt(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        public static IEnumerable<SourceGames> Parse(byte[] content)
        {
            var result = new List<SourceGames>();
            foreach (var game in Encoding.ASCII.GetString(content).Split('\n'))
            {
                var sourceGame = FromString(game.Replace("\r", ""));
                if (sourceGame != null)
                    result.Add(sourceGame);
            }
            
            return result;
        }

        public static SourceGames FromString(string data)
        {
            try
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
            catch(Exception)
            {
                return null;
            }
        }

        public static void ExportToPostgres(IEnumerable<SourceGames> sourceGames)
        {
            // export order: Category -> Game -> GameCategory -> Achievement -> DownloadableContent
            using var db = new GamesContext();
            foreach (var sourceGame in sourceGames) 
            {    
                Category category = default;

                if (!string.IsNullOrEmpty(sourceGame.CategoriesName))
                {
                    var categories = db.Categories.ToList();
                    category = categories.Find(category => category.Name.ToLower() == sourceGame.CategoriesName.ToLower());

                    // if we didn't find a category, insert a new one
                    if (category == null)
                    {
                        var add_result = db.Categories.Add(new Category() { Name = sourceGame.CategoriesName });
                        category = add_result.Entity;
                    }
                }

                // Game, GameCategory, Achievement, DownloadableContent depend on the Game key, so we check if there is a name else skip
                if (!string.IsNullOrEmpty(sourceGame.GamesName))
                {

                    var games = db.Games.ToList();
                    var game = games.Find(game => game.Name.ToLower() == sourceGame.GamesName.ToLower());

                    // if we didn't find a game, insert a new one
                    if (game == null)
                    {
                        var add_result = db.Games.Add(new Games() { Name = sourceGame.GamesName });
                        game = add_result.Entity;
                    }

                    var gameCategories = db.GamesCategories
                                                .Include(gameCategory => gameCategory.Game)
                                                .Include(gameCategory => gameCategory.Category)
                                                .ToList();

                    // if sourceGame.CategoriesName is null then category the variable will remain null and we will try to insert a new category, which will result in an error
                    if (category != null)
                    {
                        var gameCategory = gameCategories.Find(gameCategory => gameCategory.Game == game && gameCategory.Category == category);

                        if (gameCategory == null)
                        {
                            var add_result = db.GamesCategories.Add(new GamesCategory() { Game = game, Category = category });
                            gameCategory = add_result.Entity;
                        }
                    }


                    if (!string.IsNullOrEmpty(sourceGame.AchievementsName))
                    {
                        var achievements = db.Achievements.ToList();
                        var achievement = achievements.Find(achievement => achievement.Name.ToLower() == sourceGame.AchievementsName.ToLower());

                        // if we didn't find a achievement, insert a new one
                        if (achievement == null)
                            db.Achievements.Add(new Achievement() { Name = sourceGame.AchievementsName, Game = game });
                    }

                    if (!string.IsNullOrEmpty(sourceGame.DownloadableContentsName))
                    {
                        var downloadableContents = db.DownloadableContents.ToList();
                        var downloadableContent = downloadableContents.Find(downloadableContent => downloadableContent.Name.ToLower() == sourceGame.DownloadableContentsName.ToLower());

                        // if we didn't find a downloadable content, insert a new one
                        if (downloadableContent == null)
                            db.DownloadableContents.Add(new DownloadableContents() { Name = sourceGame.DownloadableContentsName, Game = game });
                    }
                }
                // save changes for each row
                db.SaveChanges();
            }
        }

        public static void ClearSourceGames(string filepath, IEnumerable<SourceGames> sourceGames = null)
        {
            var db = new SourceGamesContext(filepath);
            if (sourceGames == null)
                db.SourceGames.RemoveRange(db.SourceGames);
            else
                db.SourceGames.RemoveRange(sourceGames);
            db.SaveChanges();
        }
    }
}
