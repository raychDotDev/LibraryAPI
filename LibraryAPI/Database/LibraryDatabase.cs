using LibraryAPI.Models;
using System;
using System.Data.SQLite;
using System.IO;

namespace LibraryAPI.Database
{
    public class LibraryDatabase
    {
        /// <summary>
        /// Путь к файлу базы данных
        /// </summary>
        private static readonly string _databaseFilePath = "./database.sqlite";

        /// <summary>
        /// Строка подключения к базе
        /// </summary>
        private static readonly string _connectionString = $"Data Source={_databaseFilePath};Version=3;";

        /// <summary>
        /// Статический конструктор класса LibraryDatabase.
        /// </summary>
        static LibraryDatabase()
        {
            Connect();
        }

        /// <summary>
        /// Метод инициализации подключения к датабазе.
        /// </summary>
        private static void Connect()
        {
            // Проверяем, существует ли файл базы данных
            if (!File.Exists(_databaseFilePath))
            {
                //Если нет - создаём его
                SQLiteConnection.CreateFile(_databaseFilePath);
            }

            //Формирование соединния к базе
            using var connection = new SQLiteConnection(_connectionString);
            //Открытие соединения
            connection.Open();

            //Формирование запроса на создание таблицы
            string request = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Genre TEXT NOT NULL
                );";

            //Формирование комманды, с учетом заготовленной выше строки запроса, и её выполнение
            using (var command = new SQLiteCommand(request, connection))
            {
                command.ExecuteNonQuery();
            }

            //Закрытие соединения
            connection.Close();
        }

        /// <summary>
        /// Метод добавления книги в датабазу по экземпляру книги
        /// </summary>
        /// <param name="book">Экземпляр класса Book, данные которого передаются в запрос 
        /// на добавление книги</param>
        public static void AddBook(Book book)
        {
            //Создание экземпляра соединения
            using (var connection = new SQLiteConnection(_connectionString))
            {
                //Открытие соединения
                connection.Open();

                //Формирование подготовленного запроса
                string request = @"
                INSERT INTO Books (title, author, genre, description) 
                VALUES (@title, @author, @genre, @description);";

                //Дополнение данных запроса и его последующее выполнение
                using (var command = new SQLiteCommand(request, connection))
                {
                    command.Parameters.AddWithValue("@title", book.Title);
                    command.Parameters.AddWithValue("@author", book.Author);
                    command.Parameters.AddWithValue("@genre", book.Genre);
                    command.Parameters.AddWithValue("@description", book.Description);
                    command.ExecuteNonQuery();
                }
                //Закрытие соединения
                connection.Close();
            }
        }

