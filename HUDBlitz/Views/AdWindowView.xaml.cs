using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для AdWindowView.xaml
    /// </summary>
    public partial class AdWindowView : Window
    {
        public AdWindowView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //MyWebView.Navigate("https://www.vk.com");
            }
            catch(Exception exc)
            {
                MessageBox.Show("Err", "Игра не найдена.", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
    }
}
