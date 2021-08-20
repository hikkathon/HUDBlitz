namespace HUDBlitz.Models.Player
{
    public class Account
    {
        public Statistics statistics { get; set; }
        public int account_id { get; set; }
        public object created_at { get; set; }
        public object updated_at { get; set; }
        public Private @private { get; set; }
        public object last_battle_time { get; set; }
        public string nickname { get; set; }
    }
}