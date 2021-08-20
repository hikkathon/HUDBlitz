using HUDBlitz.Models;
using HUDBlitz.Models.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HUDBlitz.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindowView.xaml
    /// </summary>
    public partial class LoginWindowView : Window
    {
        public LoginWindowView()
        {
            InitializeComponent();
        }

        private void btnEnterToken_Click(object sender, RoutedEventArgs e)
        {
            Auth(UserKey.Password);

            if (GlobalVariables.Noilty != null)
            {
                if (GlobalVariables.Noilty.status == "success")
                {
                    TokenPanelHidden();
                }
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            #region Show HUD

            HUDWindowView hudView = new HUDWindowView();
            hudView.Owner = this;
            hudView.Show();

            #endregion
        }

        public void TokenPanelHidden()
        {
            TokenPanel.Visibility = Visibility.Hidden;
            AuthPanel.Visibility = Visibility.Visible;
        }

        // Метод возвращает информацию об игроке.
        private async Task GetAccountInfo(string application_id, string account_id, string access_token, string language)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://api.wotblitz.{language}/");

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

                GlobalVariables.player = JsonConvert.DeserializeObject<Response>(resultJson);
                GlobalVariables.playerStatic = File.Exists($"{account_id}.json") ?
                    JsonConvert.DeserializeObject<Response>(File.ReadAllText($"{account_id}.json")) :
                    JsonConvert.DeserializeObject<Response>(resultJson);
            }
        }

        // Метод возвращает информацию об игроке.
        private async Task Auth(string auth_token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://blitzbury.noilty.loc");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("auth_token", auth_token)
                });

                var response = await client.PostAsync("/api/get-data/from/db-user", content);
                string json = await response.Content.ReadAsStringAsync();

                GlobalVariables.Noilty = JsonConvert.DeserializeObject<Models.Noilty.Response>(json);

                DebugLabel.Content = json;
            }
        }
    }
}
