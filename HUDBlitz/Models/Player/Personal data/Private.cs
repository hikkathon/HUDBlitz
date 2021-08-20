namespace HUDBlitz.Models.Player
{
    public class Private
    {
        public Restrictions restrictions { get; set; }
        public int gold { get; set; }
        public int free_xp { get; set; }
        public object ban_time { get; set; }
        public bool is_premium { get; set; }
        public int credits { get; set; }
        public object premium_expires_at { get; set; }
        public object battle_life_time { get; set; }
        public string ban_info { get; set; }
    }

    public class Restrictions
    {
        public object chat_ban_time { get; set; }
    }
}
