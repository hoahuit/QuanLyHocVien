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
    public partial class QuanLyHoanTien : Form
    {
        private string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True";

        public QuanLyHoanTien()
        {
            InitializeComponent();
            LoadYeuCauHoanTien();
        }
        private void LoadYeuCauHoanTien()
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
                    hp.TongHocPhi,
                    COALESCE(SUM(tt.SoTien), 0) AS SoTienDaDong,
                    (hp.TongHocPhi - COALESCE(SUM(tt.SoTien), 0)) AS SoTienNo
                FROM DangKyHoc dk
                JOIN HocVien hv ON dk.MaHocVien = hv.MaHocVien
                JOIN KhoaHoc kh ON dk.MaKhoaHoc = kh.MaKhoaHoc
                LEFT JOIN HocPhi hp ON dk.MaDangKy = hp.MaDangKy
                LEFT JOIN ThanhToan tt ON hp.MaHocPhi = tt.MaHocPhi
                WHERE dk.TrangThai = N'Đang yêu cầu hoàn tiền'
                GROUP BY dk.MaDangKy, hv.TenHocVien, kh.TenKhoaHoc, hp.TongHocPhi;";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách yêu cầu hoàn tiền: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
     
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn yêu cầu hoàn tiền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maDangKy = dataGridView1.SelectedRows[0].Cells["MaDangKy"].Value.ToString();
                decimal tongHocPhi = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["TongHocPhi"].Value);

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();

                        try
                        {
                            // **Tạo mã giao dịch kế toán**
                            string maGiaoDich = "GD" + new Random().Next(10000, 99999);

                            // **Thêm vào bảng GiaoDichKeToan trước**
                            string insertGiaoDichQuery = "INSERT INTO GiaoDichKeToan (MaGiaoDich, NgayGiaoDich, MoTa) VALUES (@MaGiaoDich, GETDATE(), N'Hoàn tiền cho học viên')";
                            using (SqlCommand cmd = new SqlCommand(insertGiaoDichQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MaGiaoDich", maGiaoDich);
                                cmd.ExecuteNonQuery();
                            }

                            // **Thêm vào bảng ChiTietGiaoDich**
                            string insertChiTietQuery = "INSERT INTO ChiTietGiaoDich (MaChiTiet, MaGiaoDich, SoTienCo) VALUES (@MaChiTiet, @MaGiaoDich, @SoTienCo)";
                            using (SqlCommand cmd = new SqlCommand(insertChiTietQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MaChiTiet", "CTGD" + new Random().Next(10000, 99999));
                                cmd.Parameters.AddWithValue("@MaGiaoDich", maGiaoDich);
                                cmd.Parameters.AddWithValue("@SoTienCo", tongHocPhi);
                                cmd.ExecuteNonQuery();
                            }

                            // **Cập nhật trạng thái thành "Đã hoàn tiền"**
                            string updateQuery = "UPDATE DangKyHoc SET TrangThai = N'Đã hoàn tiền' WHERE MaDangKy = @MaDangKy";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MaDangKy", maDangKy);
                                cmd.ExecuteNonQuery();
                            }

                            // **Commit transaction**
                            transaction.Commit();

                            MessageBox.Show("Hoàn tiền thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadYeuCauHoanTien();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Lỗi khi xử lý hoàn tiền: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }
    }
}
