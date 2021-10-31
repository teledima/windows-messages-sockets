using Dapper;

namespace HelperSockets
{
    [Table("games")]
    public class PostgresGames
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
