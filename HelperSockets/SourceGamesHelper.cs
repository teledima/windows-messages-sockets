using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Dapper;
using System.Collections.Generic;
using System.Text;

namespace HelperSockets
{
    public static class SourceGamesHelper
    {
        public async static Task<IEnumerable<SourceGames>> GetSource(string filepath)
        {
            using var connection = new SqliteConnection(string.Format("Data Source={0};Mode=ReadWrite", filepath));
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
            await connection.OpenAsync();

            return await connection.GetListAsync<SourceGames>();
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
                GamesId = int.Parse(values[0]),
                GamesName = values[1],
                GamesCategoryGameId = int.Parse(values[2]),
                GamesCategoryCategoryId = int.Parse(values[3]),
                CategoryId = int.Parse(values[4]),
                CategoryName = values[5],
                DownloadableContentsId = string.IsNullOrEmpty(values[6]) ? null : int.Parse(values[6]),
                DownloadableContentsName = string.IsNullOrEmpty(values[7]) ? null : values[7],
                DownloadableContentsGameId = string.IsNullOrEmpty(values[8]) ? null : int.Parse(values[8]),
                AchievementId = string.IsNullOrEmpty(values[9]) ? null : int.Parse(values[9]),
                AchievementName = string.IsNullOrEmpty(values[10]) ? null : values[10],
                AchievementGameId = string.IsNullOrEmpty(values[11]) ? null : int.Parse(values[11])
            };
        }
    }
}
