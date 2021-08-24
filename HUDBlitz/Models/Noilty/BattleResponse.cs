using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HUDBlitz.Commands.WarGame.API;

namespace HUDBlitz.Models.Noilty
{
    [Serializable]
    public class BattleResponse
    {
        public Battle battle { get; set; }
        public string auth_token { get; set; }
        public string password { get; set; }
    }

    [Serializable]
    public class Battle
    {
        public string user_id { get; set; }
        public string account_id { get; set; }
        public string fraction_id { get; set; }
        public string wg_region { get; set; }
        public string battle_type_id { get; set; }
        public string damage_blocked { get; set; }
        public string current_durability_tank { get; set; }
        public string capture_points { get; set; }
        public string damage_dealt { get; set; }
        public string damage_received { get; set; }
        public string dropped_capture_points { get; set; }
        public string frags { get; set; }
        public string frags8p { get; set; }
        public string hits { get; set; }
        public string losses { get; set; }
        public string max_frags { get; set; }
        public string max_xp { get; set; }
        public string shots { get; set; }
        public string spotted { get; set; }
        public string survived_battles { get; set; }
        public string win_and_survived { get; set; }
        public string wins { get; set; }
        public string xp { get; set; }
        public string credits { get; set; }
        public string gold { get; set; }
        public string free_xp { get; set; }
        public string battle_life_time { get; set; }
        public string is_premium { get; set; }
    }
}
