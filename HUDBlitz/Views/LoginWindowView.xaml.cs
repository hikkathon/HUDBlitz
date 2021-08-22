using HUDBlitz.Commands;
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

        private async void btnEnterToken_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.response_Noilty != null && !GlobalVariables.response_Noilty.status.Contains("error"))
            {
                TokenPanelHidden();
            }
            else
            {
                await AuthToken(UserKey.Password);
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.MS = new MemoryScanner("wotblitz.exe");

            try
            {
                GlobalVariables.MS.GetBaseAddress();
                GlobalVariables.IsEnabledGame = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show($"World of Tanks Blitz не запущен, запустите игру и повторите попытку.", $"Ошибка : {exc.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                GlobalVariables.IsEnabledGame = false;
            }

            if (GlobalVariables.response_Noilty != null && !GlobalVariables.IsHashValid.Contains("error") && GlobalVariables.IsEnabledGame)
            {
                PasswordPanelHidden();
            }
            else
            {
                await AuthPassword(UserPassword.Password, GlobalVariables.response_Noilty.data.account.password);
            }
        }

        public void TokenPanelHidden()
        {
            TokenPanel.Visibility = Visibility.Hidden;
            AuthPanel.Visibility = Visibility.Visible;
        }

        public void PasswordPanelHidden()
        {
            HUDWindowView hudView = new HUDWindowView();
            hudView.Owner = this;
            hudView.Show();

            AuthPanel.Visibility = Visibility.Hidden;
            UserProfilePanel.Visibility = Visibility.Visible;

            btnLogin.IsEnabled = false;
            LabelNotify.Visibility = Visibility.Hidden;
        }

        // Метод возвращает информацию об игроке.
        private async Task GetAccountInfo(string application_id, string account_id, string access_token, string region)
        {
            bool check = true;
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

                while (check)
                {
                    File.WriteAllText($"{account_id}.json", JsonConvert.SerializeObject(GlobalVariables.Response_WG));
                    check = false;
                }
            }
        }

        // Метод возвращает информацию об игроке.
        private async Task AuthToken(string auth_token)
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

                GlobalVariables.response_Noilty = JsonConvert.DeserializeObject<Models.Noilty.Response>(json);
            }

            // Если токен валидный пропускаем пользователя дальше
            if (GlobalVariables.response_Noilty.status.Contains("success"))
            {
                UserKey.IsEnabled = false;
                btnEnterToken.Content = $"Далее";
                LabelNotify.Content = GlobalVariables.response_Noilty.status;
            }
            else
            {
                LabelNotify.Content = GlobalVariables.response_Noilty.status;
            }
        }

        private async Task AuthPassword(string password, string passwordHash)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://blitzbury.noilty.loc");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("password_hash", passwordHash)
                });

                var response = await client.PostAsync("/api/check/hash/password", content);
                GlobalVariables.IsHashValid = await response.Content.ReadAsStringAsync();
            }

            if (GlobalVariables.IsHashValid.Contains("success"))
            {
                LabelNotify.Content = "success";
                UserPassword.IsEnabled = false;
                btnLogin.Content = "Запустить HUD";

                await GetAccountInfo(
                    "d2bfb95adbc6f34fb32c4924b4c93fa4",
                    GlobalVariables.response_Noilty.data.user.wg_account_id.ToString(),
                    GlobalVariables.response_Noilty.data.user.wg_access_token,
                    GlobalVariables.response_Noilty.data.user.wg_region);

                LabelNick.Content = $"{GlobalVariables.Response_WG.data.account.nickname}";
            }
            else
            {
                LabelNotify.Content = "error";
            }
        }
    }
}
