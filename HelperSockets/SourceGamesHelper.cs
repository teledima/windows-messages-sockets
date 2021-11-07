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

        public static byte[] Encrypt(IEnumerable<SourceGames> sourceGames, byte[] public_key)
        {
            StringBuilder stringBuilder = new();
            foreach (SourceGames game in sourceGames)
            {
                if (game != null)
                    stringBuilder.AppendLine(game.ToString());
            }

            var desEncryptor = DES.Create();
            desEncryptor.GenerateKey();
            desEncryptor.GenerateIV();
            var des_transform = desEncryptor.CreateEncryptor(desEncryptor.Key, desEncryptor.IV);
            

            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, des_transform, CryptoStreamMode.Write);

            var content = Encoding.ASCII.GetBytes(stringBuilder.ToString());
            cStream.Write(content, 0, content.Length );
            cStream.FlushFinalBlock();

            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(Encoding.ASCII.GetString(public_key));
            var desKey = rsa.Encrypt(desEncryptor.Key, false);
            var desIV = rsa.Encrypt(desEncryptor.IV, false);

            var data = mStream.ToArray()
                        .Concat(desKey)
                        .Concat(desIV)
                        .ToArray();

            return data;
        }

        public static IEnumerable<SourceGames> Decrypt(byte[] content, byte[] key, byte[] iv)
        {
            var result = new List<SourceGames>();
            var desDecryptor = DES.Create();

            using var mStream = new MemoryStream(content);
            using var cStream = new CryptoStream(mStream, desDecryptor.CreateDecryptor(key, iv), CryptoStreamMode.Read);

            var decryptedContent = new byte[content.Length];
            cStream.Read(decryptedContent, 0, decryptedContent.Length);

            foreach (var game in Encoding.ASCII.GetString(decryptedContent).Split('\n'))
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

        public async static void ExportToPostgres(IEnumerable<SourceGames> sourceGames)
        {
            // export order: Category -> Game -> GameCategory -> Achievement -> DownloadableContent
            using var db = new GamesContext();
            foreach (var sourceGame in sourceGames) 
            {    
                Category category = default;

                if (!string.IsNullOrEmpty(sourceGame.CategoriesName))
                {
                    var categories = await db.Categories.ToListAsync();
                    category = categories.Find(category => category.Name.ToLower() == sourceGame.CategoriesName.ToLower());

                    // if we didn't find a category, insert a new one
                    if (category == null)
                    {
                        var add_result = await db.Categories.AddAsync(new Category() { Name = sourceGame.CategoriesName });
                        category = add_result.Entity;
                    }
                }

                // Game, GameCategory, Achievement, DownloadableContent depend on the Game key, so we check if there is a name else skip
                if (!string.IsNullOrEmpty(sourceGame.GamesName))
                {

                    var games = await db.Games.ToListAsync();
                    var game = games.Find(game => game.Name.ToLower() == sourceGame.GamesName.ToLower());

                    // if we didn't find a game, insert a new one
                    if (game == null)
                    {
                        var add_result = await db.Games.AddAsync(new Games() { Name = sourceGame.GamesName });
                        game = add_result.Entity;
                    }

                    var gameCategories = await db.GamesCategories
                                                .Include(gameCategory => gameCategory.Game)
                                                .Include(gameCategory => gameCategory.Category)
                                                .ToListAsync();

                    // if sourceGame.CategoriesName is null then category the variable will remain null and we will try to insert a new category, which will result in an error
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

                        // if we didn't find a achievement, insert a new one
                        if (achievement == null)
                            await db.Achievements.AddAsync(new Achievement() { Name = sourceGame.AchievementsName, Game = game });
                    }

                    if (!string.IsNullOrEmpty(sourceGame.DownloadableContentsName))
                    {
                        var downloadableContents = await db.DownloadableContents.ToListAsync();
                        var downloadableContent = downloadableContents.Find(downloadableContent => downloadableContent.Name.ToLower() == sourceGame.DownloadableContentsName.ToLower());

                        // if we didn't find a downloadable content, insert a new one
                        if (downloadableContent == null)
                            await db.DownloadableContents.AddAsync(new DownloadableContents() { Name = sourceGame.DownloadableContentsName, Game = game });
                    }
                }
                // save changes for each row
                await db.SaveChangesAsync();
            }
        }

        public static void ClearSourceGames(string filepath)
        {
            var db = new SourceGamesContext(filepath);
            db.SourceGames.RemoveRange(db.SourceGames);
            db.SaveChanges();
        }
    }
}
