using HUDBlitz.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HUDBlitz.Models.Player;

namespace HUDBlitz.Commands.WarGame
{
    public class API
    {
        // Метод возвращает информацию об игроке.
        public static async Task GetAccountInfo(string application_id, string account_id, string access_token, string region)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://api.wotblitz.{region}/");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("application_id", application_id),
                    new KeyValuePair<string, string>("account_id", account_id),
                    new KeyValuePair<string, string>("access_token", access_token),
                    new KeyValuePair<string, string>("extra", "statistics.rating")
                });

                var response = await client.PostAsync("/wotb/account/info/", content);
                string json = await response.Content.ReadAsStringAsync();

                string original = json;
                string substring = account_id.ToString();
                int i = original.IndexOf(substring);
                string resultJson = original.Remove(i, substring.Length).Insert(i, "account");

                GlobalVariables.Response_WG = JsonConvert.DeserializeObject<Response>(resultJson);
                GlobalVariables.Response_WG_Static = File.Exists($"{account_id}.json") ?
                    JsonConvert.DeserializeObject<Response>(File.ReadAllText($"{account_id}.json")) :
                    JsonConvert.DeserializeObject<Response>(resultJson);

                while (GlobalVariables.check)
                {
                    File.WriteAllText($"{account_id}.json", JsonConvert.SerializeObject(GlobalVariables.Response_WG));
                    GlobalVariables.check = false;
                }

                if (GlobalVariables.Response_WG.data.account.statistics.rating.battles > GlobalVariables.Response_WG_Static.data.account.statistics.rating.battles)
                {
                    GlobalVariables.battleType = BattleType.RATING;
                    CalculateStatistics(
                        GlobalVariables.battleType,
                        GlobalVariables.Response_WG.data.account.statistics.rating.wins,                    GlobalVariables.Response_WG_Static.data.account.statistics.rating.wins,
                        GlobalVariables.Response_WG.data.account.statistics.rating.battles,                 GlobalVariables.Response_WG_Static.data.account.statistics.rating.battles,
                        GlobalVariables.Response_WG.data.account.statistics.rating.frags,                   GlobalVariables.Response_WG_Static.data.account.statistics.rating.frags,
                        GlobalVariables.Response_WG.data.account.statistics.rating.survived_battles,        GlobalVariables.Response_WG_Static.data.account.statistics.rating.survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.rating.hits,                    GlobalVariables.Response_WG_Static.data.account.statistics.rating.hits,
                        GlobalVariables.Response_WG.data.account.statistics.rating.shots,                   GlobalVariables.Response_WG_Static.data.account.statistics.rating.shots,
                        GlobalVariables.Battles - GlobalVariables.Survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.rating.damage_dealt,            GlobalVariables.Response_WG_Static.data.account.statistics.rating.damage_dealt,
                        GlobalVariables.Response_WG.data.account.statistics.rating.damage_received,         GlobalVariables.Response_WG_Static.data.account.statistics.rating.damage_received,
                        GlobalVariables.Response_WG.data.account.statistics.rating.spotted,                 GlobalVariables.Response_WG_Static.data.account.statistics.rating.spotted,
                        GlobalVariables.Response_WG.data.account.statistics.rating.dropped_capture_points,  GlobalVariables.Response_WG_Static.data.account.statistics.rating.dropped_capture_points,
                        GlobalVariables.Response_WG.data.account.statistics.rating.capture_points,          GlobalVariables.Response_WG_Static.data.account.statistics.rating.capture_points);
                }
                else if (GlobalVariables.Response_WG.data.account.statistics.all.battles > GlobalVariables.Response_WG_Static.data.account.statistics.all.battles)
                {
                    GlobalVariables.battleType = BattleType.ORDINARY;
                                        CalculateStatistics(
                        GlobalVariables.battleType,
                        GlobalVariables.Response_WG.data.account.statistics.all.wins,                    GlobalVariables.Response_WG_Static.data.account.statistics.all.wins,
                        GlobalVariables.Response_WG.data.account.statistics.all.battles,                 GlobalVariables.Response_WG_Static.data.account.statistics.all.battles,
                        GlobalVariables.Response_WG.data.account.statistics.all.frags,                   GlobalVariables.Response_WG_Static.data.account.statistics.all.frags,
                        GlobalVariables.Response_WG.data.account.statistics.all.survived_battles,        GlobalVariables.Response_WG_Static.data.account.statistics.all.survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.all.hits,                    GlobalVariables.Response_WG_Static.data.account.statistics.all.hits,
                        GlobalVariables.Response_WG.data.account.statistics.all.shots,                   GlobalVariables.Response_WG_Static.data.account.statistics.all.shots,
                        GlobalVariables.Battles - GlobalVariables.Survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.all.damage_dealt,            GlobalVariables.Response_WG_Static.data.account.statistics.all.damage_dealt,
                        GlobalVariables.Response_WG.data.account.statistics.all.damage_received,         GlobalVariables.Response_WG_Static.data.account.statistics.all.damage_received,
                        GlobalVariables.Response_WG.data.account.statistics.all.spotted,                 GlobalVariables.Response_WG_Static.data.account.statistics.all.spotted,
                        GlobalVariables.Response_WG.data.account.statistics.all.dropped_capture_points,  GlobalVariables.Response_WG_Static.data.account.statistics.all.dropped_capture_points,
                        GlobalVariables.Response_WG.data.account.statistics.all.capture_points,          GlobalVariables.Response_WG_Static.data.account.statistics.all.capture_points);
                }
                else if(GlobalVariables.Response_WG.data.account.statistics.clan.battles > GlobalVariables.Response_WG_Static.data.account.statistics.clan.battles)
                {
                    GlobalVariables.battleType = BattleType.CLAN;
                                        CalculateStatistics(
                        GlobalVariables.battleType,
                        GlobalVariables.Response_WG.data.account.statistics.clan.wins,                    GlobalVariables.Response_WG_Static.data.account.statistics.clan.wins,
                        GlobalVariables.Response_WG.data.account.statistics.clan.battles,                 GlobalVariables.Response_WG_Static.data.account.statistics.clan.battles,
                        GlobalVariables.Response_WG.data.account.statistics.clan.frags,                   GlobalVariables.Response_WG_Static.data.account.statistics.clan.frags,
                        GlobalVariables.Response_WG.data.account.statistics.clan.survived_battles,        GlobalVariables.Response_WG_Static.data.account.statistics.clan.survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.clan.hits,                    GlobalVariables.Response_WG_Static.data.account.statistics.clan.hits,
                        GlobalVariables.Response_WG.data.account.statistics.clan.shots,                   GlobalVariables.Response_WG_Static.data.account.statistics.clan.shots,
                        GlobalVariables.Battles - GlobalVariables.Survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.clan.damage_dealt,            GlobalVariables.Response_WG_Static.data.account.statistics.clan.damage_dealt,
                        GlobalVariables.Response_WG.data.account.statistics.clan.damage_received,         GlobalVariables.Response_WG_Static.data.account.statistics.clan.damage_received,
                        GlobalVariables.Response_WG.data.account.statistics.clan.spotted,                 GlobalVariables.Response_WG_Static.data.account.statistics.clan.spotted,
                        GlobalVariables.Response_WG.data.account.statistics.clan.dropped_capture_points,  GlobalVariables.Response_WG_Static.data.account.statistics.clan.dropped_capture_points,
                        GlobalVariables.Response_WG.data.account.statistics.clan.capture_points,          GlobalVariables.Response_WG_Static.data.account.statistics.clan.capture_points);
                }
            }
        }

        public enum BattleType
        {
            UNKNOW, // ненайден
            ORDINARY, // обычный
            RATING, // рейтинговый
            CLAN, // клановый
        }

        public static void CalculateStatistics( 
            BattleType battleType,
            int updateWins, int staticWins,
            int updateBattles, int staticBattles,
            int updateFrags, int staticFrags,
            int updateSurvived_battles, int staticSurvived_battles,
            int updateHits, int staticHits,
            int updateShots, int statickShots,
            int updateDeaths,
            int updateDamage_dealt, int staticDamage_dealt,
            int updateDamage_received, int staticDamage_received,
            int updateSpotted, int staticSpotted,
            int updateDropped_capture_points, int staticDropped_capture_points,
            int updateCapture_points, int staticCapture_points)
        {

            try
            {
                switch (battleType)
                {
                    case BattleType.ORDINARY:
                        GlobalVariables.Wins = updateWins - staticWins;
                        GlobalVariables.Battles = updateBattles - staticBattles;
                        GlobalVariables.Frags = updateFrags - staticFrags;
                        GlobalVariables.Survived_battles = updateSurvived_battles - staticSurvived_battles;
                        GlobalVariables.Hits = updateHits - staticHits;
                        GlobalVariables.Shots = updateShots - statickShots;
                        GlobalVariables.Deaths = updateDeaths;
                        GlobalVariables.Damage_dealt = updateDamage_dealt - staticDamage_dealt;
                        GlobalVariables.Damage_received = updateDamage_received - staticDamage_received;
                        GlobalVariables.Spotted = updateSpotted - staticSpotted;
                        GlobalVariables.Dropped_capture_points = updateDropped_capture_points - staticDropped_capture_points;
                        GlobalVariables.Capture_points = updateCapture_points - staticCapture_points;
                        
                        GlobalVariables.WinRate = GlobalVariables.Wins / GlobalVariables.Battles * 100f;
                        break;
                    case BattleType.RATING:
                        GlobalVariables.Wins = updateWins - staticWins;
                        GlobalVariables.Battles = updateBattles - staticBattles;
                        GlobalVariables.Frags = updateFrags - staticFrags;
                        GlobalVariables.Survived_battles = updateSurvived_battles - staticSurvived_battles;
                        GlobalVariables.Hits = updateHits - staticHits;
                        GlobalVariables.Shots = updateShots - statickShots;
                        GlobalVariables.Deaths = updateDeaths;
                        GlobalVariables.Damage_dealt = updateDamage_dealt - staticDamage_dealt;
                        GlobalVariables.Damage_received = updateDamage_received - staticDamage_received;
                        GlobalVariables.Spotted = updateSpotted - staticSpotted;
                        GlobalVariables.Dropped_capture_points = updateDropped_capture_points - staticDropped_capture_points;
                        GlobalVariables.Capture_points = updateCapture_points - staticCapture_points;

                        GlobalVariables.WinRate = GlobalVariables.Wins / GlobalVariables.Battles * 100f;
                        break;
                    case BattleType.CLAN:
                        GlobalVariables.Wins = updateWins - staticWins;
                        GlobalVariables.Battles = updateBattles - staticBattles;
                        GlobalVariables.Frags = updateFrags - staticFrags;
                        GlobalVariables.Survived_battles = updateSurvived_battles - staticSurvived_battles;
                        GlobalVariables.Hits = updateHits - staticHits;
                        GlobalVariables.Shots = updateShots - statickShots;
                        GlobalVariables.Deaths = updateDeaths;
                        GlobalVariables.Damage_dealt = updateDamage_dealt - staticDamage_dealt;
                        GlobalVariables.Damage_received = updateDamage_received - staticDamage_received;
                        GlobalVariables.Spotted = updateSpotted - staticSpotted;
                        GlobalVariables.Dropped_capture_points = updateDropped_capture_points - staticDropped_capture_points;
                        GlobalVariables.Capture_points = updateCapture_points - staticCapture_points;

                        GlobalVariables.WinRate = GlobalVariables.Wins / GlobalVariables.Battles * 100f;
                        break;
                }
            }
            catch(Exception exc)
            {

            }
        }
    }
}
