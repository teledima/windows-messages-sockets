﻿using Dapper;

namespace HelperSockets
{
    [Table("downloadable_contents")]
    public class PostgresDownloadableContents
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("game_id")]
        public int GameId { get; set; }
    }
}
