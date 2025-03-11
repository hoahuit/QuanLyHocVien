using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class MainKeToan : Form
    {
        private string tentaikhoan;
        private string maketoan;
        public MainKeToan(string tentaikhoan, string maketoan)
        {
            InitializeComponent(); 
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
            this.tentaikhoan = tentaikhoan;
            this.maketoan = maketoan;
            label4.Text = "Bạn đang đăng nhập với tài khoản " + tentaikhoan;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void quảnLýKhóaHọcToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SinhVienChuaThanhToan t = new SinhVienChuaThanhToan(maketoan);
            t.ShowDialog();
        }

        private void danhSáchSinhViênNợToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SinhVienConNo t = new SinhVienConNo();  
            t.ShowDialog();
        }

        private void yêuCầuHoànTiềnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyHoanTien t = new QuanLyHoanTien(maketoan);
            t.ShowDialog();
        }
    }
}
