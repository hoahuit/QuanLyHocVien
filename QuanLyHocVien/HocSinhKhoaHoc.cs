using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class HocSinhKhoaHoc : Form
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";

        public HocSinhKhoaHoc()
        {
            InitializeComponent();
        }

        private void HocSinhKhoaHoc_Load(object sender, EventArgs e)
        {
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            kh.MaKhoaHoc, 
                            kh.TenKhoaHoc, 
                            gv.MaGiangVien,
                            gv.TenGiangVien,
                            kh.NgayBatDau,
                            kh.NgayKetThuc,
                            hp.TongHocPhi
                        FROM KhoaHoc kh
                        LEFT JOIN KhoaHoc_GiangVien khgv ON kh.MaKhoaHoc = khgv.MaKhoaHoc
                        LEFT JOIN GiangVien gv ON khgv.MaGiangVien = gv.MaGiangVien
                        LEFT JOIN HocPhi hp ON kh.MaKhoaHoc = hp.MaKhoaHoc";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int GetCurrentHocSinhID()
        {
            return 1; // Thay 1 bằng ID thật của học sinh
        }

        private void btnDangKyHoc_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một khóa học để đăng ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int maHocSinh = GetCurrentHocSinhID(); // Lấy ID của học sinh đăng nhập
                    int maKhoaHoc = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["MaKhoaHoc"].Value);

                    // Kiểm tra xem học sinh đã đăng ký chưa
                    string checkQuery = "SELECT COUNT(*) FROM DangKyKhoaHoc WHERE MaHocSinh = @MaHocSinh AND MaKhoaHoc = @MaKhoaHoc";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@MaHocSinh", maHocSinh);
                        checkCmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Bạn đã đăng ký khóa học này trước đó!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Thêm vào bảng DangKyKhoaHoc
                    string insertQuery = "INSERT INTO DangKyKhoaHoc (MaHocSinh, MaKhoaHoc, NgayDangKy) VALUES (@MaHocSinh, @MaKhoaHoc, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHocSinh", maHocSinh);
                        cmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một khóa học để hủy đăng ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        int maHocSinh = GetCurrentHocSinhID();
                        string maKhoaHoc = dataGridView2.SelectedRows[0].Cells["MaKhoaHoc"].Value.ToString();

                        string deleteQuery = "DELETE FROM DangKyHoc WHERE MaHocPhi = @MaHocPhi AND MaKhoaHoc = @MaKhoaHoc";
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaHocPhi", maHocSinh);
                            cmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Hủy đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCourses();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hủy đăng ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }
    }
}