        /// <summary>
        /// Метод получения списка всех книг в базе данных
        /// </summary>
        /// <returns>Список с экземплярами класса Book, полученными из базы</returns>
        public static List<Book> GetBooks()
        {
            //Создание списка книг который будет возвращать метод
            List<Book> books = new List<Book>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                //Открытие соединения
                connection.Open();

                //Формирование подготовленного запроса
                string request = @"
                SELECT * FROM Books";

                //Выполнение запроса
                using (var command = new SQLiteCommand(request, connection))
                {
                    //Формирование ридера
                    var rdr = command.ExecuteReader();

                    //Чтение данных из базы по запросу
                    while (rdr.Read())
                    {
                        //Создание локального экземпляра книги
                        var book = new Book()
                        {
                            Id = rdr.GetInt64(0),
                            Title = rdr.GetString(1),
                            Author = rdr.GetString(2),
                            Description = rdr.GetString(3),
                            Genre = rdr.GetString(4)
                        };
                        //Добавление книги в список
                        books.Add(book);
                    }
                    //Закрытие ридера
                    rdr.Close();
                }
                //Закрытие соединения
                connection.Close();
            }
            //Возврат списка книг
            return books;
        }

        /// <summary>
        /// Метод получения списка книг по жанру
        /// </summary>
        /// <param name="genre">Жанр книги для поиска</param>
        /// <returns>Список с экземплярами класса Book, полученными из базы, 
        /// жанр которых совпадает с введённым параметром</returns>
        public static List<Book> GetBooksByGenre(string genre)
        {
            List<Book> books = new List<Book>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Формирование строки запроса
                string request = @"
                    SELECT * FROM Books WHERE Genre = @Genre";

                using (var command = new SQLiteCommand(request, connection))
                {
                    // Задаем значение параметра для фильтрации по жанру
                    command.Parameters.AddWithValue("@Genre", genre);

                    // Открываем ридер
                    using (var rdr = command.ExecuteReader())
                    {
                        // Читаем данные из базы
                        while (rdr.Read())
                        {
                            // Создаём локальный экземпляр книги
                            var book = new Book()
                            {
                                Id = rdr.GetInt64(0),
                                Title = rdr.GetString(1),
                                Author = rdr.GetString(2),
                                Description = rdr.GetString(3),
                                Genre = rdr.GetString(4)
                            };
                            //Добавляем книгу в список
                            books.Add(book);
                        }
                        // Закрываем ридер
                        rdr.Close();
                    }
                }
                //Закрываем соединение
                connection.Close();
            }
            //Возвращаем список книг
            return books;
        }

        /// <summary>
        /// Получение книг по ключевому слову
        /// </summary>
        /// <param name="keyword">Ключевое слово для поиска</param>
        /// <returns>Список книг, в названии\имени автора которых содержится ключевое слово</returns>
        public static List<Book> GetBooksByKeyword(string keyword)
        {
            List<Book> books = new List<Book>();

            // Подключение к базе данных
            using (var connection = new SQLiteConnection(_connectionString))
            {
                // Открытие соединения
                connection.Open();

                // Формирование запроса
                string request = @"
                    SELECT * FROM Books WHERE Title LIKE @Keyword OR Author LIKE @Keyword";

                // Выполнение запроса
                using (var command = new SQLiteCommand(request, connection))
                {
                    // Задание параметров для поиска
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    // Чтение данных из базы
                    var rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        // Создание локального экземпляра книги
                        var book = new Book()
                        {
                            Id = rdr.GetInt64(0),
                            Title = rdr.GetString(1),
                            Author = rdr.GetString(2),
                            Description = rdr.GetString(3),
                            Genre = rdr.GetString(4)
                        };
                        // Добавление книги в список
                        books.Add(book);
                    }
                    // Закрытие ридера
                    rdr.Close();
                }
                // Закрытие соединения
                connection.Close();
            }
            // Возврат списка книг
            return books;
        }

        /// <summary>
        /// Метод удаления книги из базы
        /// </summary>
        /// <param name="id">Идентификатор книги</param>
        /// <returns>true - если книга была успешно удалена и 
        /// false - если книги с заданным идентификатором нет в базе</returns>
        public static bool DeleteBook(long id)
        {
            // Подключение к базе данных
            using (var connection = new SQLiteConnection(_connectionString))
            {
                // Открытие соединения
                connection.Open();

                // Формирование запроса
                string request = @"
                    DELETE FROM Books WHERE Id = @Id";

                // Выполнение запроса
                using (var command = new SQLiteCommand(request, connection))
                {
                    // Задание параметра
                    command.Parameters.AddWithValue("@Id", id);
                    // Выполнение запроса
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    // Если книги с заданным идентификатором нет в базе
                    catch
                    {
                        // Закрытие соединения
                        connection.Close();
                        // Возврат результата
                        return false;
                    }
                }
                // Закрытие соединения
                connection.Close();
            }
            // Возврат результата
            return true;
        }

        /// <summary>
        /// Получение книги по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор книги</param>
        /// <returns>Экземпляр класса Book, если удалось её найти,
        /// и null, если книга не была найдена</returns>
        public static Book? GetBookById(int id)
        {
            // Подключение к базе данных
            using (var connection = new SQLiteConnection(_connectionString))
            {
                // Открытие соединения
                connection.Open();

                // Формирование запроса
                string request = @"
                    SELECT * FROM Books 
                    WHERE Id = @id;";

                // Выполнение запроса
                using (var command = new SQLiteCommand(request, connection))
                {
                    // Задание параметра
                    command.Parameters.AddWithValue("@id", id);
                    // Чтение данных из базы
                    var rdr = command.ExecuteReader();
                    if (rdr.Read())
                    {
                        // Создание экземпляра Book
                        var book = new Book
                        {
                            Id = rdr.GetInt64(0),
                            Title = rdr.GetString(1),
                            Author = rdr.GetString(2),
                            Genre = rdr.GetString(3),
                            Description = rdr.GetString(4)
                        };
                        // Закрытие ридера
                        rdr.Close();
                        // Возвращение книги
                        return book;
                    }
                }
                // Закрытие соединения
                connection.Close();
            }
            // Возвращаем null, если книга с таким ID не найдена
            return null; 
        }
    }
}
