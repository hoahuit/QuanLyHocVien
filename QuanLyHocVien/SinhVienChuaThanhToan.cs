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

    public partial class SinhVienChuaThanhToan : Form
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";

        public SinhVienChuaThanhToan()
        {
            InitializeComponent();
            LoadSinhVienChuaThanhToan();
            groupBox1.Visible = false;
        }
        private void LoadSinhVienChuaThanhToan()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    dk.MaDangKy,
                    hv.TenHocVien,
                    kh.TenKhoaHoc,
                    dk.TrangThai,
                    hp.TongHocPhi
                FROM DangKyHoc dk
                JOIN HocVien hv ON dk.MaHocVien = hv.MaHocVien
                JOIN KhoaHoc kh ON dk.MaKhoaHoc = kh.MaKhoaHoc
                LEFT JOIN HocPhi hp ON dk.MaDangKy = hp.MaDangKy
                WHERE dk.TrangThai = N'Chưa thanh toán'
                ORDER BY dk.NgayDangKy";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách sinh viên chưa thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maDangKy = dataGridView1.SelectedRows[0].Cells["MaDangKy"].Value.ToString();
                int soTienThanhToan = int.Parse(textBox1.Text);

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // **Lấy tổng học phí và số tiền đã đóng**
                        string getHocPhiQuery = @"
                SELECT hp.MaHocPhi, hp.TongHocPhi, 
                       COALESCE(SUM(tt.SoTien), 0) AS SoTienDaDong
                FROM HocPhi hp
                LEFT JOIN ThanhToan tt ON hp.MaHocPhi = tt.MaHocPhi
                WHERE hp.MaDangKy = @MaDangKy
                GROUP BY hp.MaHocPhi, hp.TongHocPhi";

                        string maHocPhi = "";
                        int tongHocPhi = 0;
                        int soTienDaDong = 0;

                        using (SqlCommand cmd = new SqlCommand(getHocPhiQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                maHocPhi = reader["MaHocPhi"].ToString();
                                tongHocPhi = Convert.ToInt32(reader["TongHocPhi"]);
                                soTienDaDong = Convert.ToInt32(reader["SoTienDaDong"]);
                            }
                            reader.Close();
                        }

                        if (maHocPhi == "")
                        {
                            MessageBox.Show("Không tìm thấy học phí!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int soTienNo = tongHocPhi  - soTienThanhToan;

                        // **Ghi nhận thanh toán**
                        string insertQuery = "INSERT INTO ThanhToan (MaThanhToan, MaHocPhi, NgayThanhToan, SoTien) VALUES (@MaThanhToan, @MaHocPhi, GETDATE(), @SoTien)";
                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaThanhToan", "TT" + new Random().Next(10000, 99999));
                            cmd.Parameters.AddWithValue("@MaHocPhi", maHocPhi);
                            cmd.Parameters.AddWithValue("@SoTien", soTienThanhToan);
                            cmd.ExecuteNonQuery();
                        }

                        // **Cập nhật trạng thái thanh toán**
                        string trangThai = soTienNo > 0 ? "Đang nợ học phí" : "Đã thanh toán";
                        string updateStatusQuery = "UPDATE DangKyHoc SET TrangThai = @TrangThai WHERE MaDangKy = @MaDangKy";
                        using (SqlCommand cmd = new SqlCommand(updateStatusQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                            cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show($"Thanh toán thành công! Số tiền còn nợ: {soTienNo:N0} VND", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSinhVienChuaThanhToan();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi ghi nhận thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
        }
    }
}
