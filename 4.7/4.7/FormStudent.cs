using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Bai4_7_MDIFramework
{
    public class FormStudent : Form
    {
        private DataGridView dgvSinhVien;
        private ContextMenuStrip cmsDataGrid;

        public FormStudent()
        {
            this.Text = "Quản Lý Sinh Viên";
            this.Size = new Size(600, 400);

            InitUI();
        }

        private void InitUI()
        {
            // 1. Tạo ContextMenuStrip (Menu chuột phải)
            cmsDataGrid = new ContextMenuStrip();
            cmsDataGrid.Items.Add("Xóa dòng", null, (s, e) => XoaDong());
            cmsDataGrid.Items.Add("Làm mới", null, (s, e) => dgvSinhVien.Refresh());

            // 2. Tạo DataGridView
            dgvSinhVien = new DataGridView
            {
                Dock = DockStyle.Fill, // Phủ kín cửa sổ
                ContextMenuStrip = cmsDataGrid,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Dữ liệu mẫu
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã SV");
            dt.Columns.Add("Họ Tên");
            dt.Columns.Add("Lớp");
            dt.Rows.Add("SV01", "Nguyễn Văn A", "20DTH01");
            dt.Rows.Add("SV02", "Trần Thị B", "20DTH02");

            dgvSinhVien.DataSource = dt;

            // 3. Thêm DataGridView vào Form con
            this.Controls.Add(dgvSinhVien);
        }

        private void XoaDong()
        {
            if (dgvSinhVien.CurrentRow != null && !dgvSinhVien.CurrentRow.IsNewRow)
            {
                dgvSinhVien.Rows.RemoveAt(dgvSinhVien.CurrentRow.Index);

                // Gửi thông điệp ngược lại cho FormMain (Form Cha)
                if (this.MdiParent is FormMain mainForm)
                {
                    mainForm.UpdateStatus("Đã xóa 1 dòng sinh viên");
                }
            }
        }
    }
}