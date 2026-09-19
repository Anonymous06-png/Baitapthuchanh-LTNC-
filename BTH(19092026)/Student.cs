using System;

namespace StudentManagement
{
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        // Constructor không tham số
        public Student()
        {
        }

        // Constructor có tham số
        public Student(string id, string name, int age, string address)
        {
            Id = id;
            Name = name;
            Age = age;
            Address = address;
        }

        // Hiển thị thông tin sinh viên
        public override string ToString()
        {
            return $"ID: {Id} | Name: {Name} | Age: {Age} | Address: {Address}";
        }
    }
}
