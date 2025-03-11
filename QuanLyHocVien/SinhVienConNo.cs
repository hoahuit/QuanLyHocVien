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
    public partial class SinhVienConNo : Form
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";

        public SinhVienConNo()
        {
            InitializeComponent();
            LoadSinhVienConNo();
        }
        private void LoadSinhVienConNo()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                    dk.MaDangKy,
                                    hv.TenHocVien,
                                    kh.TenKhoaHoc,
                                    hp.TongHocPhi,
                                    COALESCE(SUM(tt.SoTien), 0) AS SoTienDaDong,
                                    (hp.TongHocPhi - COALESCE(SUM(tt.SoTien), 0)) AS SoTienNo
                                FROM DangKyHoc dk
                                JOIN HocVien hv ON dk.MaHocVien = hv.MaHocVien
                                JOIN KhoaHoc kh ON dk.MaKhoaHoc = kh.MaKhoaHoc
                                LEFT JOIN HocPhi hp ON dk.MaDangKy = hp.MaDangKy
                                LEFT JOIN ThanhToan tt ON hp.MaHocPhi = tt.MaHocPhi
                                WHERE dk.TrangThai = N'Đang nợ học phí'
                                GROUP BY dk.MaDangKy, hv.TenHocVien, kh.TenKhoaHoc, hp.TongHocPhi;
                                ";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách công nợ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên để thanh toán nợ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maDangKy = dataGridView1.SelectedRows[0].Cells["MaDangKy"].Value.ToString();
            decimal soTienNo = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["SoTienNo"].Value);

            if (soTienNo <= 0)
            {
                MessageBox.Show("Sinh viên này không còn nợ học phí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // **Lấy MaHocPhi**
                    string getHocPhiQuery = "SELECT MaHocPhi FROM HocPhi WHERE MaDangKy = @MaDangKy";
                    string maHocPhi = "";
                    using (SqlCommand cmd = new SqlCommand(getHocPhiQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        object result = cmd.ExecuteScalar();
                        if (result != null) maHocPhi = result.ToString();
                    }

                    if (maHocPhi == "")
                    {
                        MessageBox.Show("Không tìm thấy học phí của sinh viên này!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string insertThanhToanQuery = "INSERT INTO ThanhToan (MaThanhToan, MaHocPhi, NgayThanhToan, SoTien , PhuongThucThanhToan ) VALUES (@MaThanhToan, @MaHocPhi, GETDATE(), @SoTien , N'Tiền Mặt')";
                    using (SqlCommand cmd = new SqlCommand(insertThanhToanQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaThanhToan", "TT" + new Random().Next(10000, 99999));
                        cmd.Parameters.AddWithValue("@MaHocPhi", maHocPhi);
                        cmd.Parameters.AddWithValue("@SoTien", soTienNo);
                        cmd.ExecuteNonQuery();
                    }

                    string updateTrangThaiQuery = "UPDATE DangKyHoc SET TrangThai = 'Đã thanh toán' WHERE MaDangKy = @MaDangKy";
                    using (SqlCommand cmd = new SqlCommand(updateTrangThaiQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Thanh toán công nợ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSinhVienConNo(); // Cập nhật lại danh sách
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán công nợ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
