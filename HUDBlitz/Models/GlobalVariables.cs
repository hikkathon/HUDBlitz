using HUDBlitz.Models.Player;
using System.Net.Http;

namespace HUDBlitz.Models
{
    public static class GlobalVariables
    {
        /// <summary>
        /// Player : Информаци о игроке, обновляется каждый тик таймера
        /// </summary>
        public static Response response_WG;
        /// <summary>
        /// Player : Информаци о игроке, статическая. Записывается при первом запуске HUD'а
        /// </summary>
        public static Response response_WG_Static;

        /// <summary>
        /// Данные зарегестрированного ирока в топе Noilty
        /// </summary>
        public static Noilty.Response response_Noilty;
        public static string isHashValid = "error";
    }
}
