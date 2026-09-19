using System;
using System.Collections.Generic;

namespace StudentManagement
{
    public class StudentDAO
    {
        // Danh sách dùng để lưu sinh viên
        private List<Student> students = new List<Student>();


        // 1. Thêm sinh viên
        public bool Add(Student student)
        {
            // Kiểm tra ID đã tồn tại chưa
            Student s = GetById(student.Id);

            if (s != null)
            {
                return false;
            }

            students.Add(student);

            return true;
        }


        // 2. Sửa sinh viên
        public bool Edit(Student student)
        {
            Student s = GetById(student.Id);

            // Không tìm thấy
            if (s == null)
            {
                return false;
            }

            s.Name = student.Name;
            s.Age = student.Age;
            s.Address = student.Address;

            return true;
        }


        // 3. Xóa sinh viên theo ID
        public bool Delete(string id)
        {
            Student student = GetById(id);

            if (student == null)
            {
                return false;
            }

            students.Remove(student);

            return true;
        }


        // 4. Lấy toàn bộ danh sách sinh viên
        public List<Student> GetAlls()
        {
            return students;
        }


        // 5. Tìm sinh viên theo ID
        public Student GetById(string id)
        {
            foreach (Student student in students)
            {
                if (student.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return student;
                }
            }

            return null;
        }


        // 6. Tìm sinh viên theo tên
        public List<Student> GetByName(string name)
        {
            List<Student> result = new List<Student>();

            foreach (Student student in students)
            {
                if (student.Name.Contains(
                    name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(student);
                }
            }

            return result;
        }
    }
}
