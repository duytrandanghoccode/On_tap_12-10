using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bai4_6_QuanLySinhVien
{
    public class Student
    {
        public string StudentId { get; set; }  // Mã SV
        public string FullName { get; set; }   // Họ tên
        public int Age { get; set; }           // Tuổi
        public string Gender { get; set; }     // Giới tính
        public string AvatarPath { get; set; } // Đường dẫn ảnh đại diện
    }
}