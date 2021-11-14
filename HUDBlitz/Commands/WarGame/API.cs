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
                    new KeyValuePair<string, string>("extra", "statistics.rating, private.grouped_contacts")
                });

                var response = await client.PostAsync("/wotb/account/info/", content);
                string json = await response.Content.ReadAsStringAsync();

                string original = json;
                string substring = account_id.ToString();
                int i = original.IndexOf(substring);
                string resultJson = original.Remove(i, substring.Length).Insert(i, "account");

                Global.Response_WG = JsonConvert.DeserializeObject<Response>(resultJson);

                Global.Response_WG_Static = File.Exists($"{account_id}.json") ?
                    JsonConvert.DeserializeObject<Response>(File.ReadAllText($"{account_id}.json")) :
                    JsonConvert.DeserializeObject<Response>(resultJson);

                Global.Response_WG_Send = File.Exists($"{account_id}_send.json") ?
                    JsonConvert.DeserializeObject<Response>(File.ReadAllText($"{account_id}_send.json")) :
                    JsonConvert.DeserializeObject<Response>(resultJson);

                if (Global.check)
                {
                    File.WriteAllText($"{account_id}.json", JsonConvert.SerializeObject(Global.Response_WG));
                    Global.check = false;
                }

                if (Global.IsSendNoilty)
                {
                    File.WriteAllText($"{account_id}_send.json", JsonConvert.SerializeObject(Global.Response_WG_Send));
                    Global.IsSendNoilty = false;
                }

                if (Global.Response_WG.data.account.statistics.rating.battles > Global.Response_WG_Static.data.account.statistics.rating.battles)
                {
                    Global.battleType = BattleType.RATING;
                    CalculateStatistics(
                        Global.battleType,
                        Global.Response_WG.data.account.statistics.rating.wins, Global.Response_WG_Static.data.account.statistics.rating.wins,
                        Global.Response_WG.data.account.statistics.rating.battles, Global.Response_WG_Static.data.account.statistics.rating.battles,
                        Global.Response_WG.data.account.statistics.rating.frags, Global.Response_WG_Static.data.account.statistics.rating.frags,
                        Global.Response_WG.data.account.statistics.rating.survived_battles, Global.Response_WG_Static.data.account.statistics.rating.survived_battles,
                        Global.Response_WG.data.account.statistics.rating.hits, Global.Response_WG_Static.data.account.statistics.rating.hits,
                        Global.Response_WG.data.account.statistics.rating.shots, Global.Response_WG_Static.data.account.statistics.rating.shots,
                        Global.Battles - Global.Survived_battles,
                        Global.Response_WG.data.account.statistics.rating.damage_dealt, Global.Response_WG_Static.data.account.statistics.rating.damage_dealt,
                        Global.Response_WG.data.account.statistics.rating.damage_received, Global.Response_WG_Static.data.account.statistics.rating.damage_received,
                        Global.Response_WG.data.account.statistics.rating.spotted, Global.Response_WG_Static.data.account.statistics.rating.spotted,
                        Global.Response_WG.data.account.statistics.rating.dropped_capture_points, Global.Response_WG_Static.data.account.statistics.rating.dropped_capture_points,
                        Global.Response_WG.data.account.statistics.rating.capture_points, Global.Response_WG_Static.data.account.statistics.rating.capture_points);
                }
                else if (Global.Response_WG.data.account.statistics.all.battles > Global.Response_WG_Static.data.account.statistics.all.battles)
                {
                    Global.battleType = BattleType.ORDINARY;
                    CalculateStatistics(
                        Global.battleType,
                        Global.Response_WG.data.account.statistics.all.wins, Global.Response_WG_Static.data.account.statistics.all.wins,
                        Global.Response_WG.data.account.statistics.all.battles, Global.Response_WG_Static.data.account.statistics.all.battles,
                        Global.Response_WG.data.account.statistics.all.frags, Global.Response_WG_Static.data.account.statistics.all.frags,
                        Global.Response_WG.data.account.statistics.all.survived_battles, Global.Response_WG_Static.data.account.statistics.all.survived_battles,
                        Global.Response_WG.data.account.statistics.all.hits, Global.Response_WG_Static.data.account.statistics.all.hits,
                        Global.Response_WG.data.account.statistics.all.shots, Global.Response_WG_Static.data.account.statistics.all.shots,
                        Global.Battles - Global.Survived_battles,
                        Global.Response_WG.data.account.statistics.all.damage_dealt, Global.Response_WG_Static.data.account.statistics.all.damage_dealt,
                        Global.Response_WG.data.account.statistics.all.damage_received, Global.Response_WG_Static.data.account.statistics.all.damage_received,
                        Global.Response_WG.data.account.statistics.all.spotted, Global.Response_WG_Static.data.account.statistics.all.spotted,
                        Global.Response_WG.data.account.statistics.all.dropped_capture_points, Global.Response_WG_Static.data.account.statistics.all.dropped_capture_points,
                        Global.Response_WG.data.account.statistics.all.capture_points, Global.Response_WG_Static.data.account.statistics.all.capture_points);
                }
                else if (Global.Response_WG.data.account.statistics.clan.battles > Global.Response_WG_Static.data.account.statistics.clan.battles)
                {
                    Global.battleType = BattleType.CLAN;
                    CalculateStatistics(
                        Global.battleType,
                        Global.Response_WG.data.account.statistics.clan.wins, Global.Response_WG_Static.data.account.statistics.clan.wins,
                        Global.Response_WG.data.account.statistics.clan.battles, Global.Response_WG_Static.data.account.statistics.clan.battles,
                        Global.Response_WG.data.account.statistics.clan.frags, Global.Response_WG_Static.data.account.statistics.clan.frags,
                        Global.Response_WG.data.account.statistics.clan.survived_battles, Global.Response_WG_Static.data.account.statistics.clan.survived_battles,
                        Global.Response_WG.data.account.statistics.clan.hits, Global.Response_WG_Static.data.account.statistics.clan.hits,
                        Global.Response_WG.data.account.statistics.clan.shots, Global.Response_WG_Static.data.account.statistics.clan.shots,
                        Global.Battles - Global.Survived_battles,
                        Global.Response_WG.data.account.statistics.clan.damage_dealt, Global.Response_WG_Static.data.account.statistics.clan.damage_dealt,
                        Global.Response_WG.data.account.statistics.clan.damage_received, Global.Response_WG_Static.data.account.statistics.clan.damage_received,
                        Global.Response_WG.data.account.statistics.clan.spotted, Global.Response_WG_Static.data.account.statistics.clan.spotted,
                        Global.Response_WG.data.account.statistics.clan.dropped_capture_points, Global.Response_WG_Static.data.account.statistics.clan.dropped_capture_points,
                        Global.Response_WG.data.account.statistics.clan.capture_points, Global.Response_WG_Static.data.account.statistics.clan.capture_points);
                }

                if (Global.Response_WG.data.account.statistics.rating.battles > Global.Response_WG_Send.data.account.statistics.rating.battles)
                {
                    string _user_id = Global.response_Noilty.data.account.user_id.ToString();
                    string _account_id = Global.response_Noilty.data.account.id.ToString();
                    string _fraction_id = Global.response_Noilty.data.account.fraction_id.ToString();
                    string _wg_region = Global.response_Noilty.data.user.wg_region;
                    string _battle_type_id = ((int)Global.battleType).ToString();
                    string _damage_blocked = Global.blocked_damage.ToString();
                    string _current_durability_tank = Global.Strength.ToString();
                    string _capture_points = (Global.Response_WG.data.account.statistics.rating.battles - Global.Response_WG_Send.data.account.statistics.rating.battles).ToString();
                    string _damage_dealt = (Global.Response_WG.data.account.statistics.rating.damage_dealt - Global.Response_WG_Send.data.account.statistics.rating.damage_dealt).ToString();
                    string _damage_received = (Global.Response_WG.data.account.statistics.rating.damage_received - Global.Response_WG_Send.data.account.statistics.rating.damage_received).ToString();
                    string _dropped_capture_points = (Global.Response_WG.data.account.statistics.rating.dropped_capture_points - Global.Response_WG_Send.data.account.statistics.rating.dropped_capture_points).ToString();
                    string _frags = (Global.Response_WG.data.account.statistics.rating.frags - Global.Response_WG_Send.data.account.statistics.rating.frags).ToString();
                    string _frags8p = (Global.Response_WG.data.account.statistics.rating.frags8p - Global.Response_WG_Send.data.account.statistics.rating.frags8p).ToString();
                    string _hits = (Global.Response_WG.data.account.statistics.rating.hits - Global.Response_WG_Send.data.account.statistics.rating.hits).ToString();
                    string _losses = (Global.Response_WG.data.account.statistics.rating.losses - Global.Response_WG_Send.data.account.statistics.rating.losses).ToString();
                    string _max_frags = (Global.Response_WG.data.account.statistics.all.max_frags - Global.Response_WG_Send.data.account.statistics.all.max_frags).ToString();
                    string _max_xp = (Global.Response_WG.data.account.statistics.all.max_xp - Global.Response_WG_Send.data.account.statistics.all.max_xp).ToString();
                    string _shots = (Global.Response_WG.data.account.statistics.rating.shots - Global.Response_WG_Send.data.account.statistics.rating.shots).ToString();
                    string _spotted = (Global.Response_WG.data.account.statistics.rating.spotted - Global.Response_WG_Send.data.account.statistics.rating.spotted).ToString();
                    string _survived_battles = (Global.Response_WG.data.account.statistics.rating.survived_battles - Global.Response_WG_Send.data.account.statistics.rating.survived_battles).ToString();
                    string _win_and_survived = (Global.Response_WG.data.account.statistics.rating.win_and_survived - Global.Response_WG_Send.data.account.statistics.rating.win_and_survived).ToString();
                    string _wins = (Global.Response_WG.data.account.statistics.rating.wins - Global.Response_WG_Send.data.account.statistics.rating.wins).ToString();
                    string _xp = (Global.Response_WG.data.account.statistics.rating.xp - Global.Response_WG_Send.data.account.statistics.rating.xp).ToString();
                    string _credits = (Global.Response_WG.data.account.@private.credits - Global.Response_WG_Send.data.account.@private.credits).ToString();
                    string _gold = (Global.Response_WG.data.account.@private.gold - Global.Response_WG_Send.data.account.@private.gold).ToString();
                    string _free_xp = (Global.Response_WG.data.account.@private.free_xp - Global.Response_WG_Send.data.account.@private.free_xp).ToString();
                    string _battle_life_time = Global.Response_WG.data.account.@private.battle_life_time.ToString();
                    string _is_premium = Global.Response_WG.data.account.@private.is_premium ? "1" : "0"; 
                    
                    await SendData(_user_id,_account_id,_fraction_id,_wg_region,_battle_type_id,_damage_blocked,_current_durability_tank,_capture_points,_damage_dealt,_damage_received,_dropped_capture_points,
                        _frags, _frags8p, _hits,_losses,_max_frags, _max_xp,_shots,_spotted,_survived_battles,_win_and_survived,_wins, _xp, _credits,_gold,_free_xp,_battle_life_time,_is_premium);

                    File.WriteAllText($"{account_id}_send.json", JsonConvert.SerializeObject(Global.Response_WG));

                    Global.IsSendNoilty = true;
                }
                else if (Global.Response_WG.data.account.statistics.all.battles > Global.Response_WG_Send.data.account.statistics.all.battles)
                {
                    string _user_id = Global.response_Noilty.data.account.user_id.ToString();
                    string _account_id = Global.response_Noilty.data.account.id.ToString();
                    string _fraction_id = Global.response_Noilty.data.account.fraction_id.ToString();
                    string _wg_region = Global.response_Noilty.data.user.wg_region;
                    string _battle_type_id = ((int)Global.battleType).ToString();
                    string _damage_blocked = Global.blocked_damage.ToString();
                    string _current_durability_tank = Global.Strength.ToString();
                    string _capture_points = (Global.Response_WG.data.account.statistics.all.battles - Global.Response_WG_Send.data.account.statistics.all.battles).ToString();
                    string _damage_dealt = (Global.Response_WG.data.account.statistics.all.damage_dealt - Global.Response_WG_Send.data.account.statistics.all.damage_dealt).ToString();
                    string _damage_received = (Global.Response_WG.data.account.statistics.all.damage_received - Global.Response_WG_Send.data.account.statistics.all.damage_received).ToString();
                    string _dropped_capture_points = (Global.Response_WG.data.account.statistics.all.dropped_capture_points - Global.Response_WG_Send.data.account.statistics.all.dropped_capture_points).ToString();
                    string _frags = (Global.Response_WG.data.account.statistics.all.frags - Global.Response_WG_Send.data.account.statistics.all.frags).ToString();
                    string _frags8p = (Global.Response_WG.data.account.statistics.all.frags8p - Global.Response_WG_Send.data.account.statistics.all.frags8p).ToString();
                    string _hits = (Global.Response_WG.data.account.statistics.all.hits - Global.Response_WG_Send.data.account.statistics.all.hits).ToString();
                    string _losses = (Global.Response_WG.data.account.statistics.all.losses - Global.Response_WG_Send.data.account.statistics.all.losses).ToString();
                    string _max_frags = (Global.Response_WG.data.account.statistics.all.max_frags - Global.Response_WG_Send.data.account.statistics.all.max_frags).ToString();
                    string _max_xp = (Global.Response_WG.data.account.statistics.all.max_xp - Global.Response_WG_Send.data.account.statistics.all.max_xp).ToString();
                    string _shots = (Global.Response_WG.data.account.statistics.all.shots - Global.Response_WG_Send.data.account.statistics.all.shots).ToString();
                    string _spotted = (Global.Response_WG.data.account.statistics.all.spotted - Global.Response_WG_Send.data.account.statistics.all.spotted).ToString();
                    string _survived_battles = (Global.Response_WG.data.account.statistics.all.survived_battles - Global.Response_WG_Send.data.account.statistics.all.survived_battles).ToString();
                    string _win_and_survived = (Global.Response_WG.data.account.statistics.all.win_and_survived - Global.Response_WG_Send.data.account.statistics.all.win_and_survived).ToString();
                    string _wins = (Global.Response_WG.data.account.statistics.all.wins - Global.Response_WG_Send.data.account.statistics.all.wins).ToString();
                    string _xp = (Global.Response_WG.data.account.statistics.all.xp - Global.Response_WG_Send.data.account.statistics.all.xp).ToString();
                    string _credits = (Global.Response_WG.data.account.@private.credits - Global.Response_WG_Send.data.account.@private.credits).ToString();
                    string _gold = (Global.Response_WG.data.account.@private.gold - Global.Response_WG_Send.data.account.@private.gold).ToString();
                    string _free_xp = (Global.Response_WG.data.account.@private.free_xp - Global.Response_WG_Send.data.account.@private.free_xp).ToString();
                    string _battle_life_time = Global.Response_WG.data.account.@private.battle_life_time.ToString();
                    string _is_premium = Global.Response_WG.data.account.@private.is_premium ? "1" : "0";

                    await SendData(_user_id, _account_id, _fraction_id, _wg_region, _battle_type_id, _damage_blocked, _current_durability_tank, _capture_points, _damage_dealt, _damage_received, _dropped_capture_points,
                        _frags, _frags8p, _hits, _losses, _max_frags, _max_xp, _shots, _spotted, _survived_battles, _win_and_survived, _wins, _xp, _credits, _gold, _free_xp, _battle_life_time, _is_premium);

                    File.WriteAllText($"{account_id}_send.json", JsonConvert.SerializeObject(Global.Response_WG));

                    Global.IsSendNoilty = true;
                }
                else if (Global.Response_WG.data.account.statistics.clan.battles > Global.Response_WG_Send.data.account.statistics.clan.battles)
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
                        Global.Wins = updateWins - staticWins;
                        Global.Battles = updateBattles - staticBattles;
                        Global.Frags = updateFrags - staticFrags;
                        Global.Survived_battles = updateSurvived_battles - staticSurvived_battles;
                        Global.Hits = updateHits - staticHits;
                        Global.Shots = updateShots - statickShots;
                        Global.Deaths = updateDeaths;
                        Global.Damage_dealt = updateDamage_dealt - staticDamage_dealt;
                        Global.Damage_received = updateDamage_received - staticDamage_received;
                        Global.Spotted = updateSpotted - staticSpotted;
                        Global.Dropped_capture_points = updateDropped_capture_points - staticDropped_capture_points;
                        Global.Capture_points = updateCapture_points - staticCapture_points;

                        Global.WinRate = Global.Wins / Global.Battles * 100f;
                        break;
                    case BattleType.RATING:
                        Global.Wins = updateWins - staticWins;
                        Global.Battles = updateBattles - staticBattles;
                        Global.Frags = updateFrags - staticFrags;
                        Global.Survived_battles = updateSurvived_battles - staticSurvived_battles;
                        Global.Hits = updateHits - staticHits;
                        Global.Shots = updateShots - statickShots;
                        Global.Deaths = updateDeaths;
                        Global.Damage_dealt = updateDamage_dealt - staticDamage_dealt;
                        Global.Damage_received = updateDamage_received - staticDamage_received;
                        Global.Spotted = updateSpotted - staticSpotted;
                        Global.Dropped_capture_points = updateDropped_capture_points - staticDropped_capture_points;
                        Global.Capture_points = updateCapture_points - staticCapture_points;

                        Global.WinRate = Global.Wins / Global.Battles * 100f;
                        break;
                    case BattleType.CLAN:
                        Global.Wins = updateWins - staticWins;
                        Global.Battles = updateBattles - staticBattles;
                        Global.Frags = updateFrags - staticFrags;
                        Global.Survived_battles = updateSurvived_battles - staticSurvived_battles;
                        Global.Hits = updateHits - staticHits;
                        Global.Shots = updateShots - statickShots;
                        Global.Deaths = updateDeaths;
                        Global.Damage_dealt = updateDamage_dealt - staticDamage_dealt;
                        Global.Damage_received = updateDamage_received - staticDamage_received;
                        Global.Spotted = updateSpotted - staticSpotted;
                        Global.Dropped_capture_points = updateDropped_capture_points - staticDropped_capture_points;
                        Global.Capture_points = updateCapture_points - staticCapture_points;

                        Global.WinRate = Global.Wins / Global.Battles * 100f;
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
            string current_durability_tank, 
            string capture_points,
            string damage_dealt, 
            string damage_received, 
            string dropped_capture_points, 
            string frags, 
            string frags8p, 
            string hits,
            string losses,
            string max_frags,
            string max_xp,
            string shots,
            string spotted,
            string survived_battles,
            string win_and_survived,
            string wins,
            string xp,
            string credits,
            string gold,
            string free_xp,
            string battle_life_time,
            string is_premium)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{Global.urlBbury}");

                Global.battleResponse = new Models.Noilty.BattleResponse
                {
                    auth_token = Global.response_Noilty.data.user.auth_token,
                    password = Global.password,

                    battle = new Models.Noilty.Battle
                    {
                        user_id = user_id,
                        account_id = account_id,
                        fraction_id = fraction_id,
                        wg_region = wg_region,
                        battle_type_id = battle_type_id,
                        damage_blocked = damage_blocked,
                        current_durability_tank = current_durability_tank,
                        capture_points = capture_points,
                        damage_dealt = damage_dealt,
                        damage_received = damage_received,
                        dropped_capture_points = dropped_capture_points,
                        frags = frags,
                        frags8p = frags8p,
                        hits = hits,
                        losses = losses,
                        max_frags = max_frags,
                        max_xp = max_xp,
                        shots = shots,
                        spotted = spotted,
                        survived_battles = survived_battles,
                        win_and_survived = win_and_survived,
                        wins = wins,
                        xp = xp,
                        credits = credits,
                        gold = gold,
                        free_xp = free_xp,
                        battle_life_time = battle_life_time,
                        is_premium = is_premium
                    }
            };

                var battleJson = JsonConvert.SerializeObject(Global.battleResponse);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("data", battleJson),
                });

                var response = await client.PostAsync("/api/push-data/from/desktop-app", content);
                string json = await response.Content.ReadAsStringAsync();

                //GlobalVariables.response_Noilty = JsonConvert.DeserializeObject<Models.Noilty.Response>(json);
            }
        }
    }
}
