﻿using System.Collections.Generic;
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
                if (game != null)
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

        public async static void ExportToPostgres(List<SourceGames> sourceGames)
        {
            using var db = new GamesContext();
            foreach (var sourceGame in sourceGames) 
            {    
                Category category = default;

                if (!string.IsNullOrEmpty(sourceGame.CategoriesName))
                {
                    var categories = await db.Categories.ToListAsync();
                    category = categories.Find(category => category.Name.ToLower() == sourceGame.CategoriesName.ToLower());

                    if (category == null)
                    {
                        var add_result = await db.Categories.AddAsync(new Category() { Name = sourceGame.CategoriesName });
                        category = add_result.Entity;
                    }
                }

                if (!string.IsNullOrEmpty(sourceGame.GamesName))
                {

                    var games = await db.Games.ToListAsync();
                    var game = games.Find(game => game.Name.ToLower() == sourceGame.GamesName.ToLower());

                    if (game == null)
                    {
                        var add_result = await db.Games.AddAsync(new Games() { Name = sourceGame.GamesName });
                        game = add_result.Entity;
                    }

                    var gameCategories = await db.GamesCategories
                                                .Include(gameCategory => gameCategory.Game)
                                                .Include(gameCategory => gameCategory.Category)
                                                .ToListAsync();

                    if (category != null)
                    {
                        var gameCategory = gameCategories.Find(gameCategory => gameCategory.Game == game && gameCategory.Category == category);

                        if (gameCategory == null)
                        {
                            var add_result = await db.GamesCategories.AddAsync(new GamesCategory() { Game = game, Category = category });
                            gameCategory = add_result.Entity;
                        }
                    }


                    if (!string.IsNullOrEmpty(sourceGame.AchievementsName))
                    {
                        var achievements = await db.Achievements.ToListAsync();
                        var achievement = achievements.Find(achievement => achievement.Name.ToLower() == sourceGame.AchievementsName.ToLower());

                        if (achievement == null)
                            await db.Achievements.AddAsync(new Achievement() { Name = sourceGame.AchievementsName, Game = game });
                    }

                    if (!string.IsNullOrEmpty(sourceGame.DownloadableContentsName))
                    {
                        var downloadableContents = await db.DownloadableContents.ToListAsync();
                        var downloadableContent = downloadableContents.Find(downloadableContent => downloadableContent.Name.ToLower() == sourceGame.DownloadableContentsName.ToLower());

                        if (downloadableContent == null)
                            await db.DownloadableContents.AddAsync(new DownloadableContents() { Name = sourceGame.DownloadableContentsName, Game = game });
                    }
                }
                await db.SaveChangesAsync();
            }
        }

        public static void ClearSourceGames(string filepath)
        {
            var db = new SourceGamesContext(filepath);
            db.SourceGames.RemoveRange(db.SourceGames);
            db.SaveChangesAsync();
        }
    }
}
