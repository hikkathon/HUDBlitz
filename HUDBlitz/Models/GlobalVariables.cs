using HUDBlitz.Commands;
using System.Net.Http;
using System.Windows.Threading;
using static HUDBlitz.Commands.WarGame.API;

namespace HUDBlitz.Models
{
    public static class GlobalVariables
    {
        public static string urlBbury = "http://www.blitzbury.noilty.com";
        /// <summary>
        /// Player : Информаци о игроке, обновляется каждый тик таймера
        /// </summary>
        public static Player.Response Response_WG;
        /// <summary>
        /// Player : Информаци о игроке, статическая. Записывается при первом запуске HUD'а
        /// </summary>
        public static Player.Response Response_WG_Static;

        /// <summary>
        /// Данные зарегестрированного ирока в топе Noilty
        /// </summary>
        public static Noilty.Response response_Noilty;
        public static string IsHashValid = "error";
        public static bool IsSendNoilty = true;
        public static Player.Response Response_WG_Send;
        public static Noilty.BattleResponse battleResponse;
        public static string password;

        public static int blocked_damage;
        public static int Strength;

        // Переменные Сканера памяти
        public static bool IsEnabledGame = false;
        /// <summary>
        /// MemoryScanner
        /// </summary>
        public static MemoryScanner MS;

        // Переменные после боевой статистики
        public static BattleType battleType;
        public static bool check = true;

        #region "Бои"
        public static float WinRate;
        public static int Wins;
        public static int Battles;
        public static int Frags;
        public static int Survived_battles;
        public static int Hits;
        public static int Shots;
        public static int Deaths;
        public static int Damage_dealt;
        public static int Damage_received;
        public static int Spotted;
        public static int Dropped_capture_points;
        public static int Capture_points;
        #endregion
    }
}
