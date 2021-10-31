using Dapper;

namespace HelperSockets
{
    [Table("category")]
    public class PostgresCategory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
