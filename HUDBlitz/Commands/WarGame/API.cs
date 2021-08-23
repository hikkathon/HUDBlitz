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

                GlobalVariables.Response_WG = JsonConvert.DeserializeObject<Models.Player.Response>(resultJson);
                GlobalVariables.Response_WG_Static = File.Exists($"{account_id}.json") ?
                    JsonConvert.DeserializeObject<Models.Player.Response>(File.ReadAllText($"{account_id}.json")) :
                    JsonConvert.DeserializeObject<Models.Player.Response>(resultJson);

                GlobalVariables.Response_WG_Send = File.Exists($"{account_id}_send.json") ?
                    JsonConvert.DeserializeObject<Models.Player.Response>(File.ReadAllText($"{account_id}_send.json")) :
                    JsonConvert.DeserializeObject<Models.Player.Response>(resultJson);

                while (GlobalVariables.check)
                {
                    File.WriteAllText($"{account_id}.json", JsonConvert.SerializeObject(GlobalVariables.Response_WG));
                    GlobalVariables.check = false;
                }

                while (GlobalVariables.IsSendNoilty)
                {
                    File.WriteAllText($"{account_id}_send.json", JsonConvert.SerializeObject(GlobalVariables.Response_WG_Send));
                    GlobalVariables.IsSendNoilty = false;
                }

                if (GlobalVariables.Response_WG.data.account.statistics.rating.battles > GlobalVariables.Response_WG_Static.data.account.statistics.rating.battles)
                {
                    GlobalVariables.battleType = BattleType.RATING;
                    CalculateStatistics(
                        GlobalVariables.battleType,
                        GlobalVariables.Response_WG.data.account.statistics.rating.wins, GlobalVariables.Response_WG_Static.data.account.statistics.rating.wins,
                        GlobalVariables.Response_WG.data.account.statistics.rating.battles, GlobalVariables.Response_WG_Static.data.account.statistics.rating.battles,
                        GlobalVariables.Response_WG.data.account.statistics.rating.frags, GlobalVariables.Response_WG_Static.data.account.statistics.rating.frags,
                        GlobalVariables.Response_WG.data.account.statistics.rating.survived_battles, GlobalVariables.Response_WG_Static.data.account.statistics.rating.survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.rating.hits, GlobalVariables.Response_WG_Static.data.account.statistics.rating.hits,
                        GlobalVariables.Response_WG.data.account.statistics.rating.shots, GlobalVariables.Response_WG_Static.data.account.statistics.rating.shots,
                        GlobalVariables.Battles - GlobalVariables.Survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.rating.damage_dealt, GlobalVariables.Response_WG_Static.data.account.statistics.rating.damage_dealt,
                        GlobalVariables.Response_WG.data.account.statistics.rating.damage_received, GlobalVariables.Response_WG_Static.data.account.statistics.rating.damage_received,
                        GlobalVariables.Response_WG.data.account.statistics.rating.spotted, GlobalVariables.Response_WG_Static.data.account.statistics.rating.spotted,
                        GlobalVariables.Response_WG.data.account.statistics.rating.dropped_capture_points, GlobalVariables.Response_WG_Static.data.account.statistics.rating.dropped_capture_points,
                        GlobalVariables.Response_WG.data.account.statistics.rating.capture_points, GlobalVariables.Response_WG_Static.data.account.statistics.rating.capture_points);
                }
                else if (GlobalVariables.Response_WG.data.account.statistics.all.battles > GlobalVariables.Response_WG_Static.data.account.statistics.all.battles)
                {
                    GlobalVariables.battleType = BattleType.ORDINARY;
                    CalculateStatistics(
                        GlobalVariables.battleType,
                        GlobalVariables.Response_WG.data.account.statistics.all.wins, GlobalVariables.Response_WG_Static.data.account.statistics.all.wins,
                        GlobalVariables.Response_WG.data.account.statistics.all.battles, GlobalVariables.Response_WG_Static.data.account.statistics.all.battles,
                        GlobalVariables.Response_WG.data.account.statistics.all.frags, GlobalVariables.Response_WG_Static.data.account.statistics.all.frags,
                        GlobalVariables.Response_WG.data.account.statistics.all.survived_battles, GlobalVariables.Response_WG_Static.data.account.statistics.all.survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.all.hits, GlobalVariables.Response_WG_Static.data.account.statistics.all.hits,
                        GlobalVariables.Response_WG.data.account.statistics.all.shots, GlobalVariables.Response_WG_Static.data.account.statistics.all.shots,
                        GlobalVariables.Battles - GlobalVariables.Survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.all.damage_dealt, GlobalVariables.Response_WG_Static.data.account.statistics.all.damage_dealt,
                        GlobalVariables.Response_WG.data.account.statistics.all.damage_received, GlobalVariables.Response_WG_Static.data.account.statistics.all.damage_received,
                        GlobalVariables.Response_WG.data.account.statistics.all.spotted, GlobalVariables.Response_WG_Static.data.account.statistics.all.spotted,
                        GlobalVariables.Response_WG.data.account.statistics.all.dropped_capture_points, GlobalVariables.Response_WG_Static.data.account.statistics.all.dropped_capture_points,
                        GlobalVariables.Response_WG.data.account.statistics.all.capture_points, GlobalVariables.Response_WG_Static.data.account.statistics.all.capture_points);
                }
                else if (GlobalVariables.Response_WG.data.account.statistics.clan.battles > GlobalVariables.Response_WG_Static.data.account.statistics.clan.battles)
                {
                    GlobalVariables.battleType = BattleType.CLAN;
                    CalculateStatistics(
                        GlobalVariables.battleType,
                        GlobalVariables.Response_WG.data.account.statistics.clan.wins, GlobalVariables.Response_WG_Static.data.account.statistics.clan.wins,
                        GlobalVariables.Response_WG.data.account.statistics.clan.battles, GlobalVariables.Response_WG_Static.data.account.statistics.clan.battles,
                        GlobalVariables.Response_WG.data.account.statistics.clan.frags, GlobalVariables.Response_WG_Static.data.account.statistics.clan.frags,
                        GlobalVariables.Response_WG.data.account.statistics.clan.survived_battles, GlobalVariables.Response_WG_Static.data.account.statistics.clan.survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.clan.hits, GlobalVariables.Response_WG_Static.data.account.statistics.clan.hits,
                        GlobalVariables.Response_WG.data.account.statistics.clan.shots, GlobalVariables.Response_WG_Static.data.account.statistics.clan.shots,
                        GlobalVariables.Battles - GlobalVariables.Survived_battles,
                        GlobalVariables.Response_WG.data.account.statistics.clan.damage_dealt, GlobalVariables.Response_WG_Static.data.account.statistics.clan.damage_dealt,
                        GlobalVariables.Response_WG.data.account.statistics.clan.damage_received, GlobalVariables.Response_WG_Static.data.account.statistics.clan.damage_received,
                        GlobalVariables.Response_WG.data.account.statistics.clan.spotted, GlobalVariables.Response_WG_Static.data.account.statistics.clan.spotted,
                        GlobalVariables.Response_WG.data.account.statistics.clan.dropped_capture_points, GlobalVariables.Response_WG_Static.data.account.statistics.clan.dropped_capture_points,
                        GlobalVariables.Response_WG.data.account.statistics.clan.capture_points, GlobalVariables.Response_WG_Static.data.account.statistics.clan.capture_points);
                }

                if (GlobalVariables.Response_WG.data.account.statistics.rating.battles > GlobalVariables.Response_WG_Send.data.account.statistics.rating.battles)
                {
                    string _user_id = GlobalVariables.response_Noilty.data.account.user_id.ToString();
                    string _account_id = GlobalVariables.Response_WG.data.account.account_id.ToString();
                    string _fraction_id = GlobalVariables.response_Noilty.data.account.fraction_id.ToString();
                    string _wg_region = GlobalVariables.response_Noilty.data.user.wg_region;
                    string _battle_type_id = ((int)GlobalVariables.battleType).ToString();
                    string _damage_blocked = GlobalVariables.MaxReceived.ToString();
                    string _tank_durability = GlobalVariables.Strength.ToString();
                    string _capture_points = (GlobalVariables.Response_WG.data.account.statistics.rating.battles - GlobalVariables.Response_WG_Send.data.account.statistics.rating.battles).ToString();
                    string _damage_dealt = (GlobalVariables.Response_WG.data.account.statistics.rating.damage_dealt - GlobalVariables.Response_WG_Send.data.account.statistics.rating.damage_dealt).ToString();
                    string _damage_received = (GlobalVariables.Response_WG.data.account.statistics.rating.damage_received - GlobalVariables.Response_WG_Send.data.account.statistics.rating.damage_received).ToString();
                    string _dropped_capture_points = (GlobalVariables.Response_WG.data.account.statistics.rating.dropped_capture_points - GlobalVariables.Response_WG_Send.data.account.statistics.rating.dropped_capture_points).ToString();
                    string _frags = (GlobalVariables.Response_WG.data.account.statistics.rating.frags - GlobalVariables.Response_WG_Send.data.account.statistics.rating.frags).ToString();
                    string _hits = (GlobalVariables.Response_WG.data.account.statistics.rating.hits - GlobalVariables.Response_WG_Send.data.account.statistics.rating.hits).ToString();
                    string _losses = (GlobalVariables.Response_WG.data.account.statistics.rating.losses - GlobalVariables.Response_WG_Send.data.account.statistics.rating.losses).ToString();
                    string _max_frags = "0";
                    string _max_xp = "0";
                    string _shots = (GlobalVariables.Response_WG.data.account.statistics.rating.shots - GlobalVariables.Response_WG_Send.data.account.statistics.rating.shots).ToString();
                    string _spotted = (GlobalVariables.Response_WG.data.account.statistics.rating.spotted - GlobalVariables.Response_WG_Send.data.account.statistics.rating.spotted).ToString();
                    string _survived_battles = (GlobalVariables.Response_WG.data.account.statistics.rating.survived_battles - GlobalVariables.Response_WG_Send.data.account.statistics.rating.survived_battles).ToString();
                    string _win_and_survived = (GlobalVariables.Response_WG.data.account.statistics.rating.win_and_survived - GlobalVariables.Response_WG_Send.data.account.statistics.rating.win_and_survived).ToString();
                    string _wins = (GlobalVariables.Response_WG.data.account.statistics.rating.wins - GlobalVariables.Response_WG_Send.data.account.statistics.rating.wins).ToString();
                    string _credits = "0";
                    string _gold = "0";
                    string _free_xp = "0";
                    string _battle_life_time = "0";
                    string _is_premium = "0"; 
                    
                    await SendData(_user_id,_account_id,_fraction_id,_wg_region,_battle_type_id,_damage_blocked,_tank_durability,_capture_points,_damage_dealt,_damage_received,_dropped_capture_points,
                        _frags,_hits,_losses,_max_frags, _max_xp,_shots,_spotted,_survived_battles,_win_and_survived,_wins,_credits,_gold,_free_xp,_battle_life_time,_is_premium);
                    GlobalVariables.IsSendNoilty = true;
                }
                else if (GlobalVariables.Response_WG.data.account.statistics.all.battles > GlobalVariables.Response_WG_Send.data.account.statistics.all.battles)
                {

                }
                else if (GlobalVariables.Response_WG.data.account.statistics.clan.battles > GlobalVariables.Response_WG_Send.data.account.statistics.clan.battles)
                {

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
            catch (Exception exc)
            {

            }
        }

        public static async Task SendData(
            string user_id,
            string account_id,
            string fraction_id, 
            string wg_region, 
            string battle_type_id, 
            string damage_blocked, 
            string tank_durability, 
            string capture_points,
            string damage_dealt, 
            string damage_received, 
            string dropped_capture_points, 
            string frags, 
            string hits,
            string losses,
            string max_frags,
            string max_xp,
            string shots,
            string spotted,
            string survived_battles,
            string win_and_survived,
            string wins,
            string credits,
            string gold,
            string free_xp,
            string battle_life_time,
            string is_premium)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://blitzbury.noilty.loc");

                GlobalVariables.battleResponse = new Models.Noilty.BattleResponse
                {
                    auth_token = GlobalVariables.response_Noilty.data.user.auth_token,
                    password = GlobalVariables.password,

                    battle = new Models.Noilty.Battle
                    {
                        user_id = user_id,
                        account_id = account_id,
                        fraction_id = fraction_id,
                        wg_region = wg_region,
                        battle_type_id = battle_type_id,
                        damage_blocked = damage_blocked,
                        tank_durability = tank_durability,
                        capture_points = capture_points,
                        damage_dealt = damage_dealt,
                        damage_received = damage_received,
                        dropped_capture_points = dropped_capture_points,
                        frags = frags,
                        hits = hits,
                        losses = losses,
                        max_frags = max_frags,
                        max_xp = max_xp,
                        shots = shots,
                        spotted = spotted,
                        survived_battles = survived_battles,
                        win_and_survived = win_and_survived,
                        wins = wins,
                        credits = credits,
                        gold = gold,
                        free_xp = free_xp,
                        battle_life_time = battle_life_time,
                        is_premium = is_premium
                    }
            };

                var battleJson = JsonConvert.SerializeObject(GlobalVariables.battleResponse);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("battle", battleJson),
                });

                var response = await client.PostAsync("api/push-data/from/desktop-app", content);
                string json = await response.Content.ReadAsStringAsync();

                //GlobalVariables.response_Noilty = JsonConvert.DeserializeObject<Models.Noilty.Response>(json);
            }
        }
    }
}
