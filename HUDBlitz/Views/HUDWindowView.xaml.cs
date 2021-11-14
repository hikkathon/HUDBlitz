using HUDBlitz.Commands;
using HUDBlitz.Commands.WarGame;
using HUDBlitz.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace HUDBlitz.Views
{
    /// <summary>
    /// Логика взаимодействия для HUDWindowView.xaml
    /// </summary>
    public partial class HUDWindowView : Window, INotifyPropertyChanged
    {
        public HUDWindowView()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.DataContext = this;

            Global.MS.GetProcessByName();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Global.MS.RegisterHandler(new MemoryScanner.MemoryEditorStateHandler(DamageShow));


            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += Global.MS.GetDamage;
            dt.Tick += WindowSize;
            dt.Start();

            StartTimer();
        }

        public void StartTimer()
        {
            DispatcherTimer TimerPostCombatStatistics = new DispatcherTimer();
            TimerPostCombatStatistics.Interval = TimeSpan.FromSeconds(2);
            TimerPostCombatStatistics.Tick += AccountInfo;
            TimerPostCombatStatistics.Start();
        }

        public int count;
        private async void AccountInfo(object sender, EventArgs e)
        {
            count++;
            await API.GetAccountInfo("d2bfb95adbc6f34fb32c4924b4c93fa4", Global.response_Noilty.data.user.wg_account_id.ToString(), Global.response_Noilty.data.user.wg_access_token, Global.response_Noilty.data.user.wg_region);  
            BattleStatsLabel.Content = 
                $"Mode:\t\t\t{Global.battleType}\n" +
                $"Nickname:\t\t{Global.Response_WG.data.account.nickname}\n" +
                $"Wins/Battles:\t\t{Global.Wins} ({Global.Battles})\t({Math.Round((float)Global.Wins / (float)Global.Battles * 100, 2).ToString().Replace("не число", "0")}%)\n" +
                $"Kills:\t\t\t{Global.Frags}\t({Math.Round((float)Global.Frags / (float)Global.Battles, 2).ToString().Replace("не число", "0")})\n" +
                $"Deaths:\t\t\t{Global.Deaths}\t({Math.Round((float)Global.Deaths / (float)Global.Battles * 100, 2).ToString().Replace("не число", "0")}%)\n" +
                $"Hits/Shots:\t\t{Global.Hits}/{Global.Shots}\t({Math.Round((float)Global.Hits / (float)Global.Shots * 100, 2).ToString().Replace("не число", "0")}%)\n" +
                $"Damage Dealt:\t\t{Global.Damage_dealt}\t({Math.Round((float)Global.Damage_dealt / (float)Global.Battles, 0).ToString().Replace("не число", "0")})\n" +
                $"Damage Received:\t{Global.Damage_received}\t({Math.Round((float)Global.Damage_received / (float)Global.Battles, 0).ToString().Replace("не число", "0")})\n" +
                $"Spotted:\t\t\t{Global.Spotted}\t({Math.Round((float)Global.Spotted / (float)Global.Battles, 2).ToString().Replace("не число", "0")})\n" +
                $"Defence:\t\t\t{Global.Dropped_capture_points}\t({Math.Round((float)Global.Dropped_capture_points / (float)Global.Battles, 2).ToString().Replace("не число", "0")})\n" +
                $"Capture:\t\t\t{Global.Capture_points}\t({Math.Round((float)Global.Capture_points / (float)Global.Battles, 2).ToString().Replace("не число", "0")})\n\n" +
                $"Count: {count}, Send : {Global.IsSendNoilty}" ;
        }

        #region Damage Panel

        int Dealt { get { return Global.MS.Dealt; } }
        int Blocked { get { return Global.MS.Blocked; } }
        int Strength { get { return Global.MS.Strength; } }
        int WGID { get { return Global.MS.WGID; } }

        private int _currentDealt;
        private int _currentBlocked;
        private int _maxDealt;
        private int _maxBlocked;

        public int CurrentDealt{ get { return _currentDealt; } set { _currentDealt = value; } }
        public int CurrentBlocked { get { return _currentBlocked; } set { _currentBlocked = value; ; } }
        public int MaxDealt{ get { return _maxDealt; } set { _maxDealt = value; } }
        public int MaxBlocked { get { return _maxBlocked; } set { _maxBlocked = value; ; } }

        public void DamageShow()
        {
            if (Strength == 0)
            {
                MaxDealt = 0;
                MaxBlocked = 0;
                //StackPanelDamage.Visibility = Visibility.Hidden;
                //GridStats.Visibility = Visibility.Hidden;
            }
            else
            {
            //    StackPanelDamage.Visibility = Visibility.Visible;
            //    GridStats.Visibility = Visibility.Visible;
            }

            CurrentDealt = Dealt;
            CurrentBlocked = Blocked;

            if (CurrentDealt > MaxDealt)
                MaxDealt = CurrentDealt;

            if (CurrentBlocked > MaxBlocked)
                MaxBlocked = CurrentBlocked;


            DealtText.Text = (CurrentDealt == 0) ? " 0" : $"{MaxDealt:# ### ###}";
            ReceivedText.Text = (CurrentBlocked == 0) ? " 0" : $"{MaxBlocked:# ### ###}";
            StrengthText.Text = (Strength == 0) ? " 0" : $"{Strength:# ### ###}";

            Global.blocked_damage = MaxBlocked;
            Global.Strength = Strength;
        }

        #endregion

        #region Изменение размера окна
        public event PropertyChangedEventHandler PropertyChanged;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public void WindowSize(object sender, EventArgs e)
        {
            if (Global.IsEnabledGame)
            {
                using (Process process = Process.GetProcessesByName("wotblitz")[0])
                {
                    uint handle = (uint)process.MainWindowHandle; // хендл окна
                    Rect r = new Rect();
                    if (GetWindowRect((IntPtr)handle, ref r))
                    {
                        CustomWidth = r.right - r.left;
                        CustomHeight = r.bottom - r.top;
                        CustomTop = r.top;
                        CustomLeft = r.left;
                    }
                }
            }
        }

        public int CustomHeight
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomHeight"));
            }
        }
        public int CustomWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomWidth"));
            }
        }
        public int CustomTop
        {
            get
            {
                return _posX;
            }
            set
            {
                _posX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomTop"));
            }
        }
        public int CustomLeft
        {
            get
            {
                return _PosY;
            }
            set
            {
                _PosY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomLeft"));
            }
        }

        private int _height;
        private int _width;
        private int _posX;
        private int _PosY;
        #endregion
    }
}
