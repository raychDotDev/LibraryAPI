using Nancy;
using Nancy.ModelBinding;
using System.Linq;
using LibraryAPI.Models;
using LibraryAPI.Database;

namespace LibraryAPI.Modules;

public class BookModule : NancyModule
{
    /// <summary>
    /// Модуль для работы с входящими запросами
    /// </summary>
    public BookModule() : base("/books")
    {
        #region GET
        // Получение списка всех книг
        Get("/", _ =>
        {
            // Получение списка всех книг из базы данных
            List<Book> books = LibraryDatabase.GetBooks();
            // Возврат списка книг в формате JSON
            return Response.AsJson(books);
        });

        // Получение книг по ключевому слову
        Get("/search/{keyword}", parameters =>
        {
            string keyword = parameters.keyword;
            List<Book> books = LibraryDatabase.GetBooksByKeyword(keyword);
            return Response.AsJson(books);
        });

        // Получение книг по жанру
        Get("/genre/{genre}", parameters =>
        {
            string genre = parameters.genre;
            // Получение списка книг по жанру из базы данных
            List<Book> books = LibraryDatabase.GetBooksByGenre(genre);
            // Возврат списка книг в формате JSON
            return Response.AsJson(books);
        });

        // Получение книги по идентификатору
        Get("/{id:int}", parameters =>
        {
            int bookId = parameters.id;
            //Поиск книги по идентификатору
            Book? book = LibraryDatabase.GetBookById(bookId);
            //Возврат книги в формате JSON или сообщения об ошибке
            return book != null ? Response.AsJson(book) : HttpStatusCode.NotFound;
        });
        #endregion

        #region PUT
        // Добавление новой книги
        Put("/", _ =>
        {
            var book = this.Bind<Book>();
            LibraryDatabase.AddBook(book);
            return HttpStatusCode.Created;
        });
        #endregion

        #region DELETE
        // Удаление книги
        Delete("/{id:int}", parameters =>
        {
            int bookId = parameters.id;
            return LibraryDatabase.DeleteBook(bookId) ? HttpStatusCode.OK : HttpStatusCode.NotFound;
        });
        #endregion

    }
}
