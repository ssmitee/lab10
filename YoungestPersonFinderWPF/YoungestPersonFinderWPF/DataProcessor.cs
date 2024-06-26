using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YoungestPersonFinderWPF
{
    public static class DataProcessor
    {
        public static List<Person> ReadPeopleFromFile(string filePath)
        {
            List<Person> people = new List<Person>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] data = line.Split(';');
                    Person person = new Person
                    {
                        LastName = data[0],
                        FirstName = data[1],
                        MiddleName = data[2],
                        Gender = data[3],
                        Nationality = data[4],
                        Height = int.Parse(data[5]),
                        Weight = int.Parse(data[6]),
                        DateOfBirth = DateTime.ParseExact(data[7], "yyyy.MM.dd", null),
                        PhoneNumber = data[8],
                        HomeAddress = new Address
                        {
                            PostalCode = data[9],
                            Country = data[10],
                            Region = data[11],
                            District = data[12],
                            City = data[13],
                            Street = data[14],
                            House = data[15],
                            Apartment = data[16]
                        }
                    };

                    people.Add(person);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}");
            }

            return people;
        }

        public static Person FindYoungestPerson(List<Person> people)
        {
            var youngestPerson = people.OrderBy(p => p.DateOfBirth).FirstOrDefault();
            return youngestPerson;
        }

        public static void SaveYoungestPersonToFile(string filePath)
        {
            try
            {
                List<Person> people = new List<Person>(); // Ваш список людей

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var person in people)
                    {
                        writer.WriteLine($"Фамилия: {person.LastName}");
                        writer.WriteLine($"Имя: {person.FirstName}");
                        writer.WriteLine($"Отчество: {person.MiddleName}");
                        writer.WriteLine($"Пол: {person.Gender}");
                        writer.WriteLine($"Национальность: {person.Nationality}");
                        writer.WriteLine($"Рост: {person.Height}");
                        writer.WriteLine($"Вес: {person.Weight}");
                        writer.WriteLine($"Дата рождения: {person.DateOfBirth:dd.MM.yyyy}");
                        writer.WriteLine($"Телефон: {person.PhoneNumber}");
                        writer.WriteLine($"Домашний адрес:");
                        writer.WriteLine($"Почтовый индекс: {person.HomeAddress.PostalCode}");
                        writer.WriteLine($"Страна: {person.HomeAddress.Country}");
                        writer.WriteLine($"Область: {person.HomeAddress.Region}");
                        writer.WriteLine($"Район: {person.HomeAddress.District}");
                        writer.WriteLine($"Город: {person.HomeAddress.City}");
                        writer.WriteLine($"Улица: {person.HomeAddress.Street}");
                        writer.WriteLine($"Дом: {person.HomeAddress.House}");
                        writer.WriteLine($"Квартира: {person.HomeAddress.Apartment}");
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}");
            }
        }
    }

}
