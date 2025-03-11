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
using static System.Net.Mime.MediaTypeNames;

namespace QuanLyHocVien
{
    public partial class DangNhap : Form
    {
        SqlConnection conn;
        SqlDataAdapter da;
        DataTable dt;
        public DangNhap()
        {
            InitializeComponent();
            conn = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=q;Integrated Security=True");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string tenDangNhap = textBox1.Text.Trim();
                string matKhau = textBox2.Text.Trim();
                string query = "SELECT mataikhoan, tentaikhoan, mahocvien, maketoan, Quyen FROM TaiKhoan WHERE TenTaiKhoan = @TenTaiKhoan AND MatKhau = @MatKhau";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenTaiKhoan", tenDangNhap);
                cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    int quyen = Convert.ToInt32(dt.Rows[0]["Quyen"]);
                    string tentaikhoan = dt.Rows[0]["tentaikhoan"].ToString();
                    string mahocvien = dt.Rows[0]["mahocvien"].ToString(); 
                    string maketoan = dt.Rows[0]["maketoan"].ToString(); 

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();

                    // Navigate to the correct form based on the role (quyen)
                    if (quyen == 0)
                    {
                        MainHocSinh hocSinhKhoaHocForm = new MainHocSinh(tentaikhoan, mahocvien);
                        hocSinhKhoaHocForm.ShowDialog();
                    }
                    else if (quyen == 2)
                    {
                        MainKeToan mainKeToanForm = new MainKeToan(tentaikhoan,maketoan);
                        mainKeToanForm.ShowDialog();
                    }
                    else if (quyen == 1)
                    {
                        TrangChu trangChuForm = new TrangChu(tentaikhoan);
                        trangChuForm.ShowDialog();
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }


    }
}
