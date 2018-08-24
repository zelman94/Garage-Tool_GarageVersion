using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace GarageTool
{
    class DataBaseManager
    {
        /// <summary>
        /// Matko bosko... w tej klasie jest taki syf, że o Jezus.
        /// </summary>
        public bool DB_connection; //jezeli jest polaczenie z BD 
        public MySqlConnection SQLConnection;
        private Stopwatch time;

        public DataBaseManager(Stopwatch time)
        {
            SQLConnection = ConnectToDB();
            
        }

        private MySqlConnection ConnectToDB()
        {
            try
            {
                string tmp = "server=10.128.64.144;" +
                                    "database=Garage;" +
                                   "uid=changer;" +
                                   "password=changer;SslMode=none;Connection Timeout=5";


                tmp = "server=zadanko-z-zutu.cba.pl;" +
                                   "database=zelman_2;" +
                                  "uid=zelman;" +
                                  "password=Santiego94;SslMode=none;";

                // string tmp = "Server=localhost;"
                //+ "Database=Garage;uid = root;password = 1234;SslMode =none;";

                MySqlConnection sqlConn = new MySqlConnection(tmp);
                sqlConn.Open();
               // sqlConn.Close();
                DB_connection = true;
                return sqlConn;

            }

            catch (Exception e)
            {
                Console.WriteLine("Wystąpił nieoczekiwany błąd!");
                Console.WriteLine(e.Message);
                DB_connection = false;
                return null;
            }

        }

        public string GetCountItems(string name) {
            string count ="";


            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                   // SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT Count(*) FROM `Items` WHERE `owner` = 'SWS' AND `name` = '{name}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            count = myReader.GetString(0);
                        }

                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                  
                }
            }
            else
            {
                try
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");                    
                }
            }

            return count;
        }
        public List<string> GetIdItems(string item, bool AllorNO) {
            List<string> AllAvailableId = new List<string>();
            if (DB_connection)
            {
                string zapytanie = "";
                if (AllorNO) // jezeli true to wszystkie
                {
                    zapytanie = $"SELECT id FROM `Items` WHERE name = '{item}'";
                }
                else
                {
                    zapytanie = $"SELECT id FROM `Items` WHERE status = 'FREE' AND name = '{item}'";
                }


                MySqlDataReader myReader;
                try
                {
                   // SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(zapytanie, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            AllAvailableId.Add(myReader.GetString(0));
                        }

                    }
                }
                catch (Exception )
                {
                }
            }
            else
            {
                //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
            }
            
            return AllAvailableId;
        }
        public void SetHire(string user, string item, string id,string when) { // wysylamy aktualizacje do BD że Item o id jest wypoyczony przez usera


            if (DB_connection)
            {


                string wyszukaj_item = $"SELECT * FROM `Items` WHERE `name` = '{item}' AND `id` = '{id}'";


                MySqlDataReader myReader;
                MySqlDataReader myReader2;
                try
                {
                    try
                    {
                        SQLConnection.Open();
                    }
                    catch (Exception)
                    {
                        //juz otwarte i fajnie
                    }
                    
                    using (MySqlCommand myCommand = new MySqlCommand(wyszukaj_item, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Read();

                        string edytuj = $"UPDATE `Items` SET `name`='{myReader.GetString(0)}',`id`='{myReader.GetString(1)}',`x`='{myReader.GetString(2)}',`y`='{myReader.GetString(3)}',`lokalizacja`='{myReader.GetString(4)}',`owner`='{user}',`status`='TAKEN' WHERE `name` = '{item}' AND `id` = '{id}'";
                        myReader.Close();
                        using (MySqlCommand myCommand2 = new MySqlCommand(edytuj, SQLConnection))
                        {
                            myReader2 = myCommand2.ExecuteReader(); //ustawienie wypozyczenie
                        }
                    }
                }
                catch (Exception )
                {

                }
            }
            else
            {

            }



        }
        public bool SetReturnItem(string item, string id, string when) { // wysylamy aktualizacje do BD że Item o id wrocil


            if (DB_connection)
            {
                    string wyszukaj_item = $"SELECT * FROM `Items` WHERE `name` = '{item}' AND `id` = '{id}'";


                MySqlDataReader myReader;
                MySqlDataReader myReader2;
                try
                {
                    try
                    {
                        SQLConnection.Open();
                    }
                    catch (Exception)
                    {
                       // jak ju otwarte to super
                    }

                    
                    using (MySqlCommand myCommand = new MySqlCommand(wyszukaj_item, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Read();

                        string edytuj = $"UPDATE `Items` SET `name`='{myReader.GetString(0)}',`id`='{myReader.GetString(1)}',`x`='{myReader.GetString(2)}',`y`='{myReader.GetString(3)}',`lokalizacja`='{myReader.GetString(4)}',`owner`='SWS',`status`='FREE' WHERE `name` = '{item}' AND `id` = '{id}'";
                        myReader.Close();
                        using (MySqlCommand myCommand2 = new MySqlCommand(edytuj, SQLConnection))
                        {
                            myReader2 = myCommand2.ExecuteReader(); //ustawienie zwrotu
                        }        
                    }
   
                }
                catch (Exception )
                {
                   // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                    SQLConnection.Close();
                }
            }
            else
            {
               // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
              
            }

            return false;
        }

        public string[] GetStock() { // pobieramy ile czego mamy i dajemy do raportu

            return null;
        }


        public List<string> CheckMyBorrowItems( string name) { 

            List<string> MyItems = new List<string>();
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT `name`,`id` FROM `Items` WHERE `owner` = '{name}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            MyItems.Add(myReader.GetString(0) + ", Id: " + myReader.GetString(1));
                        }

                    }
                }
                catch (Exception x)
                {
                    try
                    {
                        MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);

                    }
                    catch (Exception)
                    {

                    }
                    // SQLConnection.Close();
                }
            }
            else
            {
                try
                {
                   // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");

                }
                
            }

            return MyItems;

        }

        public bool confirmadminuser(string user,string pass)
        {
            bool confirmation = false;
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT Count(*) FROM `admin` WHERE `user` = '{user}' AND `pass` = '{pass}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        myReader.Read();
                        
                         if (myReader.GetString(0) != "0")
                         {
                            confirmation = true;
                         }       
                        
                    }

                }
                catch (Exception x)
                {
                    MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);

                }
            }
            else
            {
                try
                {
                    MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #2");

                }
               
            }

            try
            {
                //SQLConnection.Close();
            }
            catch (Exception)
            {

                Console.WriteLine("Cannot Close Connection to DB");

            }

            return confirmation;

        }

        public bool SetEditedItem(string OldName, string Old_ID, string Name, string ID, string X, string Y, string Lokal, string Owner, string Status)
        {
            // set do BD

            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                 

                        string edytuj = $"UPDATE `Items` SET `name`='{Name}',`id`='{ID}',`x`='{X}',`y`='{Y}',`lokalizacja`='{Lokal}',`owner`='{Owner}',`status`='{Status}' WHERE `name` = '{OldName}' AND `id` = '{Old_ID}'";
                      
                        using (MySqlCommand myCommand2 = new MySqlCommand(edytuj, SQLConnection))
                        {
                            myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                        }

                    return true;
                }
                catch (Exception )
                {

                    return false;
                }
            }
            else
            {

                return false;
            }

            //closeConnection();



        }

        public bool AddNewItem(string Name, string ID, string X, string Y, string Lokal, string Owner, string Status)
        {
            // set do BD

            if (DB_connection)
            {


                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();


                    string dodaj = $"INSERT INTO `Items` VALUES ('{Name}','{ID}','{X}','{Y}','{Lokal}','{Owner}','{Status}')";

                    using (MySqlCommand myCommand2 = new MySqlCommand(dodaj, SQLConnection))
                    {
                        myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                    }

                    return true;
                }
                catch (Exception )
                {

                    return false;
                }
            }
            else
            {

                return false;
            }
            
        }
       
        public List<Item> GetMyRentItemsNames()
        {
            List<Item> MyItems = new List<Item>();
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT `name`,`id`,`X`,`Y` FROM `Items` WHERE `owner` = '{((MainWindow)System.Windows.Application.Current.MainWindow).MyIDentyf}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {                  
                            Point tmp_pkt = new Point(Convert.ToDouble(myReader.GetString(2)), Convert.ToDouble(myReader.GetString(3)));
                            Item tmp = new Item(myReader.GetString(0), myReader.GetString(1),tmp_pkt);
                            MyItems.Add(tmp);
                        }

                    }
                }
                catch (Exception x)
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                }
            }
            else
            {
                try
                {
                   // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                }
            }
            return MyItems;
        }



        public Item GetItem(string name, string Id) // zwraca obiekt Item 
        {
            Item item ;
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    
                   
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT * FROM `Items` WHERE `name` = '{name}' AND `id` = '{Id}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            Point tmp_pkt = new Point(Convert.ToDouble(myReader.GetString(2)), Convert.ToDouble(myReader.GetString(3)));
                            item = new Item(myReader.GetString(0), myReader.GetString(1), tmp_pkt, myReader.GetString(5), myReader.GetString(4), myReader.GetString(6));
                            return item;
                        }

                    }
                }
                catch (Exception x)
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                    //closeConnection();
                }
            }
            else
            {
                try
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                   // closeConnection();
                }

            }
            return null;
        }

    }
}
    

