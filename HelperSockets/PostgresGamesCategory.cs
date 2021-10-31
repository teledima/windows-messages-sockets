using Dapper;

namespace HelperSockets
{
    [Table("games_category")]
    public class PostgresGamesCategory
    {
        [Key]
        [Column("game_id")]
        public int GameId { get; set; }
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }
    }
}
