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

namespace GarageTool
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            try
            {
                lblCountBatt.Content = ((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[9];
                btnOk.Content = ((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[10];
            }
            catch (Exception)
            {
                lblCountBatt.Content = "tu jest blad :)";
            }
            sliTest.IsSnapToTickEnabled = true;
            txtTest.Text = "0";


        }

        private void sliTest_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            txtTest.Text = sliTest.Value.ToString();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).Battery_toRent = txtTest.Text;
            this.Close();
        }
    }
}
