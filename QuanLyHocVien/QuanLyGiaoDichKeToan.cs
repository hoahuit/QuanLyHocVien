using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class QuanLyGiaoDichKeToan : Form
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";

        public QuanLyGiaoDichKeToan()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            // SQL query to fetch data
            string query = @"
                select distinct *  
  from ThanhToan
        , hocphi
         , dangkyhoc                       
 where thanhtoan.MaHocPhi = HocPhi.MaHocPhi and hocphi.MaDangKy = DangKyHoc.MaDangKy
            ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void QuanLyGiaoDichKeToan_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
