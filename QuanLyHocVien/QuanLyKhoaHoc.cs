using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class QuanLyKhoaHoc : Form
    {
        // Connection string (modify if needed)
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";
        private DataTable coursesTable;
        private SqlDataAdapter dataAdapter;
        private SqlConnection connection; 
        public QuanLyKhoaHoc()
        {
            InitializeComponent();
        }

        private void QuanLyKhoaHoc_Load(object sender, EventArgs e)
        {
            LoadCourses();
            LoadKhoaHocGiangVien();

        }
    
        private void LoadCourses()
        {
           connection = new SqlConnection(connectionString); // Khởi tạo connection
            string query = "SELECT * FROM KhoaHoc";
            dataAdapter = new SqlDataAdapter(query, connection);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
            dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

            coursesTable = new DataTable();
            dataAdapter.Fill(coursesTable);
            dataGridView1.DataSource = coursesTable;

            dataGridView1.AllowUserToAddRows = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Thêm dòng mới vào bảng khóa học
            DataRow newRow = coursesTable.NewRow();
            coursesTable.Rows.Add(newRow);
            int lastRowIndex = dataGridView1.Rows.Count - 1;
            dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells[0];
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
                List<DataRow> newRows = coursesTable.AsEnumerable()
                    .Where(row => row.RowState == DataRowState.Added)
                    .ToList();

                if (newRows.Count == 0 && coursesTable.GetChanges() == null)
                {
                    MessageBox.Show("Không có thay đổi để lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string selectedLecturer = null;
                if (newRows.Count > 0)
                {
                    using (SelectLecturerForm selectForm = new SelectLecturerForm(connectionString))
                    {
                        if (selectForm.ShowDialog() == DialogResult.OK)
                        {
                            selectedLecturer = selectForm.SelectedLecturer;
                            if (string.IsNullOrEmpty(selectedLecturer))
                            {
                                MessageBox.Show("Vui lòng chọn giảng viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    dataAdapter.Update(coursesTable);

                    if (newRows.Count > 0 && !string.IsNullOrEmpty(selectedLecturer))
                    {
                        foreach (DataRow row in newRows)
                        {
                            string maKhoaHoc = row["MaKhoaHoc"].ToString();
                            if (string.IsNullOrEmpty(maKhoaHoc))
                            {
                                MessageBox.Show("Không thể lấy mã khóa học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                continue;
                            }

                            string insertQuery = "INSERT INTO KhoaHoc_GiangVien (MaKhoaHoc, MaGiangVien) VALUES (@MaKhoaHoc, @MaGiangVien)";
                            using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                                cmd.Parameters.AddWithValue("@MaGiangVien", selectedLecturer);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                MessageBox.Show("Dữ liệu đã được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCourses();
                LoadKhoaHocGiangVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadKhoaHocGiangVien()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                khgv.MaKhoaHoc,
                kh.TenKhoaHoc,
                khgv.MaGiangVien,
                gv.TenGiangVien
            FROM KhoaHoc_GiangVien khgv
            INNER JOIN KhoaHoc kh ON khgv.MaKhoaHoc = kh.MaKhoaHoc
            INNER JOIN GiangVien gv ON khgv.MaGiangVien = gv.MaGiangVien";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView2.DataSource = table;
                dataGridView2.AllowUserToAddRows = false;
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
