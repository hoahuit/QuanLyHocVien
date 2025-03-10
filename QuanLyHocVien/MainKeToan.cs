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
        public MainKeToan()
        {
            InitializeComponent();
        }

        private void quảnLýKhóaHọcToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SinhVienChuaThanhToan t = new SinhVienChuaThanhToan();
            t.ShowDialog();
        }

        private void danhSáchSinhViênNợToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SinhVienConNo t = new SinhVienConNo();  
            t.ShowDialog();
        }

        private void yêuCầuHoànTiềnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyHoanTien t = new QuanLyHoanTien();
            t.ShowDialog();
        }
    }
}
