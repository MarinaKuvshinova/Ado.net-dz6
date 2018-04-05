using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace _26._03
{
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source = HOME-ПК; Initial Catalog = BookShop; Integrated Security = true;";
        SqlConnection sqlConnection = null;
        DataTable dataTable = null;
        public Form1()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connectionString);
            btnStart.Click += btnStart_Click;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            label1.Text = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                label1.Text = comboBox1.SelectedValue.ToString();
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string sql = textBox1.Text;
            dataGridView1.DataSource = null;
            comboBox1.DataSource = null;
            dataGridView1.DataSource = await GetDataAsync(sql);
            comboBox1.DataSource = await GetDataComboboxAsync(sql); ;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "count";
            label1.Text = comboBox1.SelectedValue.ToString();
        }

        private async Task<object> GetDataComboboxAsync(string sql)
        {
            if (sql == "select * from Books" || sql == "select*from Books" || sql == "select * from books")
            {
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();
                    //or sql += " order by ID_Author";
                    string sql2 = @"select CONCAT(Authors.FirstName,' ', Authors.LastName) as Name, COUNT(Books.ID_Author) as count from Books inner join Authors on Authors.ID_Author=Books.ID_Author group by CONCAT(Authors.FirstName,' ', Authors.LastName)";
                    SqlCommand sqlCommand = new SqlCommand(sql2, sqlConnection);
                    dataTable = new DataTable();
                    SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                    int line = 0;
                    do
                    {
                        while (await sqlDataReader.ReadAsync())
                        {
                            if (line++ == 0)
                            {
                                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                                {
                                    dataTable.Columns.Add(sqlDataReader.GetName(i));
                                }
                            }
                            DataRow dataRow = dataTable.NewRow();
                            for (int i = 0; i < sqlDataReader.FieldCount; i++)
                            {
                                dataRow[i] = await sqlDataReader.GetFieldValueAsync<Object>(i);
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                    } while (await sqlDataReader.NextResultAsync());
                }
            }
            else dataTable = null;
            return dataTable;
        }

        private async Task<object> GetDataAsync(string sql)
        {
            using (sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                dataTable = new DataTable();
                SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                int line = 0;
                do
                {
                    while (await sqlDataReader.ReadAsync())
                    {
                        if (line++ == 0)
                        {
                            for (int i = 0; i < sqlDataReader.FieldCount; i++)
                            {
                                dataTable.Columns.Add(sqlDataReader.GetName(i));
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            dataRow[i] = await sqlDataReader.GetFieldValueAsync<Object>(i);
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                } while (await sqlDataReader.NextResultAsync());
                return dataTable;
            }
        }
    }
}
