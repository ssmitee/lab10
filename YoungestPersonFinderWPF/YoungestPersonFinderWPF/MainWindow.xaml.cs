using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using YoungestPersonFinderWPF;

namespace YoungestPersonFinder
{
    public partial class MainWindow : Window
    {
        private List<Person> people; // список для хранения данных о людях

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик события нажатия кнопки "Загрузить данные"
        private void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    // Чтение данных из файла
                    people = ReadPeopleFromFile(filePath);

                    if (people.Any())
                    {
                        // Поиск самого молодого человека
                        Person youngestPerson = FindYoungestPerson();

                        if (youngestPerson != null)
                        {
                            // Отображение информации о самом молодом человеке
                            DisplayPersonInfo(youngestPerson);
                        }
                        else
                        {
                            resultsListBox.Items.Clear();
                            resultsListBox.Items.Add("Люди не найдены в списке.");
                        }
                    }
                    else
                    {
                        resultsListBox.Items.Clear();
                        resultsListBox.Items.Add("Данные не загружены из файла или файл пуст.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обработчик события нажатия кнопки "Сохранить результаты"
        private void btnSaveResults_Click(object sender, RoutedEventArgs e)
        {
            if (people == null || !people.Any())
            {
                MessageBox.Show("Нет данных для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Сохранение информации о самом молодом человеке в файл
                    SaveYoungestPersonToFile(filePath);
                    MessageBox.Show("Результаты успешно сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Метод для чтения данных о людях из файла
        private List<Person> ReadPeopleFromFile(string filePath)
        {
            List<Person> peopleList = new List<Person>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = line.Split(';'); // предполагаем, что данные разделены точкой с запятой

                    if (data.Length >= 17) // убедимся, что строка содержит все необходимые поля
                    {
                        // Создание объекта Person из данных строки
                        Person person = new Person
                        {
                            LastName = data[0].Trim(),
                            FirstName = data[1].Trim(),
                            MiddleName = data[2].Trim(),
                            Gender = data[3].Trim(),
                            Nationality = data[4].Trim(),
                            Height = int.Parse(data[5].Trim()),
                            Weight = int.Parse(data[6].Trim()),
                            DateOfBirth = DateTime.ParseExact(data[7].Trim(), "yyyy-MM-dd", null),
                            PhoneNumber = data[8].Trim(),
                            HomeAddress = new Address
                            {
                                PostalCode = data[9].Trim(),
                                Country = data[10].Trim(),
                                Region = data[11].Trim(),
                                District = data[12].Trim(),
                                City = data[13].Trim(),
                                Street = data[14].Trim(),
                                House = data[15].Trim(),
                                Apartment = data[16].Trim()
                            }
                        };

                        peopleList.Add(person);
                    }
                }
            }

            return peopleList;
        }

        // Метод для поиска самого молодого человека в списке
        private Person FindYoungestPerson()
        {
            if (people == null || people.Count == 0)
                return null;

            // Находим самого молодого человека по дате рождения
            return people.OrderBy(p => p.DateOfBirth).FirstOrDefault();
        }

        // Метод для отображения информации о человеке в ListBox
        private void DisplayPersonInfo(Person person)
        {
            resultsListBox.Items.Clear();
            resultsListBox.Items.Add($"Самый молодой человек:");
            resultsListBox.Items.Add($"Фамилия: {person.LastName}");
            resultsListBox.Items.Add($"Имя: {person.FirstName}");
            resultsListBox.Items.Add($"Отчество: {person.MiddleName}");
            resultsListBox.Items.Add($"Пол: {person.Gender}");
            resultsListBox.Items.Add($"Национальность: {person.Nationality}");
            resultsListBox.Items.Add($"Рост: {person.Height}");
            resultsListBox.Items.Add($"Вес: {person.Weight}");
            resultsListBox.Items.Add($"Дата рождения: {person.DateOfBirth:dd.MM.yyyy}");
            resultsListBox.Items.Add($"Телефон: {person.PhoneNumber}");
            resultsListBox.Items.Add($"Домашний адрес:");
            resultsListBox.Items.Add($"Почтовый индекс: {person.HomeAddress.PostalCode}");
            resultsListBox.Items.Add($"Страна: {person.HomeAddress.Country}");
            resultsListBox.Items.Add($"Область: {person.HomeAddress.Region}");
            resultsListBox.Items.Add($"Район: {person.HomeAddress.District}");
            resultsListBox.Items.Add($"Город: {person.HomeAddress.City}");
            resultsListBox.Items.Add($"Улица: {person.HomeAddress.Street}");
            resultsListBox.Items.Add($"Дом: {person.HomeAddress.House}");
            resultsListBox.Items.Add($"Квартира: {person.HomeAddress.Apartment}");
        }

        // Метод для сохранения информации о самом молодом человеке в файл
        private void SaveYoungestPersonToFile(string filePath)
        {
            if (people == null || people.Count == 0)
                throw new InvalidOperationException("Нет данных для сохранения.");

            // Находим самого молодого человека
            Person youngestPerson = FindYoungestPerson();

            if (youngestPerson == null)
                throw new InvalidOperationException("Не удалось найти самого молодого человека.");

            // Записываем информацию о самом молодом человеке в файл
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Фамилия: {youngestPerson.LastName}");
                writer.WriteLine($"Имя: {youngestPerson.FirstName}");
                writer.WriteLine($"Отчество: {youngestPerson.MiddleName}");
                writer.WriteLine($"Пол: {youngestPerson.Gender}");
                writer.WriteLine($"Национальность: {youngestPerson.Nationality}");
                writer.WriteLine($"Рост: {youngestPerson.Height}");
                writer.WriteLine($"Вес: {youngestPerson.Weight}");
                writer.WriteLine($"Дата рождения: {youngestPerson.DateOfBirth:dd.MM.yyyy}");
                writer.WriteLine($"Телефон: {youngestPerson.PhoneNumber}");
                writer.WriteLine($"Домашний адрес:");
                writer.WriteLine($"Почтовый индекс: {youngestPerson.HomeAddress.PostalCode}");
                writer.WriteLine($"Страна: {youngestPerson.HomeAddress.Country}");
                writer.WriteLine($"Область: {youngestPerson.HomeAddress.Region}");
                writer.WriteLine($"Район: {youngestPerson.HomeAddress.District}");
                writer.WriteLine($"Город: {youngestPerson.HomeAddress.City}");
                writer.WriteLine($"Улица: {youngestPerson.HomeAddress.Street}");
                writer.WriteLine($"Дом: {youngestPerson.HomeAddress.House}");
                writer.WriteLine($"Квартира: {youngestPerson.HomeAddress.Apartment}");
            }
        }
    }
}
