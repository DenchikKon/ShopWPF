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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Office.Interop.Word;
using Live = LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.Office.Interop.Excel;
using LiveCharts.Wpf.Charts.Base;

namespace WPF
{
    internal class DbManager
    {
        private static Live.SeriesCollection seriesCollection { get; set; }
        public static SqlConnection dataComputerFirm = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBaseComputerFirm"].ToString());
        public static void ConnectOpen()
        {
            dataComputerFirm.Open();
        }
        public static void ConnectClose()
        {
            dataComputerFirm.Close();
        }
        public static void LoadData(DataGrid data,string query )
        {
            dataComputerFirm.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(query,dataComputerFirm);
            System.Data.DataTable table = new System.Data.DataTable();
            adapter.Fill(table);
            dataComputerFirm.Close();
            data.ItemsSource = table.DefaultView;
        }
        public static void ExecuteQuery(string query)
        {
            dataComputerFirm.Open();
            SqlCommand command = new SqlCommand(query,dataComputerFirm);
            command.ExecuteNonQuery();
            dataComputerFirm.Close();
        }
        public static void LoadDataInComboBox(string query,ComboBox comboBox,string valueMember,string displayMember)
        {
            dataComputerFirm.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(query,dataComputerFirm);
            System.Data.DataTable table = new System.Data.DataTable();
            adapter.Fill(table);
            comboBox.ItemsSource = table.DefaultView;
            comboBox.DisplayMemberPath = displayMember;
            comboBox.SelectedValuePath= valueMember;
            dataComputerFirm.Close();
        }

        public static void LoadChart(LiveCharts.Wpf.CartesianChart chart)
        {
            chart.Series.Clear();
            List<string> key = new List<string>();
            List<int> value = new List<int>();
            SqlCommand command = new SqlCommand("Select Concat(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'FIO' from Client", DbManager.dataComputerFirm);
            DbManager.dataComputerFirm.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                key.Add(reader["FIO"].ToString());
            reader.Close();
            for (int i = 0; i < key.Count; i++)
            {
                SqlCommand countsql = new SqlCommand($"Select count(Booking.Id) from Booking" +
                    $" inner join Client on Client.Id = Booking.IdClient where Concat(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) = N'{key[i]}'", DbManager.dataComputerFirm);
                value.Add(int.Parse(countsql.ExecuteScalar().ToString()));
            }
            DbManager.dataComputerFirm.Close();

            ChartValues<int> values = new ChartValues<int>();
            values.AddRange(value);
            chart.Series.Add(new ColumnSeries()
            {
                Values = values
            });

            chart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Labels = key
            });
        }
    }
}

//List<string> key = new List<string>();
//List<int> value = new List<int>();
//SqlCommand command = new SqlCommand("Select Concat(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) as 'FIO' from Client", DbManager.dataComputerFirm);
//DbManager.dataComputerFirm.Open();
//SqlDataReader reader = command.ExecuteReader();
//while (reader.Read())
//    key.Add(reader["FIO"].ToString());
//reader.Close();
//foreach (var item in key)
//{
//    SqlCommand countsql = new SqlCommand($"Select count(Booking.Id) as 'Count' from Booking" +
//        $" inner join Client on Client.Id = Booking.IdClient where Concat(Trim(Name),' ',Trim(Surname),' ',Trim(Patronymic)) = '{item}'", DbManager.dataComputerFirm);
//    value.Add(int.Parse(countsql.ExecuteScalar().ToString()));
//}
//DbManager.dataComputerFirm.Close();

