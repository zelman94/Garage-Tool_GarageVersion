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
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Runtime.InteropServices;

namespace GarageTool
{
    /// <summary>
    /// Interaction logic for EditItems.xaml
    /// </summary>
    public partial class EditItems : Window
    {




        DataBaseManager dataBaseManagerEdit;
        BindData bindDataEdit;
        Qrclass QrGenarator = new Qrclass();




        public EditItems()
        {
            try
            {
                dataBaseManagerEdit = new DataBaseManager(Stopwatch.StartNew());
                bindDataEdit = new BindData();


                InitializeComponent();

                lblId.Content = ((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[2];
                lblItem.Content = ((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[3];

                cmbItem.ItemsSource = ((MainWindow)System.Windows.Application.Current.MainWindow).ItemsList;
                cmbName.ItemsSource = ((MainWindow)System.Windows.Application.Current.MainWindow).ItemsList;
                cmbLokal.ItemsSource = bindDataEdit.BindLokal();
                cmbStatus.ItemsSource = bindDataEdit.BindStatus();
                rdbEdit.IsChecked = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

                
                
        }



       
        private string getInfotoQR()
        {

            return $"{cmbName.Text};{txtID.Text};{txtX.Text};{txtY.Text};{cmbLokal.Text};{txtOwner.Text};{cmbStatus.Text}";

        }

        private void Button_Click(object sender, RoutedEventArgs e) //Edit
        {
            if (cmbItem.SelectedIndex != -1 && cmbId.SelectedIndex != -1)
            {
                if (dataBaseManagerEdit.SetEditedItem(cmbItem.Text, cmbId.Text, cmbName.Text, txtID.Text, txtX.Text, txtY.Text, cmbLokal.Text, txtOwner.Text, cmbStatus.Text))
                {
                    Qrimage.Source = QrGenarator.createQR(getInfotoQR(), 150);
                    MessageBox.Show("Done");
                }
                else
                {
                    MessageBox.Show("try again");
                }

                
        
            }
            else
            {
                MessageBox.Show("Select Item and ID (serial number) to Edit");
            }
            
        }

        private bool CheckIfAllFields() // true jezeli wszystko wypełnione
        {




            if (cmbName.Text != "" && cmbLokal.Text != "" && cmbStatus.Text != "" && txtID.Text != "" && txtX.Text != "" && txtY.Text != "" && txtOwner.Text != "") 
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void cmbAdd_Click(object sender, RoutedEventArgs e)//Add
        {
            if (CheckIfAllFields())
            {
                if (dataBaseManagerEdit.AddNewItem(cmbName.Text, txtID.Text, txtX.Text, txtY.Text, cmbLokal.Text, txtOwner.Text.ToUpper(), cmbStatus.Text))
                {
                    Qrimage.Source = QrGenarator.createQR(getInfotoQR(), 200);

                    cmbName.SelectedIndex = -1;
                    txtID.Text = "";
                    txtX.Text = "";
                    txtY.Text = "";
                    cmbLokal.SelectedIndex = -1;
                    txtOwner.Text = "";
                    cmbStatus.SelectedIndex = -1;
                    MessageBox.Show("Done");
                    Qrimage.Source = null;
                }
                else
                {
                    MessageBox.Show("try again");
                }
                
            }
            else
            {
                MessageBox.Show("Fill all fields to Add");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbId.Items.Refresh();

            // dodac bindowanie dla edycji 
            Item item = dataBaseManagerEdit.GetItem(cmbItem.Text, cmbId.Text);
            if (item != null)
            {
                cmbName.Text = item.Name;
                txtID.Text = item.Id;
                txtX.Text = item.Position.X.ToString();
                txtY.Text = item.Position.Y.ToString();
                cmbLokal.SelectedItem = item.Lokal;
                txtOwner.Text = item.Owner;
                cmbStatus.SelectedItem = item.Status;
            }
            else
            {
                MessageBox.Show("Error");
            }


            Qrimage.Source = QrGenarator.createQR(getInfotoQR(),50);
        }

        private void cmbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbItem.Items.Refresh();
            cmbId.ItemsSource = dataBaseManagerEdit.GetIdItems(cmbItem.Text,true);
            cmbId.Items.Refresh();
        }

        private void rdbAdd_Checked(object sender, RoutedEventArgs e)
        {
            cmbName.IsEnabled = true;
            txtID.IsEnabled = true;
        }

        private void rdbEdit_Checked(object sender, RoutedEventArgs e)
        {
            cmbName.IsEnabled = false;
            txtID.IsEnabled = false;
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbStatus.Items.Refresh();
            Qrimage.Source = QrGenarator.createQR(getInfotoQR(), 200);
        }


        private void btnSaveQR_Click(object sender, RoutedEventArgs e)
        {
            if (Qrimage.Source != null)
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Qrimage.Source));
                string path = Directory.GetCurrentDirectory();
                using (FileStream stream = new FileStream($"{path}/{cmbName.Text}_{txtID.Text}.jpg", FileMode.Create))
                encoder.Save(stream);
            }
            else
            {
                MessageBox.Show("creatte QR code");
            }
        }

        private void cmbLokal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtOwner.Text = Environment.UserName.ToUpper();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txtOwner.Text = "SWS";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            List < Item > Lista_itemow= new List<Item>();

            foreach (var item in ((MainWindow)System.Windows.Application.Current.MainWindow).ItemsList)
            {
                try
                {


                for (int i = 0; i < int.Parse(dataBaseManagerEdit.GetCountItems(item)); i++)
                {
                   List<string> IDReport = dataBaseManagerEdit.GetIdItems(item,true);
                    int j = 0;
                    while (j < IDReport.Count())
                    {
                        Lista_itemow.Add(dataBaseManagerEdit.GetItem(item, IDReport[j]));
                        j++;
                    }

                }
                }
                catch (Exception)
                {
                    
                    MessageBox.Show("please try again, during generate report was connection error");
                   // return;
                }


            }
            FileStream fs = new FileStream("Report.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            int ii = 0;
            while (ii < Lista_itemow.Count())
            {
                doc.Add(new iTextSharp.text.Paragraph(Lista_itemow[ii].Get_stringfor_QR(Lista_itemow[ii])));
                ii++;
            }
            doc.Close();

        }
    }
}
