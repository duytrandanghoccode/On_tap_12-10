using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bai4_7_MDIFramework
{
    public class FormMain : Form
    {
        // 1. Khai báo các Control
        private MenuStrip mnsMain;
        private StatusStrip stsMain;
        private ToolStripStatusLabel lblNguoiDung;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblThoiGian;
        private Timer tmrClock;

        public FormMain()
        {
            // Cấu hình thuộc tính Form cha MDI
            this.Text = "Hệ Thống Quản Lý - MDI Framework (Code Pure)";
            this.IsMdiContainer = true; // Bật giao diện Form Cha
            this.WindowState = FormWindowState.Maximized;
            this.Size = new Size(1000, 700);

            // Tự khởi tạo toàn bộ giao diện bằng code
            InitUI();
        }

        private void InitUI()
        {
            // --- A. TẠO MENU STRIP ---
            mnsMain = new MenuStrip();

            // Menu Hệ thống
            ToolStripMenuItem mniHeThong = new ToolStripMenuItem("Hệ thống");
            ToolStripMenuItem mniDangXuat = new ToolStripMenuItem("Đăng xuất", null, (s, e) => LogOut());
            ToolStripMenuItem mniThoat = new ToolStripMenuItem("Thoát", null, (s, e) => Application.Exit());
            mniHeThong.DropDownItems.AddRange(new ToolStripItem[] { mniDangXuat, mniThoat });

            // Menu Danh mục
            ToolStripMenuItem mniDanhMuc = new ToolStripMenuItem("Danh mục");
            ToolStripMenuItem mniQLSV = new ToolStripMenuItem("Quản lý Sinh viên", null, (s, e) => OpenChildForm<FormStudent>());
            mniDanhMuc.DropDownItems.Add(mniQLSV);

            // Menu Cửa sổ (Sắp xếp MDI)
            ToolStripMenuItem mniWindow = new ToolStripMenuItem("Cửa sổ");
            mniWindow.DropDownItems.Add("Cascade", null, (s, e) => this.LayoutMdi(MdiLayout.Cascade));
            mniWindow.DropDownItems.Add("Tile Horizontal", null, (s, e) => this.LayoutMdi(MdiLayout.TileHorizontal));
            mniWindow.DropDownItems.Add("Tile Vertical", null, (s, e) => this.LayoutMdi(MdiLayout.TileVertical));

            mnsMain.Items.AddRange(new ToolStripItem[] { mniHeThong, mniDanhMuc, mniWindow });

            // --- B. TẠO STATUS STRIP ---
            stsMain = new StatusStrip();
            lblNguoiDung = new ToolStripStatusLabel("Người dùng: Admin  |  ");
            lblStatus = new ToolStripStatusLabel("Trạng thái: Sẵn sàng  |  ");
            lblThoiGian = new ToolStripStatusLabel(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            stsMain.Items.AddRange(new ToolStripItem[] { lblNguoiDung, lblStatus, lblThoiGian });

            // --- C. TẠO TIMER ĐỒNG HỒ ---
            tmrClock = new Timer { Interval = 1000, Enabled = true };
            tmrClock.Tick += (s, e) => lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            // --- D. GÁN CÁC CONTROL VÀO FORM ---
            this.Controls.Add(stsMain);
            this.Controls.Add(mnsMain);
            this.MainMenuStrip = mnsMain;
        }

        // Mở Form con dạng MDI Độc quyền (Không trùng lặp)
        public void OpenChildForm<T>() where T : Form, new()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    UpdateStatus($"Đã chuyển tới: {child.Text}");
                    return;
                }
            }

            T newChild = new T { MdiParent = this };
            newChild.Show();
            UpdateStatus($"Đã mở cửa sổ: {newChild.Text}");
        }

        public void UpdateStatus(string message)
        {
            lblStatus.Text = $"Trạng thái: {message}  |  ";
        }

        private void LogOut()
        {
            foreach (Form child in this.MdiChildren) child.Close();
            UpdateStatus("Đã đăng xuất hệ thống");
        }
    }
}