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
                CategoryName = string.IsNullOrEmpty(values[1]) ? null : values[1],
                DownloadableContentsName = string.IsNullOrEmpty(values[2]) ? null : values[2],
                AchievementName = string.IsNullOrEmpty(values[3]) ? null : values[3],
            };
        }

        public static void ExportToPostgres(List<SourceGames> sourceGames)
        {
            using var db = new GamesContext();
            var game = db.Games.FirstOrDefault();
            var categories = game.GamesCategories.Select(gameCategory => gameCategory.Category);
            var a = 5;
        }
    }
}
