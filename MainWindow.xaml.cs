﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

//ZXing.net zainstalowany nugat do QR kodów


namespace GarageTool
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 





    public partial class MainWindow : Window
    {
        DataBaseManager dataBaseManager = new DataBaseManager(Stopwatch.StartNew());
        BindData bindData = new BindData();
        jezyki Jezyki = new jezyki();
        Qrclass QrGenerator = new Qrclass();
        FTP ftp;
        public List<string> Stringi;
        public List<string> ItemsList;
        public string Battery_toRent;
        private int m_nStart = 0;
        public string MyIDentyf="";
        DispatcherTimer dispatcherTimer, dispatcherTimer_wypo, dispatcherTimer_zwrt,Timer_couter, dispatcherTimer_test;
        public int count_down, count_down_started;
       public static Identyfikacja Identyfikacji_okno;



        public MainWindow()
        {
            InitializeComponent();
            count_down = 120; // 60 sekund na dzialanie inaczej wylogowanie
            count_down_started = count_down;


            Identyfikacji_okno = new Identyfikacja();
           

                try
                {
                Identyfikacji_okno.ShowDialog();

                if (MyIDentyf=="") // naprawic to gowno ... sprawdzanie zwracanej wartosci z zamykanego okna
                {
                    Identyfikacji_okno.End();
                    this.Close();
                    return;
                }
                }
                catch (Exception x)
                {

                    System.Windows.MessageBox.Show(x.ToString());
                }

            if (MyIDentyf=="")
            {
                this.Close();
            }

            txtUser.Text = MyIDentyf;
            imgtmpQR.Source = QrGenerator.createQR(MyIDentyf, 100);
            imgtmpQR.ToolTip = MyIDentyf.ToString().ToUpper();
            ftp = new FTP();


            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            if (!Directory.Exists(@"C:\ProgramData\Garage"))
            {
                Directory.CreateDirectory(@"C:\ProgramData\Garage");
            }

            
            if (!dataBaseManager.DB_connection) //jezeli nie ma polaczenia z BD
            {
                //lblConnectionStatus.Content = "Brak połączenia z Bazą Danych";
            }
            else
            {
               // lblConnectionStatus.Content = "";
            }
            //string[] tmpItems = bindData.BindItems(dataBaseManager.DB_connection);
            // bindowanie z cmbItem

            //bindowanie listy jezyków
            cmbLeng.ItemsSource = Jezyki.bindJezyki();
            cmbLeng.DisplayMemberPath = "Key";
            cmbLeng.SelectedValuePath = "Value";
            cmbLeng.SelectedIndex = 1;
            Stringi = Jezyki.select(cmbLeng.Text.ToString());
            updateStrings();
            //-----------------------------------------

            //bindowanie Itemów


            //cmbItem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cmbItem.AutoCompleteSource = AutoCompleteSource.ListItems;

            ItemsList = bindData.BindItems(dataBaseManager.DB_connection);
            cmbItem.ItemsSource = ItemsList;


            //-----------------------------------------
            //bindowanie my items

            txtMyItems.Text = bindData.getTextForUI(dataBaseManager.CheckMyBorrowItems(MyIDentyf));

            //---------------------

            //bindowanie moich itemow do zwrotu                
            cmbItemReturn.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());
            //---------------------------------
            Bind_AllAvailableItems();


            //DateTime dateTime = DateTime.UtcNow.Date;
            //var encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgtmpQR.Source));
            //string path = Directory.GetCurrentDirectory();
            //using (FileStream stream = new FileStream($"{path}/{dateTime.ToShortDateString()}.jpg", FileMode.Create))
            //encoder.Save(stream);
            //---- wybieram na poczatek selection mode dla zwrotow i wypozyczen
            rbnSelectmode.IsChecked = false;
            rbnQRmode.IsChecked = true;





            Timer_couter = new DispatcherTimer();
            Timer_couter.Tick += Timer_Counter;
            Timer_couter.Interval = new TimeSpan(0, 0, 1);
            Timer_couter.Start();




            if (!ftp.czyAktualnaversia())
            {
                btnUpdate.IsEnabled = true;
                btnUpdate.ToolTip = ftp.checkVersionOnServer();
                try
                {
                    System.Windows.MessageBox.Show("update available");
                }
                catch (Exception x)
                {
                    System.Windows.MessageBox.Show("update available");
                }
            }
            else
            {
                try
                {

                    System.Windows.MessageBox.Show("");
                }
                catch (Exception x)
                {

                    Console.WriteLine("");
                }
            }

        }

        static void OnProcessExit(object sender, EventArgs e)
        {

            Identyfikacji_okno.End();
          // dataBaseManager.SQLConnection.Close();
        }



        private void cmbItel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }// smiec po UI

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbId.Items.Refresh();
            Item tmp = dataBaseManager.GetItem(cmbItem.Text, cmbId.Text);
            try
            {
                SelectedQRItem.Source = QrGenerator.createQR(tmp.Get_stringfor_QR(tmp), 50);
                btnWyp.FontWeight = FontWeights.Bold;
                btnzwrot.IsEnabled = false;
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("try again");
            }
            if (cmbId.SelectedIndex!= -1)
            {
                lblSelItemQR.Content = "Selected Item:";
            }
            else lblSelItemQR.Content = "";

        } // smiec po UI

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // Point pos = new Point(1,2);
            try
            {
                if (cmbId.Text!="")
                {
                    count_down = count_down_started;

                    Item item = dataBaseManager.GetItem(cmbItem.Text, cmbId.Text);

                    Map map = new Map(item);
                    map.ShowDialog();
                }
               
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString() + " ALSO\n " + ee.ToString());
            }

        }

        private void cmbLeng_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbLeng.Items.Refresh();
            Stringi = Jezyki.select(cmbLeng.Text.ToString());
            updateStrings();
            if (Stringi!=null)
            {
                Stringi.Clear();
            }
            
        }
        private void updateStrings()
        {
            try
            {
                btnShow.Content = Stringi[0];
                btnWyp.Content = Stringi[1];
                lblId.Content = Stringi[2];
                lblItem.Content = Stringi[3];
                lblLang.Content = Stringi[4];
                TbRent.Header = Stringi[5];
                TbSett.Header = Stringi[6];
                lblAvailableItems.Content = Stringi[7]; //nr 8 to komunikat brak dostepu do BD nr 9 w windows1 labela
                lblUser.Content = Stringi[11];
                lblpass.Content = Stringi[12];
                btnpassconfirm.Content = Stringi[13];
            }
            catch (Exception)
            {


            }



        }

        private void cmbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbItem.Items.Refresh();
            txtAvailableItems.Text = bindData.GetcountItems(cmbItem.Text.ToString(), dataBaseManager.DB_connection, dataBaseManager.SQLConnection);
            
            if (cmbItem.SelectedIndex <= 5 && cmbItem.SelectedIndex != -1)
            {
                Window1 win2 = new Window1();
                win2.ShowDialog();

                //System.Windows.Forms.MessageBox.Show("battery to Rent : "+ Battery_toRent);

            }
            else
            {
                cmbId.ItemsSource = bindData.BindId(dataBaseManager.DB_connection, dataBaseManager.GetIdItems(cmbItem.Text.ToString(),false));
                cmbId.DisplayMemberPath = "Key";
                cmbId.SelectedValuePath = "Value";
            }



        }

        private void btnpassconfirm_Click(object sender, RoutedEventArgs e)
        {
            if (dataBaseManager.confirmadminuser(txtUser.Text.ToUpper(), txtPass.Password.ToString()))             
            {
                Timer_couter.Stop();
                EditItems win_Edit = new EditItems();
                win_Edit.ShowDialog();
                txtUser.Clear();
                txtPass.Clear();
                Refresh_bindedData();
                count_down = count_down_started;
                Timer_couter.Start();
            }
            else
            {
                txtUser.Clear();
                txtPass.Clear();
            }




        }

        private void btnWyp_Click(object sender, RoutedEventArgs e)
        {
            if (cmbItem.SelectedIndex != -1 && cmbId.SelectedIndex != -1)
            {
                //set do bazy danych 
                dataBaseManager.SetHire(imgtmpQR.ToolTip.ToString().ToUpper(), cmbItem.Text,cmbId.Text, DateTime.Today.ToShortDateString());
                Refresh_bindedData();

                cmbItem.SelectedIndex = -1;
                cmbId.SelectedIndex = -1;
                lblSelItemQR.Content = "";
                SelectedQRItem.Source = null;
                count_down = count_down_started;
                lblCounter.Background = new SolidColorBrush(Colors.White);
                if (rbnQRmode.IsChecked.Value)
                {
                    txtQRtoreadandfind.Focus();
                    dispatcherTimer_test.Start();
                }
                txtAvailableItems.Text = "0";
                btnWyp.FontWeight = FontWeights.Normal;
                btnzwrot.IsEnabled = true;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (cmbItemReturn.SelectedIndex != -1 && cmbIdReturn.SelectedIndex != -1)
            {
                //set do bazy danych 
                dataBaseManager.SetReturnItem(cmbItemReturn.Text,cmbIdReturn.Text, DateTime.Today.ToShortDateString());
                Refresh_bindedData();

                cmbItemReturn.SelectedIndex = -1;
                cmbIdReturn.SelectedIndex = -1;
                lblSelItemQR.Content = "";
                SelectedQRItem.Source = null;
                count_down = count_down_started;
                lblCounter.Background = new SolidColorBrush(Colors.White);
                if (rbnQRmode.IsChecked.Value)
                {
                    txtQRtoreadandfind.Focus();
                    dispatcherTimer_test.Start();
                }
                btnzwrot.FontWeight = FontWeights.Normal;
                btnWyp.IsEnabled = true;
            }

        }

        private void cmbItemReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbItemReturn.Items.Refresh();
            //bindowanie moich ID itemow do zwrotu                
            cmbIdReturn.ItemsSource = bindData.bindID_items_Toreturn(dataBaseManager.GetMyRentItemsNames(), cmbItemReturn.Text);
            //---------------------------------
        }

        private void Bind_AllAvailableItems()
        {
            List<string> itemki= new List<string>();

            foreach (var item in ItemsList)
            {
                itemki.Add(item + ": " + dataBaseManager.GetCountItems(item));
            }

            txtAllAvailableItems.Text = bindData.getTextForUI(itemki);

        }

        private void Refresh_bindedData()
        {

            //bindowanie Itemów
            
            ItemsList = bindData.BindItems(dataBaseManager.DB_connection);
            cmbItem.ItemsSource = ItemsList;
            cmbItemReturn.ItemsSource = ItemsList;
            cmbItem.Items.Refresh();
            cmbItemReturn.Items.Refresh();

            //-----------------------------------------
            //bindowanie my items

            txtMyItems.Text = bindData.getTextForUI(dataBaseManager.CheckMyBorrowItems(MyIDentyf));

            //---------------------

            Bind_AllAvailableItems();

            ItemsList = bindData.BindItems(dataBaseManager.DB_connection);
            cmbItem.ItemsSource = ItemsList;
            //bindowanie moich itemow do zwrotu                
            cmbItemReturn.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());


        }

        private void btnsaveQR_Click(object sender, RoutedEventArgs e)
        {
            if (imgtmpQR.Source != null)
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgtmpQR.Source));
                string path = Directory.GetCurrentDirectory();
                using (FileStream stream = new FileStream($"{path}/{Environment.UserName.ToUpper()}.jpg", FileMode.Create))
                    encoder.Save(stream);
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("creatte QR code");
            }
        }

        private void cmbIdReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbIdReturn.Items.Refresh();
            Item tmp = dataBaseManager.GetItem(cmbItemReturn.Text, cmbIdReturn.Text);
            btnzwrot.FontWeight = FontWeights.Bold;
            btnWyp.IsEnabled = false;

            try
            {
                SelectedQRItem.Source = QrGenerator.createQR(tmp.Get_stringfor_QR(tmp), 150);
     
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("try again");
            }
            if (cmbIdReturn.SelectedIndex != -1)
            {
                lblSelItemQR.Content = "Selected Item:";
            }
            else lblSelItemQR.Content = "";
        }

        public void FillQRData()
        {
            Item tmp = QrGenerator.GetItemFromstring(txtqrRead.Text);

            try
            {
                lblNameRE.Content = tmp.Name;
                lblIdRE.Content = tmp.Id;
                lblXRE.Content = tmp.Position.X;
                lblYRE.Content = tmp.Position.Y;
                lblLokalRE.Content = tmp.Lokal;
                lblOwnerRE.Content = tmp.Owner;
                lblStatusRE.Content = tmp.Status;
                lblQRContent.Content = "";
                return;
            }
            catch (Exception)
            {
                lblQRContent.Content = txtqrRead.Text;
                lblNameRE.Content = "";
                lblIdRE.Content = "";
                lblXRE.Content = "";
                lblYRE.Content = "";
                lblLokalRE.Content = "";
                lblOwnerRE.Content = "";
                lblStatusRE.Content = "";
                lblQRContent.Content = QrGenerator.decoded;
            }


        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (txtqrRead.Text!="")
            {
                FillQRData();
                txtqrRead.Text = "";
                dispatcherTimer.Stop();
            }
        }

        private void Timer_Tick_ReadWypozycz(object sender, EventArgs e)
        {
            if (txtQrReadWypo.Text != "")
            {
                try
                {
                    Item tmp = QrGenerator.GetItemFromstring(txtQrReadWypo.Text);

                    cmbItem.SelectedValue = tmp.Name;
                    cmbId.ItemsSource = bindData.BindId(dataBaseManager.DB_connection, dataBaseManager.GetIdItems(cmbItem.Text.ToString(), false));
                    cmbId.Items.Refresh();
                    cmbId.Text = tmp.Id;
                    txtQrReadWypo.Text = "";
                    
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show(txtQrReadWypo.Text);
                    txtQrReadWypo.Text = "";
                    dispatcherTimer_wypo.Stop();
                }
                dispatcherTimer_wypo.Stop();
            }
        }


        private void Timer_Tick_ReadZwrot(object sender, EventArgs e)
        {
            if (txtqedatazw.Text != "")
            {
                try
                {
                    Item tmp = QrGenerator.GetItemFromstring(txtqedatazw.Text);

                    cmbItemReturn.SelectedValue = tmp.Name;
                    cmbIdReturn.ItemsSource = bindData.bindID_items_Toreturn(dataBaseManager.GetMyRentItemsNames(), cmbItemReturn.Text);
                    cmbIdReturn.Items.Refresh();
                    cmbIdReturn.Text = tmp.Id;
                    txtqedatazw.Text = "";
                    
                }

                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show(txtqedatazw.Text);
                    txtQrReadWypo.Text = "";
                    dispatcherTimer_zwrt.Stop();
                }
                dispatcherTimer_zwrt.Stop();
            }
        }



        void initializeTimers()
        {
            
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        private void btnSelectQR_Click(object sender, RoutedEventArgs e)
        {
            txtqrRead.Text = "";
            dispatcherTimer = new DispatcherTimer();
            initializeTimers();
            txtqrRead.Focus();
            count_down = 60;

        }

        private void rbnQRmode_Checked(object sender, RoutedEventArgs e)
        {

            cmbItem.SelectedIndex = -1;
            cmbId.SelectedIndex = -1;
            cmbItemReturn.SelectedIndex = -1;
            cmbIdReturn.SelectedIndex = -1;
            rbnQRmode.IsChecked = true;
            rbnSelectmode.IsChecked = false;
            btnReadQR.IsEnabled = true;
            btnReadQRreturn.IsEnabled = true;
            txtQRtoreadandfind.Focus();

            try
            {
                dispatcherTimer_test.Stop();
            }
            catch (Exception)
            {
                
            }

            dispatcherTimer_test = new DispatcherTimer();
            dispatcherTimer_test.Tick += Timer_Tick_test;
            dispatcherTimer_test.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer_test.Start();

        }

        private void rbnSelectmode_Checked(object sender, RoutedEventArgs e)
        {
            rbnQRmode.IsChecked = false;
            rbnSelectmode.IsChecked = true;
            btnReadQR.IsEnabled = false;
            btnReadQRreturn.IsEnabled = false;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ftp.DownloadFTPFiles("ftp://garage%2540zadanko-z-zutu.cba.pl@zadanko-z-zutu.cba.pl/Garage/Garage.rar", "garage@zadanko-z-zutu.cba.pl", "Santiego94");
        }



        private void Timer_Tick_Identyfi(object sender, EventArgs e)
        {
            if (txtqedatazw.Text != "")
            {
                imgtmpQR.Source = QrGenerator.createQR(txtqedatazw.Text.ToUpper(), 100);
                imgtmpQR.ToolTip = txtqedatazw.Text.ToUpper();
                MyIDentyf = txtqedatazw.Text;
                txtqedatazw.Text = "";
                dispatcherTimer_zwrt.Stop();
            }
        }


        private void Timer_Tick_test(object sender, EventArgs e)
        {
            Item item_fromDB=null;
            if (txtQRtoreadandfind.Text != "")
            {
                Item tmp = QrGenerator.GetItemFromstring(txtQRtoreadandfind.Text);
                
                try
                {
                     item_fromDB = dataBaseManager.GetItem(tmp.Name, tmp.Id);
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Check your QR code");
                }

                try
                {
                    if (item_fromDB.Owner == "SWS")
                    {
                        cmbItem.Text = item_fromDB.Name;
                        cmbId.Text = item_fromDB.Id;
                    }
                    else if (item_fromDB.Owner == MyIDentyf)
                    {
                        cmbItemReturn.Text = item_fromDB.Name;
                        cmbIdReturn.Text = item_fromDB.Id;
                    }
                    txtQRtoreadandfind.Text = "";
                    dispatcherTimer_test.Stop();
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("error");
                    txtQRtoreadandfind.Text = "";

                }
                

            }
        }


        private void Timer_Counter(object sender, EventArgs e)
        {

            if (rbnQRmode.IsChecked.Value)
            {
                txtQRtoreadandfind.Focus();
            }

            if (count_down < (count_down_started / 2) && count_down > 15)
            {
                lblCounter.Content = count_down.ToString() + " s";
                lblCounter.Background = new SolidColorBrush(Colors.Yellow);
            }

            if (count_down < 15)
            {
                lblCounter.Content = count_down.ToString() + " s";
                lblCounter.Background = new SolidColorBrush(Colors.Red);
            }
            else
            lblCounter.Content = count_down.ToString() + " s";
            count_down--;
            
            if (count_down < 0)
            {
                Timer_couter.Stop();
                System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath); // to start new instance of application
                this.Close(); //to turn off current app
            }
        }

        private void btnRestartCounter_Click(object sender, RoutedEventArgs e)
        {
            lblCounter.Background = new SolidColorBrush(Colors.White);
            count_down = count_down_started;
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Timer_couter.Stop();
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath); // to start new instance of application
            this.Close(); //to turn off current app
        }

        private void btnIdentyfi_Click(object sender, RoutedEventArgs e)
        {
            imgtmpQR.Source = null;
            imgtmpQR.ToolTip = "";
            
            dispatcherTimer_zwrt = new DispatcherTimer();
            dispatcherTimer_zwrt.Tick += Timer_Tick_Identyfi;
            dispatcherTimer_zwrt.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer_zwrt.Start();

            txtqedatazw.Focus();
        }

        private void btnReadQRreturn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dispatcherTimer_zwrt.Stop();
            }
            catch (Exception)
            {

            }
            txtqedatazw.Text = "";
            dispatcherTimer_zwrt = new DispatcherTimer();
            dispatcherTimer_zwrt.Tick += Timer_Tick_ReadZwrot;
            dispatcherTimer_zwrt.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer_zwrt.Start();

            txtqedatazw.Focus();
        }

        private void btnReadQR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dispatcherTimer_wypo.Stop();
            }
            catch (Exception)
            {
                
            }
            txtqedata.Text = "";
            dispatcherTimer_wypo = new DispatcherTimer();
            dispatcherTimer_wypo.Tick += Timer_Tick_ReadWypozycz;
            dispatcherTimer_wypo.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer_wypo.Start();
            txtQrReadWypo.Focus();
          
        }
    }
}
