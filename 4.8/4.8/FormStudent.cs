using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai4_8_Async_ProgressBar
{
    public class FormStudent : Form
    {
        // 1. Quản lý dữ liệu Binding (Bài 4.6)
        private BindingList<Student> listStudents = new BindingList<Student>();
        private BindingSource bs = new BindingSource();

        // 2. Control Lập trình Bất đồng bộ Async & ProgressBar (Bài 4.8)
        private Button btnNapDuLieu;
        private ProgressBar prgLoading;
        private Label lblPercent;

        // 3. Control Nhập liệu & Chức năng (Bài 4.6)
        private TextBox txtMaSV, txtHoTen, txtTuoi;
        private ComboBox cboGioiTinh;
        private PictureBox picAvatar;
        private Button btnChonAnh, btnThem, btnCapNhat, btnXoa, btnXuatCSV;
        private DataGridView dgvSinhVien;

        // 4. ContextMenuStrip Menu chuột phải (Bài 4.7)
        private ContextMenuStrip cmsDataGrid;

        public FormStudent()
        {
            this.Text = "Quản Lý Sinh Viên - Async & Progress";
            this.Size = new Size(880, 620);

            InitUI();
            SetupBindings();
        }

        // --- A. DỰNG GIAO DIỆN BẰNG CODE (PURE CODE) ---
        private void InitUI()
        {
            // Panel top: Thanh nạp dữ liệu Async (Bài 4.8)
            Panel pnlAsync = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.FromArgb(240, 240, 240) };

            btnNapDuLieu = new Button
            {
                Text = "Nạp dữ liệu (Async)",
                Location = new Point(10, 10),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnNapDuLieu.Click += BtnNapDuLieu_Click;

            prgLoading = new ProgressBar
            {
                Location = new Point(170, 12),
                Size = new Size(320, 21),
                Minimum = 0,
                Maximum = 100
            };

            lblPercent = new Label
            {
                Text = "0%",
                Location = new Point(500, 14),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            pnlAsync.Controls.AddRange(new Control[] { btnNapDuLieu, prgLoading, lblPercent });

            // Panel Input: Khối nhập thông tin & Avatar (Bài 4.6)
            Panel pnlInput = new Panel { Dock = DockStyle.Top, Height = 170 };

            Label lblMa = new Label { Text = "Mã SV:", Location = new Point(10, 15), AutoSize = true };
            txtMaSV = new TextBox { Location = new Point(80, 12), Size = new Size(180, 23) };

            Label lblTen = new Label { Text = "Họ tên:", Location = new Point(10, 45), AutoSize = true };
            txtHoTen = new TextBox { Location = new Point(80, 42), Size = new Size(180, 23) };

            Label lblTuoi = new Label { Text = "Tuổi:", Location = new Point(10, 75), AutoSize = true };
            txtTuoi = new TextBox { Location = new Point(80, 72), Size = new Size(180, 23) };

            Label lblGioi = new Label { Text = "Giới tính:", Location = new Point(10, 105), AutoSize = true };
            cboGioiTinh = new ComboBox { Location = new Point(80, 102), Size = new Size(180, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cboGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ" });

            // PictureBox Avatar
            picAvatar = new PictureBox
            {
                Location = new Point(280, 12),
                Size = new Size(110, 110),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            btnChonAnh = new Button { Text = "Chọn Ảnh", Location = new Point(280, 128), Size = new Size(110, 25) };
            btnChonAnh.Click += BtnChonAnh_Click;

            // Nút chức năng
            btnThem = new Button { Text = "Thêm", Location = new Point(420, 12), Size = new Size(95, 30) };
            btnCapNhat = new Button { Text = "Cập nhật", Location = new Point(420, 50), Size = new Size(95, 30) };
            btnXoa = new Button { Text = "Xóa", Location = new Point(420, 88), Size = new Size(95, 30) };
            btnXuatCSV = new Button { Text = "Xuất CSV", Location = new Point(420, 126), Size = new Size(95, 30) };

            btnThem.Click += BtnThem_Click;
            btnCapNhat.Click += BtnCapNhat_Click;
            btnXoa.Click += BtnXoa_Click;
            btnXuatCSV.Click += BtnXuatCSV_Click;

            pnlInput.Controls.AddRange(new Control[] {
                lblMa, txtMaSV, lblTen, txtHoTen, lblTuoi, txtTuoi, lblGioi, cboGioiTinh,
                picAvatar, btnChonAnh, btnThem, btnCapNhat, btnXoa, btnXuatCSV
            });

            // ContextMenuStrip chuột phải (Bài 4.7)
            cmsDataGrid = new ContextMenuStrip();
            cmsDataGrid.Items.Add("Xóa dòng đã chọn", null, BtnXoa_Click);
            cmsDataGrid.Items.Add("Làm mới danh sách", null, (s, e) => bs.ResetBindings(false));

            // DataGridView
            dgvSinhVien = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ContextMenuStrip = cmsDataGrid
            };

            // Thêm các Panel vào Form
            this.Controls.Add(dgvSinhVien);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlAsync);
        }

        // --- B. SETUP DATA BINDING 2 CHIỀU (BÀI 4.6) ---
        private void SetupBindings()
        {
            bs.DataSource = listStudents;
            dgvSinhVien.DataSource = bs;

            txtMaSV.DataBindings.Add("Text", bs, "StudentId", true, DataSourceUpdateMode.OnPropertyChanged);
            txtHoTen.DataBindings.Add("Text", bs, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTuoi.DataBindings.Add("Text", bs, "Age", true, DataSourceUpdateMode.OnPropertyChanged);
            cboGioiTinh.DataBindings.Add("Text", bs, "Gender", true, DataSourceUpdateMode.OnPropertyChanged);

            bs.CurrentChanged += Bs_CurrentChanged;
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            if (bs.Current is Student st && !string.IsNullOrEmpty(st.AvatarPath) && File.Exists(st.AvatarPath))
            {
                if (picAvatar.Image != null) picAvatar.Image.Dispose();
                using (var stream = new FileStream(st.AvatarPath, FileMode.Open, FileAccess.Read))
                {
                    picAvatar.Image = Image.FromStream(stream);
                }
            }
            else
            {
                if (picAvatar.Image != null) picAvatar.Image.Dispose();
                picAvatar.Image = null;
            }
        }

        // --- C. XỬ LÝ LẬP TRÌNH BẤT ĐỒNG BỘ ASYNC/AWAIT & PROGRESSBAR (BÀI 4.8) ---

        // 1. Phương thức bất đồng bộ nạp dữ liệu nặng (async Task<List<Student>>)
        private async Task<List<Student>> LoadDataAsync(IProgress<int> progress)
        {
            // Task.Run đưa tiến trình xử lý sang Background Thread để không làm đơ UI Thread
            return await Task.Run(() =>
            {
                List<Student> result = new List<Student>();
                int totalItems = 100; // Giả lập nạp 100 bản ghi

                for (int i = 1; i <= totalItems; i++)
                {
                    // Giả lập độ trễ nạp dữ liệu từ CSDL / File (30ms/bản ghi)
                    Thread.Sleep(30);

                    result.Add(new Student
                    {
                        StudentId = $"SV{i:D3}",
                        FullName = $"Sinh viên {i}",
                        Age = 18 + (i % 5),
                        Gender = (i % 2 == 0) ? "Nam" : "Nữ",
                        AvatarPath = ""
                    });

                    // Tính phần trăm và báo về UI Thread thông qua IProgress
                    int percent = (i * 100) / totalItems;
                    progress?.Report(percent);
                }

                return result;
            });
        }

        // 2. Sự kiện Click nút Nạp dữ liệu (async void)
        private async void BtnNapDuLieu_Click(object sender, EventArgs e)
        {
            // YÊU CẦU BÀI 4.8: Tự động khóa nút bấm khi đang chạy
            btnNapDuLieu.Enabled = false;
            prgLoading.Value = 0;
            lblPercent.Text = "0%";

            try
            {
                // Sử dụng Progress<int> để nhận báo cáo tiến trình realtime gửi về UI
                var progress = new Progress<int>(percent =>
                {
                    prgLoading.Value = percent;
                    lblPercent.Text = $"{percent}%";

                    // Cập nhật phần trăm tiến trình lên StatusStrip của FormMain (MDI Parent)
                    NotifyParentStatus($"Đang nạp dữ liệu sinh viên bất đồng bộ... {percent}%");
                });

                NotifyParentStatus("Bắt đầu tác vụ nạp dữ liệu...");

                // Gọi hàm nạp Async với từ khóa await (UI giao diện hoàn toàn mượt mà)
                List<Student> loadedData = await LoadDataAsync(progress);

                // Cập nhật dữ liệu vào BindingList
                listStudents.Clear();
                foreach (var st in loadedData)
                {
                    listStudents.Add(st);
                }

                // Thông báo hoàn tất
                NotifyParentStatus($"Nạp hoàn tất {loadedData.Count} sinh viên thành công!");
                MessageBox.Show($"Đã nạp xong {loadedData.Count} sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi trong quá trình nạp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NotifyParentStatus("Lỗi tác vụ nạp dữ liệu!");
            }
            finally
            {
                // YÊU CẦU BÀI 4.8: Tự động mở lại nút bấm trong khối finally
                btnNapDuLieu.Enabled = true;
            }
        }

        // --- D. CÁC THAO TÁC CƠ BẢN (THÊM, CẬP NHẬT, XÓA, ẢNH, CSV) ---
        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã SV!", "Thông báo");
                return;
            }

            Student st = new Student
            {
                StudentId = txtMaSV.Text,
                FullName = txtHoTen.Text,
                Age = int.TryParse(txtTuoi.Text, out int age) ? age : 0,
                Gender = cboGioiTinh.Text,
                AvatarPath = ""
            };

            listStudents.Add(st);
            bs.MoveLast();
            NotifyParentStatus($"Đã thêm sinh viên: {st.FullName} ({st.StudentId})");
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            bs.EndEdit();
            bs.ResetBindings(false);
            NotifyParentStatus("Đã cập nhật thông tin sinh viên!");
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (bs.Current is Student st)
            {
                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa SV {st.FullName}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    string id = st.StudentId;
                    bs.RemoveCurrent();
                    NotifyParentStatus($"Đã xóa sinh viên {id}.");
                }
            }
        }

        private void BtnChonAnh_Click(object sender, EventArgs e)
        {
            if (bs.Current is Student st)
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "File Ảnh (*.jpg;*.png)|*.jpg;*.png" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        st.AvatarPath = ofd.FileName;
                        Bs_CurrentChanged(null, null);
                        bs.ResetBindings(false);
                        NotifyParentStatus($"Đã chọn ảnh cho SV: {st.StudentId}");
                    }
                }
            }
        }

        private void BtnXuatCSV_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "File CSV (*.csv)|*.csv", FileName = "DanhSachSV.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.WriteLine("Mã SV,Họ Tên,Tuổi,Giới Tính,AvatarPath");
                        foreach (var st in listStudents)
                        {
                            sw.WriteLine($"\"{st.StudentId}\",\"{st.FullName}\",{st.Age},\"{st.Gender}\",\"{st.AvatarPath}\"");
                        }
                    }
                    NotifyParentStatus("Đã xuất danh sách sinh viên ra file CSV thành công!");
                }
            }
        }

        // Truyền thông điệp lên FormMain
        private void NotifyParentStatus(string message)
        {
            if (this.MdiParent is FormMain mainForm)
            {
                mainForm.UpdateStatus(message);
            }
        }
    }
}