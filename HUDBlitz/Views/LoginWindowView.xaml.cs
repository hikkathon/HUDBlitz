using HUDBlitz.Commands;
using HUDBlitz.Commands.WarGame;
using HUDBlitz.Models;
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
using System.Windows.Threading;

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
            if (Global.response_Noilty != null && !Global.response_Noilty.status.Contains("error"))
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
            Global.MS = new MemoryScanner("wotblitz.exe");

            try
            {
                Global.MS.GetBaseAddress();
                Global.IsEnabledGame = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show($"World of Tanks Blitz не запущен, запустите игру и повторите попытку.", $"Ошибка : {exc.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                Global.IsEnabledGame = false;
            }

            if (Global.response_Noilty != null && !Global.IsHashValid.Contains("error") && Global.IsEnabledGame)
            {
                PasswordPanelHidden();
            }
            else
            {
                Global.password = UserPassword.Password;
                await AuthPassword(UserPassword.Password, Global.response_Noilty.data.account.password);
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
        private async Task AuthToken(string auth_token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{Global.urlBbury}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("auth_token", auth_token)
                });

                var response = await client.PostAsync("/api/get-data/from/db-user", content);
                string json = await response.Content.ReadAsStringAsync();

                Global.response_Noilty = JsonConvert.DeserializeObject<Models.Noilty.Response>(json);
            }

            // Если токен валидный пропускаем пользователя дальше
            if (Global.response_Noilty.status.Contains("success"))
            {
                UserKey.IsEnabled = false;
                btnEnterToken.Content = $"Далее";
                LabelNotify.Content = Global.response_Noilty.status;
            }
            else
            {
                LabelNotify.Content = Global.response_Noilty.status;
            }
        }

        private async Task AuthPassword(string password, string passwordHash)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{Global.urlBbury}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("password_hash", passwordHash)
                });

                var response = await client.PostAsync("/api/check/hash/password", content);
                Global.IsHashValid = await response.Content.ReadAsStringAsync();
            }

            if (Global.IsHashValid.Contains("success"))
            {
                LabelNotify.Content = "success";
                UserPassword.IsEnabled = false;
                btnLogin.Content = "Запустить HUD";

                //await API.GetAccountInfo(
                //    "d2bfb95adbc6f34fb32c4924b4c93fa4",
                //    GlobalVariables.response_Noilty.data.user.wg_account_id.ToString(),
                //    GlobalVariables.response_Noilty.data.user.wg_access_token,
                //    GlobalVariables.response_Noilty.data.user.wg_region);

                //LabelNick.Content = $"{GlobalVariables.Response_WG.data.account.nickname}";
            }
            else
            {
                LabelNotify.Content = "error";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //AdWindowView ad = new AdWindowView();
            //ad.Show();

            MessageBoxResult boxResult = MessageBox.Show("Я не продаю моды. Все материалы на моих ресурсах доступны всем желающим и абсолютно бесплатно и что бы это было и впредь ты можешь посмотреть рекламу в моих играх и тем самым поддержать меня)", "Поддержать проект", MessageBoxButton.YesNo);
            if(MessageBoxResult.Yes == boxResult)
            {
                //System.Diagnostics.Process.Start("https://yandex.ru/games/play/171298/");
            }
            else
            {
                Close();
            }
        }
    }
}
