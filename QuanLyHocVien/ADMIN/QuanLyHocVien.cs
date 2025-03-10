using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class QuanLyHocVien : Form
    {
        // Connection string (modify if needed)
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";

        private DataTable coursesTable;
        private SqlDataAdapter dataAdapter;

        public QuanLyHocVien()
        {
            InitializeComponent();
        }

        private void QuanLyKhoaHoc_Load(object sender, EventArgs e)
        {
            LoadCourses();
        }

        private void LoadCourses()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM HocVien";
                dataAdapter = new SqlDataAdapter(query, connection);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                coursesTable = new DataTable();
                dataAdapter.Fill(coursesTable);
                dataGridView1.DataSource = coursesTable;

                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            coursesTable.Rows.Add();
            int lastRowIndex = dataGridView1.Rows.Count - 1;
            dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells[0]; // Move cursor to the new row
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khóa học để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM HocVien";
                    dataAdapter = new SqlDataAdapter(query, connection);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                    dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                    dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                    dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                    dataAdapter.Update(coursesTable);
                }

                MessageBox.Show("Dữ liệu đã được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuanLyHocVien_Load(object sender, EventArgs e)
        {
            LoadCourses();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchValue = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(searchValue))
                {
                    coursesTable.DefaultView.RowFilter = string.Empty;
                }
                else
                {
                    coursesTable.DefaultView.RowFilter = string.Format(
                        "MaHocVien LIKE '%{0}%' OR TenHocVien LIKE '%{0}%'",
                        searchValue.Replace("'", "''")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
