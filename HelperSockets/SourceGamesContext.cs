﻿using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace HelperSockets
{
    public class SourceGamesContext : DbContext
    {
        public DbSet<SourceGames> SourceGames { get; set; }
        public string Filepath { get; private set; }
        public SourceGamesContext(string filepath) : base()
        {
            Filepath = filepath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(string.Format("Data Source = {0}", Filepath))
                .UseSnakeCaseNamingConvention();
        }
    }


    public class SourceGames
    {
        public string GamesName { get; set; }

        public string CategoryName { get; set; }

        public string DownloadableContentsName { get; set; }

        public string AchievementName { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            foreach (PropertyInfo property in typeof(SourceGames).GetProperties())
            {
                stringBuilder.Append(property.GetValue(this)?.ToString() + ";");
            }
            stringBuilder.Replace(";", "", stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}
