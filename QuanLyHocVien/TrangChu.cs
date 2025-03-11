using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyHocVien
{
    public partial class TrangChu : Form
    {
        public TrangChu(string TN)
        {
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
            InitializeComponent();
            if (TN != null)
            {

                label4.Text = "Hello " + TN;
            }
            label3.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }



        private void quảnLýHọcViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyHocVien t = new QuanLyHocVien();
            t.ShowDialog();
        }

        private void thốngKêHọcViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyGiangVien quanLyGiangVien = new QuanLyGiangVien();
            quanLyGiangVien.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                DangNhap dangNhap = new DangNhap();
                dangNhap.ShowDialog();
            }
        }

        private void quảnLýHọcViênToolStripMenuItem1_Click(object sender, EventArgs e)
        {
      
        }

        private void quảnLýLớpHọcToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void quảnLýKhóaHọcToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            QuanLyKhoaHoc t = new QuanLyKhoaHoc();
            t.ShowDialog(this);
        }

        private void giaoDịchKếToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyGiaoDichKeToan t = new QuanLyGiaoDichKeToan();
            t.ShowDialog(this);
        }
    }
}
