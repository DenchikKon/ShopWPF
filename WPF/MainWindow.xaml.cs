using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
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
using System.Configuration;
using System.Threading;
using System.Net.Mail;
using System.Windows.Ink;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using LiveCharts.Wpf;
using Live = LiveCharts;
using Microsoft.Office.Interop.Excel;
using LiveCharts;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static bool ChangeClient;
        public static int ChangeId;
        static string query;
        static string mainClientQuery = @"Select id, CONCAT(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'ФИО', Phone ""Телефон"", Discount as ""Скидка(%)"" from Client";
        static string mainBookingQuery = @"Select Booking.Id,CONCAT(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'Клиент',Service as ""Услуга"", Completing as ""Комплектуюшие"",  Format(OrderDate,'dd/MM/yyyy') as ""Дата поступления"", 
        Format(DateOfCompletion,'dd/MM/yyyy') as ""Завершено"" ,  Warranty as ""Гарантия"",
        Price as ""Цена"",
        Paid as ""Оплачено"" From Booking  inner join Client 
        on Booking.IdClient = Client.Id";
        private void ReplaceWordStub(string stubToReplace, string text , Word.Document wordDoc)
        {
            var range = wordDoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
           
            //DbManager.LoadChart(chart);
            //comboBoxClient.ItemsSource = null;
            //comboBoxClient.ItemsSource = null;
            //query = @"Select id,CONCAT(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'Клиент' from Client";
            //DbManager.LoadDataInComboBox(query, comboBoxClient, "id", "Клиент");
            //DbManager.LoadDataInComboBox(query, comboBoxClientFilter, "id", "Клиент");

            //DbManager.LoadChart(chart);
        }

        private void buttonAddClient_Click(object sender, RoutedEventArgs e)
        {
           
            if (!EnterName.Equals(string.Empty) && !EnterSurname.Equals(string.Empty) && !EnterPatronymic.Equals(string.Empty) &&
                !EnterPhone.Equals(string.Empty) && int.TryParse(EnterDiscount.Text, out int discount))
            {
                if (!ChangeClient) {
                query = $"Insert Into Client Values(N'{EnterName.Text}',N'{EnterSurname.Text}',N'{EnterPatronymic.Text}','{EnterPhone.Text}',{EnterDiscount.Text})";
                DbManager.ExecuteQuery(query);
                DbManager.LoadData(gridClient, mainClientQuery);
                gridBooking.Columns[0].Visibility = Visibility.Hidden;
                EnterName.Text = string.Empty; EnterSurname.Text = string.Empty; EnterPatronymic.Text = string.Empty;
                EnterPhone.Text = string.Empty; EnterDiscount.Text = string.Empty;
                }
                else
                {
                    DataRowView row = (DataRowView)gridClient.SelectedItems[0];
                    query = $"Update Client Set Name = N'{EnterName.Text.Trim()}',Surname = N'{EnterSurname.Text.Trim()}'," +
                        $" Patronymic = N'{EnterPatronymic.Text.Trim()}', " +
                        $"Phone = N'{EnterPhone.Text.Trim()}', Discount = {EnterDiscount.Text.Trim()} where id = {ChangeId}";
                    DbManager.ExecuteQuery(query);
                    DbManager.LoadData(gridClient, mainClientQuery);
                    gridBooking.Columns[0].Visibility = Visibility.Hidden;
                    EnterName.Text = string.Empty; EnterSurname.Text = string.Empty; EnterPatronymic.Text = string.Empty;
                    EnterPhone.Text = string.Empty; EnterDiscount.Text = string.Empty;
                    ChangeClient = false;
                }
            }
            else
            {
                MessageBox.Show("Проверьте требуется заполнение всех полей(поле скидка должно быть числовым значением)");
            }
            comboBoxClient.ItemsSource = null;
            comboBoxClient.ItemsSource = null;
            query = @"Select id,CONCAT(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'Клиент' from Client";
            DbManager.LoadDataInComboBox(query, comboBoxClient, "id", "Клиент");
            DbManager.LoadDataInComboBox(query, comboBoxClientFilter, "id", "Клиент");
            DbManager.LoadChart(chart);
        }

        private void buttondeleteClient_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)gridClient.SelectedItems[0];
            query= $"Delete From Client Where id={row["id"]}";
            DbManager.ExecuteQuery(query);
            DbManager.LoadData(gridClient, mainClientQuery);
            gridBooking.Columns[0].Visibility = Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(enterDateOrder.SelectedDate > DateTime.Parse("01.01.2015") && !enterCompleting.Equals(string.Empty) && 
                int.TryParse(enterPrice.Text,out int price) && int.TryParse(enterWarranty.Text,out int warranty) &&
                comboBoxSevice.SelectedIndex != -1 && comboBoxClient.SelectedIndex != -1)
            {
                query = $"Insert Into Booking(OrderDate,IdClient,Completing,Price,Warranty,Service)  Values('{enterDateOrder.SelectedDate}',{comboBoxClient.SelectedValue}, " +
                    $"N'{enterCompleting.Text}',{price},N'{warranty}',N'{comboBoxSevice.Text}')";
                DbManager.ExecuteQuery(query);
                DbManager.LoadData(gridBooking, mainBookingQuery);
                gridBooking.Columns[0].Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)gridBooking.SelectedItems[0];
            query = $"Delete From Booking Where id={row["id"]}";
            DbManager.ExecuteQuery(query);
            DbManager.LoadData(gridBooking, mainBookingQuery);
            gridBooking.Columns[0].Visibility = Visibility.Hidden;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DbManager.LoadData(gridBooking, mainBookingQuery);
            gridBooking.Columns[0].Visibility = Visibility.Hidden;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (checkDiscountFilter.IsChecked == true)
            {
                query = mainBookingQuery + " where Paid = 1";
                if (comboBoxClientFilter.SelectedIndex != -1)
                    query += $" and Client.id = {comboBoxClientFilter.SelectedValue}";
                DbManager.LoadData(gridBooking, query);
                gridBooking.Columns[0].Visibility = Visibility.Hidden;
            }
            else if (comboBoxClientFilter.SelectedIndex != -1)
            {
                query = mainBookingQuery + $" where Client.id = {comboBoxClientFilter.SelectedValue}";
                DbManager.LoadData(gridBooking, query);
                gridBooking.Columns[0].Visibility = Visibility.Hidden;
            }
            else MessageBox.Show("Выберите свойство для сортировки");
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)gridBooking.SelectedItems[0];
            query = $"Update Booking Set Paid = 1 where id = {row[0]}";
            DbManager.ExecuteQuery(query);
            DbManager.LoadData(gridBooking, mainBookingQuery);
            gridBooking.Columns[0].Visibility = Visibility.Hidden;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)gridBooking.SelectedItems[0];
            query = $"Update Booking Set DateOfCompletion = '{DateTime.Now.ToString("yyyy.MM.dd")}' where id = {row[0]}";
            DbManager.ExecuteQuery(query);
            DbManager.LoadData(gridBooking, mainBookingQuery);
            gridBooking.Columns[0].Visibility = Visibility.Hidden;
        }

        private void ChangingClientData_Click(object sender, RoutedEventArgs e)
        {
            ChangeClient = true;
            DataRowView row = (DataRowView)gridClient.SelectedItems[0];
            ChangeId = int.Parse(row["id"].ToString());
            string[] splited = row[1].ToString().Split(' ');
            EnterName.Text = splited[0];
            EnterSurname.Text = splited[1];
            EnterPatronymic.Text = splited[2];
            EnterPhone.Text = row[2].ToString();
            EnterDiscount.Text = row[3].ToString();
        }

        private void OutputInWord_Click(object sender, RoutedEventArgs e)
        {
            var row = (DataRowView)gridBooking.SelectedItems[0];
            if(row != null)
            {
                var wordApp = new Word.Application();
                var wordDoc = wordApp.Documents.Open(@"D:\КППрактика\WPF\check.docx");
                ReplaceWordStub("<Name>", row["Клиент"].ToString(),wordDoc);
                ReplaceWordStub("<Date>", row["Дата поступления"].ToString(),wordDoc);
                ReplaceWordStub("<Service>", row["Услуга"].ToString(),wordDoc);
                ReplaceWordStub("<Completion>", row["Комплектуюшие"].ToString(),wordDoc);
                ReplaceWordStub("<Price>", row["Цена"].ToString(),wordDoc);
                wordApp.Visible = true;
            }
            
        }

        private void OutputInExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excelApp = new Excel.Application();
            excelApp.Application.Workbooks.Add(Type.Missing);
            for (int i = 0; i < gridBooking.Items.Count; i++)
            {
                DataRowView row = (DataRowView)gridBooking.Items[i];
                for (int j = 0; j < gridBooking.Columns.Count; j++)
                {
                    excelApp.Cells[i+1, j+1] = row[j].ToString();

                }
            }
            excelApp.Visible = true;
        }

        private void EnterSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count;
            if (EnterSearch.Text.Length > 0)
            {
                for (int i = 0; i < gridBooking.Items.Count; i++)
                {
                    count = 0;
                    DataRowView row = (DataRowView)gridBooking.Items[i];
                    for (int j = 1; j < gridBooking.Columns.Count; j++)
                    {
                        if (row[j].ToString().IndexOf(EnterSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            ((DataGridRow)gridBooking.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = true;
                            break;
                        }
                        else
                        {
                            ((DataGridRow)gridBooking.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = false;
                        }
                    }
                }
            }
            else
                gridBooking.UnselectAll();
        }

        private void tabControl_Selected(object sender, RoutedEventArgs e)
        {
            comboBoxClient.ItemsSource = null;
            comboBoxClient.ItemsSource = null;
            query = @"Select id,CONCAT(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'Клиент' from Client";
            DbManager.LoadDataInComboBox(query, comboBoxClient, "id", "Клиент");
            DbManager.LoadDataInComboBox(query, comboBoxClientFilter, "id", "Клиент");

            DbManager.LoadChart(chart);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

        }

        private void tabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            

            
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void TabItem_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DbManager.LoadData(gridBooking, mainBookingQuery);
            DbManager.LoadData(gridClient, mainClientQuery);
            gridBooking.Columns[0].Visibility = Visibility.Hidden;         
            DbManager.LoadChart(chart);
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
