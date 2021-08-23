namespace HUDBlitz.Models.Noilty
{
    public class User
    {
        public int id { get; set; }
        public string wg_nickname { get; set; }
        public string wg_region { get; set; }
        public int wg_account_id { get; set; }
        public string wg_access_token { get; set; }
        public string wg_expires_at { get; set; }
        public string email { get; set; }
        public string email_verified_at { get; set; }
        public string auth_token { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Account
    {
        public int account_id { get; set; }
        public int user_id { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string balance { get; set; }
        public string date_birth { get; set; }
        public string about { get; set; }
        public string website { get; set; }
        public string social_networks { get; set; }
        public object verified_at { get; set; }
        public string password { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string city_name { get; set; }
        public string city_region { get; set; }
        public string country_name { get; set; }
        public string country_dial_code { get; set; }
        public string country_iso_code { get; set; }
        public string country_flag_url { get; set; }
        public string language_name { get; set; }
        public string language_native_name { get; set; }
        public string language_iso_code { get; set; }
        public string gender_name { get; set; }
        public string fractions { get; set; }
    }

    public class Data
    {
        public User user { get; set; }
        public Account account { get; set; }
    }

    public class Response
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
}
