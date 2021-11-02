using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HelperSockets
{
    public class GamesContext: DbContext
    {
        public DbSet<Games> Games;
        public DbSet<Category> Categories;
        public DbSet<GamesCategory> GamesCategories;
        public DbSet<Achievement> Achievements;
        public DbSet<DownloadableContents> DownloadableContents;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(Properties.Settings.Default["postgres_connection_string"].ToString())
                .UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary keys
            modelBuilder.Entity<Games>()
                .HasKey(game => game.Id);

            modelBuilder.Entity<Category>()
                .HasKey(category => category.Id);

            modelBuilder.Entity<Achievement>()
                .HasKey(achievement => achievement.Id);

            modelBuilder.Entity<DownloadableContents>()
                .HasKey(downloadableContent => downloadableContent.Id);

            modelBuilder.Entity<GamesCategory>()
                .HasKey(gamesCategory => new { gamesCategory.GameId, gamesCategory.CategoryId });

            // Indexes
            modelBuilder.Entity<Games>()
                .HasIndex(game => game.Name)
                .HasName("games_name_uniq")
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(category => category.Name)
                .HasName("category_name_uniq")
                .IsUnique();

            modelBuilder.Entity<Achievement>()
                .HasIndex(achievement => achievement.Name)
                .HasName("achievement_name_uniq")
                .IsUnique();

            modelBuilder.Entity<DownloadableContents>()
                .HasIndex(downloadableContent => downloadableContent.Name)
                .HasName("downloadable_contens_name_uniq")
                .IsUnique();

            // Relations
            modelBuilder.Entity<GamesCategory>()
                .HasOne(gameCategory => gameCategory.Game)
                .WithMany(game => game.GamesCategories)
                .HasForeignKey(gameCategory => gameCategory.GameId);

            modelBuilder.Entity<GamesCategory>()
                .HasOne(gameCategory => gameCategory.Category)
                .WithMany(category => category.GamesCategories)
                .HasForeignKey(gameCategory => gameCategory.CategoryId);

            modelBuilder.Entity<Achievement>()
                .HasOne(achievement => achievement.Game)
                .WithMany(game => game.Achievements)
                .HasForeignKey(achievement => achievement.GameId);

            modelBuilder.Entity<DownloadableContents>()
                .HasOne(downloadableContent => downloadableContent.Game)
                .WithMany(game => game.DownloadableContents)
                .HasForeignKey(downloadableContent => downloadableContent.GameId);
        }
    }
    public class Games
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<GamesCategory> GamesCategories { get; set; }
        public List<Achievement> Achievements { get; set; }
        public List<DownloadableContents> DownloadableContents { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Games> Games { get; set; }

        public List<GamesCategory> GamesCategories { get; set; }
    }
    public class GamesCategory
    {
        public int GameId { get; set; }
        public Games Game { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GameId { get; set; }
        public Games Game { get; set; }
    }
    public class DownloadableContents
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GameId { get; set; }
        public Games Game { get; set; }
    }
}
