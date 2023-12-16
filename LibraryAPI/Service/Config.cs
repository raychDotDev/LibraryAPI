using System;
using System.IO;
using System.Text.Json;

namespace LibraryAPI.Service;

public class Config
{
    // Адрес хоста и порт
    public string HOST { get; private set; }
    public string PORT { get; private set; }

    // Реализация Singleton
    private static Config instance;

    // Конструктор
    private Config()
    {
        Init();
    }

    // Свойство для получения экземпляра
    public static Config Instance
    {
        get
        {
            instance ??= new Config();
            return instance;
        }
    }

    // Инициализация
    private void Init()
    {
        // Загрузка конфигурации из JSON
        string jsonFilePath = "./config.json";
        string jsonString = File.ReadAllText(jsonFilePath);

        // Парсинг JSON
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Десериализация конфига из файла config.json
        try
        {
            var configSection = JsonSerializer.Deserialize<JsonElement>(jsonString, options);

            if (configSection.TryGetProperty("HOST", out JsonElement hostElement))
            {
                HOST = hostElement.GetString();
            }

            if (configSection.TryGetProperty("PORT", out JsonElement portElement))
            {
                PORT = portElement.GetString();
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Error parsing JSON configuration.", ex);
        }
    }
}
