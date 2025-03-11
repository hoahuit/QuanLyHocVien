using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuanLyHocVien
{
    public partial class MainHocSinh : Form
    {
        private string tendangnhap;
        private string mahocvien;
        public MainHocSinh(string TenDangNhap , string MaHocVien)
        {
            this.tendangnhap = TenDangNhap;
            this.mahocvien = MaHocVien;
            InitializeComponent();

   
            lblUserName.Text = tendangnhap;
            // Lấy kích thước màn hình
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            // Thiết lập kích thước form = 80% màn hình
            this.Width = (int)(screenWidth * 0.85);
            this.Height = (int)(screenHeight * 0.85);

            // Canh giữa form trên màn hình
            this.StartPosition = FormStartPosition.CenterScreen;

        }
        private void LoadFormToPanel(Form form)
        {
            panel3.Controls.Clear(); // Xóa nội dung cũ trong panel3
            form.TopLevel = false; // Quan trọng: Không cho form hoạt động độc lập
            form.FormBorderStyle = FormBorderStyle.None; // Loại bỏ border của form
            form.Dock = DockStyle.Fill; // Fill toàn bộ panel3
            panel3.Controls.Add(form); // Thêm form vào panel3
            form.Show(); // Hiển thị form
        }
        private void btnTaiLai_Click(object sender, EventArgs e)
        {
            LoadFormToPanel(new HocSinhKhoaHoc(tendangnhap,mahocvien));

        }
    }
}
