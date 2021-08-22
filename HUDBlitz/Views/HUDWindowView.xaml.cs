using HUDBlitz.Commands;
using HUDBlitz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для HUDWindowView.xaml
    /// </summary>
    public partial class HUDWindowView : Window, INotifyPropertyChanged
    {
        public HUDWindowView()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.DataContext = this;

            GlobalVariables.MS.GetProcessByName();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalVariables.MS.RegisterHandler(new MemoryScanner.MemoryEditorStateHandler(DamageShow));

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += GlobalVariables.MS.GetDamage;
            dt.Tick += WindowSize;
            dt.Start();
        }

        #region Damage Panel

        int Dealt { get { return GlobalVariables.MS.Dealt; } }
        int Received { get { return GlobalVariables.MS.Received; } }
        int Strength { get { return GlobalVariables.MS.Strength; } }
        int WGID { get { return GlobalVariables.MS.WGID; } }

        private int _currentDealt;
        private int _currentReceived;
        private int _maxDealt;
        private int _maxReceived;

        public int CurrentDealt{ get { return _currentDealt; } set { _currentDealt = value; } }
        public int CurrentReceived { get { return _currentReceived; } set { _currentReceived = value; ; } }
        public int MaxDealt{ get { return _maxDealt; } set { _maxDealt = value; } }
        public int MaxReceived { get { return _maxReceived; } set { _maxReceived = value; ; } }

        public void DamageShow()
        {
            if (Strength == 0 )
            {
                MaxDealt = 0;
                MaxReceived = 0;
                StackPanelDamage.Visibility = Visibility.Hidden;
            }
            else
            {
                StackPanelDamage.Visibility = Visibility.Visible;
            }

            CurrentDealt = Dealt;
            CurrentReceived = Received;

            if (CurrentDealt > MaxDealt)
                MaxDealt = CurrentDealt;

            if (CurrentReceived > MaxReceived)
                MaxReceived = CurrentReceived;


            DealtText.Text = (MaxDealt == 0) ? " 0" : $"{MaxDealt:# ### ###}";
            ReceivedText.Text = (MaxReceived == 0) ? " 0" : $"{MaxReceived:# ### ###}";
            StrengthText.Text = (Strength == 0) ? " 0" : $"{Strength:# ### ###}";
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
            if (GlobalVariables.IsEnabledGame)
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
