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
            LoadCourses();
            LoadRegisteredCourses();
        }

        private void HocSinhKhoaHoc_Load(object sender, EventArgs e)
        {
            LoadCourses();
            LoadRegisteredCourses();
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
                    gv.TenGiangVien,
                    kh.NgayBatDau,
                    kh.NgayKetThuc,
                    hp.TongHocPhi, 
                    hp.GiamGia
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

                    string maHocVien ="HV001"; // Lấy ID học viên đăng nhập
                    string maKhoaHoc = dataGridView1.SelectedRows[0].Cells["MaKhoaHoc"].Value.ToString();
                    decimal tongHocPhi = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["TongHocPhi"].Value);
                    decimal giamGia = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["GiamGia"].Value);

                    decimal hocPhiSauGiam = tongHocPhi - giamGia;

                    // Kiểm tra xem học viên đã đăng ký chưa
                    string checkQuery = "SELECT COUNT(*) FROM DangKyHoc WHERE MaHocVien = @MaHocVien AND MaKhoaHoc = @MaKhoaHoc";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@MaHocVien", maHocVien);
                        checkCmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Bạn đã đăng ký khóa học này trước đó!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // **Tạo mã đăng ký tự động**
                    string maDangKy = "DK" + new Random().Next(10000, 99999);

                    // **Chèn vào bảng DangKyHoc**
                    string insertQuery = "INSERT INTO DangKyHoc (MaDangKy, MaHocVien, MaKhoaHoc, NgayDangKy, TrangThai) VALUES (@MaDangKy, @MaHocVien, @MaKhoaHoc, GETDATE(), N'Chưa thanh toán')";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        cmd.Parameters.AddWithValue("@MaHocVien", maHocVien);
                        cmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                        cmd.ExecuteNonQuery();
                    }

                    // **Chèn vào bảng HocPhi**
                    string insertHocPhiQuery = "INSERT INTO HocPhi (MaHocPhi, MaDangKy, TongHocPhi, GiamGia, MaKhoaHoc) VALUES (@MaHocPhi, @MaDangKy, @TongHocPhi, @GiamGia, @MaKhoaHoc)";
                    using (SqlCommand cmd = new SqlCommand(insertHocPhiQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHocPhi", "HP" + maDangKy);
                        cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        cmd.Parameters.AddWithValue("@TongHocPhi", tongHocPhi);
                        cmd.Parameters.AddWithValue("@GiamGia", giamGia);
                        cmd.Parameters.AddWithValue("@MaKhoaHoc", maKhoaHoc);
                        cmd.ExecuteNonQuery();
                    }
                    LoadCourses();
                    LoadRegisteredCourses();
                    MessageBox.Show("Đăng ký thành công! Học phí sau giảm: " + hocPhiSauGiam.ToString("N2") + " VND", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadRegisteredCourses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    dk.MaDangKy,
kh.MaKhoaHoc,
                    kh.TenKhoaHoc, 
                    dk.NgayDangKy,
                    dk.TrangThai,
                    hp.TongHocPhi,
                    hp.GiamGia,
                    (hp.TongHocPhi - ISNULL(hp.GiamGia, 0)) AS HocPhiSauGiam
                FROM DangKyHoc dk
                JOIN KhoaHoc kh ON dk.MaKhoaHoc = kh.MaKhoaHoc
                LEFT JOIN HocPhi hp ON dk.MaDangKy = hp.MaDangKy
                WHERE dk.MaHocVien = @MaHocVien";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@MaHocVien", "HV001");

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView2.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khóa học đã đăng ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    string maHocVien = "HV001";
                    string maDangKy = dataGridView2.SelectedRows[0].Cells["MaDangKy"].Value.ToString();
                    string maKhoaHoc = dataGridView2.SelectedRows[0].Cells["MaKhoaHoc"].Value.ToString();

                    // **Kiểm tra trạng thái trước khi hủy**
                    string checkQuery = "SELECT TrangThai FROM DangKyHoc WHERE MaDangKy = @MaDangKy";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        object result = checkCmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("Không tìm thấy khóa học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string trangThai = result.ToString();

                        if (trangThai != "Chưa thanh toán")
                        {
                            MessageBox.Show("Bạn chỉ có thể hủy đăng ký khóa học chưa thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // **Xóa bản ghi trong `HocPhi` trước**
                    string deleteHocPhiQuery = "DELETE FROM HocPhi WHERE MaDangKy = @MaDangKy";
                    using (SqlCommand cmd = new SqlCommand(deleteHocPhiQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        cmd.ExecuteNonQuery();
                    }

                    // **Xóa bản ghi trong `DangKyHoc`**
                    string deleteDangKyQuery = "DELETE FROM DangKyHoc WHERE MaDangKy = @MaDangKy";
                    using (SqlCommand cmd = new SqlCommand(deleteDangKyQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        cmd.ExecuteNonQuery();
                    }

                    // **Cập nhật lại danh sách**
                    LoadCourses();
                    LoadRegisteredCourses();
                    MessageBox.Show("Hủy đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hủy đăng ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn khóa học để yêu cầu hoàn tiền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maDangKy = dataGridView2.SelectedRows[0].Cells["MaDangKy"].Value.ToString();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // **Cập nhật trạng thái khóa học thành "Đang yêu cầu hoàn tiền"**
                        string updateQuery = "UPDATE DangKyHoc SET TrangThai = N'Đang yêu cầu hoàn tiền' WHERE MaDangKy = @MaDangKy";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Yêu cầu hoàn tiền đã được gửi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRegisteredCourses();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi gửi yêu cầu hoàn tiền: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
        }
    }
}
