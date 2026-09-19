using System;

namespace StudentManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StudentDAO studentDAO = new StudentDAO();
            

            Student s1 = new Student(
                "SV001",
                "Nguyen Van An",
                20,
                "Ha Noi"
            );

            Student s2 = new Student(
                "SV002",
                "Tran Van Binh",
                21,
                "Hai Phong"
            );

            Student s3 = new Student(
                "SV003",
                "Nguyen Minh Duc",
                19,
                "Ha Nam"
            );


            studentDAO.Add(s1);
            studentDAO.Add(s2);
            studentDAO.Add(s3);
            

            Console.WriteLine("DANH SACH SINH VIEN");

            foreach (Student student in studentDAO.GetAlls())
            {
                Console.WriteLine(student);
            }

            
            Console.WriteLine("\nTIM SINH VIEN CO ID = SV002");

            Student findStudent = studentDAO.GetById("SV002");

            if (findStudent != null)
            {
                Console.WriteLine(findStudent);
            }
            else
            {
                Console.WriteLine("Khong tim thay sinh vien.");
            }

            

            Console.WriteLine("\nTIM SINH VIEN CO TEN NGUYEN");

            foreach (Student student in studentDAO.GetByName("Nguyen"))
            {
                Console.WriteLine(student);
            }

            

            Student studentEdit = new Student(
                "SV002",
                "Tran Van Binh Edited",
                22,
                "Ha Noi"
            );

            if (studentDAO.Edit(studentEdit))
            {
                Console.WriteLine("\nSua sinh vien thanh cong.");
            }
            else
            {
                Console.WriteLine("\nKhong tim thay sinh vien.");
            }


            // In lại danh sách
            Console.WriteLine("\nDANH SACH SAU KHI SUA");

            foreach (Student student in studentDAO.GetAlls())
            {
                Console.WriteLine(student);
            }

            

            if (studentDAO.Delete("SV001"))
            {
                Console.WriteLine("\nXoa sinh vien SV001 thanh cong.");
            }
            else
            {
                Console.WriteLine("\nKhong tim thay sinh vien.");
            }


            // In danh sách sau khi xóa
            Console.WriteLine("\nDANH SACH SAU KHI XOA");

            foreach (Student student in studentDAO.GetAlls())
            {
                Console.WriteLine(student);
            }


            Console.ReadKey();
        }
    }
}
