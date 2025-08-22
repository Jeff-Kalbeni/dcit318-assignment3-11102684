using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryManagement
{
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new List<T>();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void Add(T item)
        {
            _log.Add(item ?? throw new ArgumentNullException(nameof(item)));
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                using (var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream))
                {
                    var jsonString = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(jsonString);
                }
                Console.WriteLine($"Data saved to {_filePath}");
            } catch (IOException ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            } catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"File {_filePath} not found, starting with empty log.");
                    return;
                }

                using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream))
                {
                    var jsonString = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var items = JsonSerializer.Deserialize<List<T>>(jsonString);
                        if (items != null) _log.Clear();
                        if (items != null) _log.AddRange(items);
                    }
                }
                Console.WriteLine($"Data loaded from {_filePath}");
            } catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing file: {ex.Message}");
            } catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            } catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error loading from file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;
        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop stand", 10, DateTime.Now.AddDays(-10)));
            _logger.Add(new InventoryItem(2, "RAM Chips", 25, DateTime.Now.AddDays(-5)));
            _logger.Add(new InventoryItem(3, "Monitor", 10, DateTime.Now.AddDays(-2)));
            _logger.Add(new InventoryItem(4, "Laptop cover", 8, DateTime.Now.AddDays(-1)));
            _logger.Add(new InventoryItem(5, "Keyboard", 12, DateTime.Now));
            Console.WriteLine("Sample data seeded.");
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            Console.WriteLine("\nInventory Items:");
            foreach (var item in items)
            {
                Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            string filePath = "C:\\Dkp Files\\Lecture Slides\\DCIT 318\\dcit318-assignment3-11102684\\InventoryManagementSystem\\InventoryManagementSystem\\inventory.json";
            var app = new InventoryApp(filePath);

            app.SeedSampleData();
            app.SaveData();
            app = new InventoryApp(filePath);
            app.LoadData();
            app.PrintAllItems();
        }
    }
}