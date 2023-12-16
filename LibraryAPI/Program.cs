using Nancy.Hosting.Self;
using LibraryAPI.Service;

class Program
{
    // Настройки хоста
    /// <summary>
    /// Адрес хоста
    /// </summary>
    public static readonly string HOST = Config.Instance.HOST;
    /// <summary>
    /// Порт хоста
    /// </summary>
    public static readonly string PORT = Config.Instance.PORT;

    static void Main(string[] args)
    {
        // Создание хоста
        Console.WriteLine("Открываем хост...");
        var nancyHost = new NancyHost(new Uri($"http://{HOST}:{PORT}"));
        // Запуск хоста
        nancyHost.Start();
        Console.WriteLine($"Хост открыт на http://{HOST}:{PORT}");
        Console.WriteLine("Для остановки хоста нажмите Enter");
        Console.ReadLine();
        // Остановка хоста
        Console.WriteLine("Останавливаем хост...");
        nancyHost.Stop();
        Console.WriteLine("Хост остановлен");
    }
}