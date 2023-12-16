namespace LibraryAPI.Models
{
    public class Book
    {
        /// <summary>
        /// Идентификатор книги, получаемый при запросе к датабазе
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название книги
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Автор книги
        /// </summary>
        public required string Author { get; set; }

        /// <summary>
        /// Описание книги
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Жанр книги
        /// </summary>
        public required string Genre { get; set; }
    }

}
