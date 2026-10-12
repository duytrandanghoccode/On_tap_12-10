using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bai4_8_Async_ProgressBar
{
    public class FormMain : Form
    {
        private MenuStrip mnsMain;
        private StatusStrip stsMain;
        private ToolStripStatusLabel lblNguoiDung;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblThoiGian;
        private Timer tmrClock;

        public FormMain()
        {
            // Cấu hình cửa sổ MDI Parent
            this.Text = "Bài 4.8: Lập Trình Bất Đồng Bộ (Async/Await) & Tiến Trình ProgressBar";
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new Size(1100, 750);

            InitUI();
        }

        private void InitUI()
        {
            // 1. TẠO MENU STRIP (BÀI 4.7)
            mnsMain = new MenuStrip();

            // Menu Hệ thống
            ToolStripMenuItem mniHeThong = new ToolStripMenuItem("Hệ thống");
            mniHeThong.DropDownItems.Add("Đăng xuất", null, (s, e) => LogOut());
            mniHeThong.DropDownItems.Add("Thoát", null, (s, e) => Application.Exit());

            // Menu Danh mục
            ToolStripMenuItem mniDanhMuc = new ToolStripMenuItem("Danh mục");
            mniDanhMuc.DropDownItems.Add("Quản lý Sinh viên (Async)", null, (s, e) => OpenChildForm<FormStudent>());

            // Menu Cửa sổ (Quản lý MDI Child)
            ToolStripMenuItem mniWindow = new ToolStripMenuItem("Cửa sổ");
            mniWindow.DropDownItems.Add("Cascade", null, (s, e) => this.LayoutMdi(MdiLayout.Cascade));
            mniWindow.DropDownItems.Add("Tile Horizontal", null, (s, e) => this.LayoutMdi(MdiLayout.TileHorizontal));
            mniWindow.DropDownItems.Add("Tile Vertical", null, (s, e) => this.LayoutMdi(MdiLayout.TileVertical));

            mnsMain.Items.AddRange(new ToolStripItem[] { mniHeThong, mniDanhMuc, mniWindow });

            // 2. TẠO STATUS STRIP (CẬP NHẬT TRẠNG THÁI REALTIME)
            stsMain = new StatusStrip();
            lblNguoiDung = new ToolStripStatusLabel("Người dùng: Admin  |  ");
            lblStatus = new ToolStripStatusLabel("Trạng thái: Sẵn sàng  |  ");
            lblThoiGian = new ToolStripStatusLabel(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            stsMain.Items.AddRange(new ToolStripItem[] { lblNguoiDung, lblStatus, lblThoiGian });

            // 3. TIMER ĐỒNG HỒ THỜI GIAN THỰC
            tmrClock = new Timer { Interval = 1000, Enabled = true };
            tmrClock.Tick += (s, e) => lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            // Nối các Control vào Form
            this.Controls.Add(stsMain);
            this.Controls.Add(mnsMain);
            this.MainMenuStrip = mnsMain;
        }

        // THUẬT TOÁN MỞ FORM CON KHÔNG TRÙNG LẶP (BÀI 4.7)
        public void OpenChildForm<T>() where T : Form, new()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    UpdateStatus($"Đã kích hoạt cửa sổ: {child.Text}");
                    return;
                }
            }

            T newChild = new T { MdiParent = this };
            newChild.Show();
            UpdateStatus($"Đã mở cửa sổ: {newChild.Text}");
        }

        // HÀM NHẬN THÔNG ĐIỆP CẬP NHẬT STATUSSTRIP TỪ FORM CON (BÀI 4.7 & 4.8)
        public void UpdateStatus(string message)
        {
            lblStatus.Text = $"Trạng thái: {message}  |  ";
        }

        private void LogOut()
        {
            foreach (Form child in this.MdiChildren) child.Close();
            UpdateStatus("Đã đăng xuất hệ thống.");
        }
    }
}