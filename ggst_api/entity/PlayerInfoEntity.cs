
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ggst_api.entity
{
    [Table("tb_player_info")]
    public class PlayerInfoEntity
    {
        [NotMapped]
        public int? pos { get; set; }

        [Column("id")]
        public string id { get; set; }

        [Column("platform")]
        public string platform { get; set; }


        [Column("character")]
        public string character { get; set; }

        [Column("character_short")]
        public string character_short { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("game_count")]
        public int game_count { get; set; }

        [Column("rating_value")]
        public int rating_value { get; set; }

        [Column("rating_deviation")]
        public int rating_deviation { get; set; }

        [Column("vip_status")]
        public string? vip_status { get; set; }


        [Column("cheater_status")]
        public string? cheater_status { get; set; }

        [Column("hidden_status")]
        public string? hidden_status { get; set; }

        public virtual PlayerInfoEntity deepcopy() { 
            PlayerInfoEntity entity = new PlayerInfoEntity();
            entity.pos = pos;
            entity.id = id;
            entity.platform = platform;
            entity.character = character;
            entity.character_short = character_short;
            entity.name = name;
            entity.game_count = game_count;
            entity.rating_value = rating_value;
            entity.rating_deviation = rating_deviation;
            entity.hidden_status = hidden_status;
            entity.vip_status = vip_status;
            entity.cheater_status = cheater_status;
            return entity;
        }

        public static List<PlayerInfoEntity> decodeFromString(string decodeString) {
            try
            {
                List<PlayerInfoEntity> totalList = JsonSerializer.Deserialize<List<PlayerInfoEntity>>(decodeString);
                return totalList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string encodeFromList(List<PlayerInfoEntity> lists) { 
            return JsonSerializer.Serialize(lists);
        }

        

    }
}
