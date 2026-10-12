using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Bai4_6_QuanLySinhVien
{
    public partial class Form1 : Form
    {
        // 1. Khởi tạo danh sách hai chiều BindingList và bộ trung gian BindingSource
        private BindingList<Student> listStudents = new BindingList<Student>();
        private BindingSource bs = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Kết nối BindingList với BindingSource
            bs.DataSource = listStudents;
            // Gán nguồn dữ liệu cho DataGridView
            dgvSinhVien.DataSource = bs;

            // 2. Cấu hình DataBinding hai chiều cho các ô nhập liệu
            txtMaSV.DataBindings.Add("Text", bs, "StudentId", true, DataSourceUpdateMode.OnPropertyChanged);
            txtHoTen.DataBindings.Add("Text", bs, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            txtTuoi.DataBindings.Add("Text", bs, "Age", true, DataSourceUpdateMode.OnPropertyChanged);
            cboGioiTinh.DataBindings.Add("Text", bs, "Gender", true, DataSourceUpdateMode.OnPropertyChanged);

            // Bắt sự kiện khi người dùng click chọn dòng khác trên DataGridView
            bs.CurrentChanged += Bs_CurrentChanged;
        }

        // Tự động load ảnh đại diện khi chuyển chọn sinh viên khác
        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            if (bs.Current is Student currentStudent && !string.IsNullOrEmpty(currentStudent.AvatarPath) && File.Exists(currentStudent.AvatarPath))
            {
                // Giải phóng ảnh cũ tránh bị khóa file
                if (picAvatar.Image != null) picAvatar.Image.Dispose();

                using (var stream = new FileStream(currentStudent.AvatarPath, FileMode.Open, FileAccess.Read))
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

        // CHỨC NĂNG: Thêm Sinh Viên
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Sinh Viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            bs.MoveLast(); // Di chuyển tới dòng vừa thêm
        }

        // CHỨC NĂNG: Cập Nhật
        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            bs.EndEdit();
            bs.ResetBindings(false);
            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // CHỨC NĂNG: Xóa Sinh Viên
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (bs.Current != null)
            {
                var confirm = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    bs.RemoveCurrent();
                }
            }
        }

        // CHỨC NĂNG: Chọn Ảnh Đại Diện (OpenFileDialog)
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            if (bs.Current == null)
            {
                MessageBox.Show("Vui lòng chọn hoặc thêm một sinh viên trước!", "Thông báo");
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh đại diện cho Sinh Viên";
                ofd.Filter = "File Ảnh (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Student currentStudent = (Student)bs.Current;
                    currentStudent.AvatarPath = ofd.FileName;

                    // Hiển thị ảnh lên PictureBox ngay
                    Bs_CurrentChanged(null, null);
                    bs.ResetBindings(false);
                }
            }
        }

        // CHỨC NĂNG: Xuất file CSV (SaveFileDialog)
        private void btnXuatCSV_Click(object sender, EventArgs e)
        {
            if (listStudents.Count == 0)
            {
                MessageBox.Show("Chưa có sinh viên nào trong danh sách để xuất!", "Thông báo");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Chọn nơi lưu File CSV";
                sfd.Filter = "File CSV (*.csv)|*.csv";
                sfd.FileName = "DanhSachSinhVien.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Xuất mã hóa UTF-8 chuẩn có BOM để Excel mở không bị lỗi font Tiếng Việt
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            // 1. Ghi dòng tiêu đề
                            sw.WriteLine("Mã SV,Họ Tên,Tuổi,Giới Tính,Đường Dẫn Avatar");

                            // 2. Ghi từng hàng dữ liệu
                            foreach (var student in listStudents)
                            {
                                sw.WriteLine($"\"{student.StudentId}\",\"{student.FullName}\",{student.Age},\"{student.Gender}\",\"{student.AvatarPath}\"");
                            }
                        }

                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtTuoi_TextChanged(object sender, EventArgs e)
        {

        }
    }
}