using HUDBlitz.Models.Player;
using System.Net.Http;

namespace HUDBlitz.Models
{
    public static class GlobalVariables
    {
        // Player
        public static Response player;  // Информаци о игрое, обновляется
        public static Response playerStatic; // Информаци о игрое, статическая. Записывается при первом запуске

        // Noilty
        public static Noilty.Response Noilty;
    }
}
