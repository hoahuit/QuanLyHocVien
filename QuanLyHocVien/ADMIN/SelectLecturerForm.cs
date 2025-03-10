using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class SelectLecturerForm : Form
    {
        public string SelectedLecturer { get; private set; }
        private string connectionString; // Sử dụng tham số truyền vào

        public SelectLecturerForm(string connString)
        {
            InitializeComponent();
            this.connectionString = connString; // Gán chuỗi kết nối từ tham số
            LoadGiangVien();
        }

        private void LoadGiangVien()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT MaGiangVien, TenGiangVien FROM GiangVien";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dtGiangVien = new DataTable();
                adapter.Fill(dtGiangVien);
                comboBox1.DataSource = dtGiangVien;
                comboBox1.DisplayMember = "TenGiangVien";
                comboBox1.ValueMember = "MaGiangVien";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                SelectedLecturer = comboBox1.SelectedValue.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn giáo viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Nếu bạn muốn đổi tên nút "Save" thành "OK" trong designer, hãy cập nhật sự kiện tương ứng
    }
}