using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Warehouse_Management_System
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }

        public class ElectronicItem : IInventoryItem
        {
            public int Id { get; }
            public string Name { get; }
            public int Quantity { get; set; }
            public string Brand { get; }
            public int WarrantyMonths { get; }

            public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
            {
                Id = id;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Quantity = quantity < 0 ? 0 : quantity;
                Brand = brand ?? throw new ArgumentNullException(nameof(brand));
                WarrantyMonths = warrantyMonths;
            }

            public override string ToString()
            {
                return $"Electric [Id: {Id}, Name: {Name}, Quantity: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths} months]";
            }
        }

        public class GroceryItem : IInventoryItem
        {
            public int Id { get; }
            public string Name { get; }
            public int Quantity { get; set; }
            public DateTime ExpiryDate { get; }

            public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
            {
                Id = id;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Quantity = quantity < 0 ? 0 : quantity;
                ExpiryDate = expiryDate;
            }

            public override string ToString()
            {
                return $"Grocery [Id: {Id}, Name: {Name}, Quantity: {Quantity}, Expiry: {ExpiryDate}]";
            }
        }

        public class DuplicateItemException : Exception
        {
            public DuplicateItemException(string message) : base(message) { }
        }

        public class ItemNotFoundException : Exception
        {
            public  ItemNotFoundException(string message) : base(message) { }
        }

        public class InvalidQuantityException : Exception
        {
            public InvalidQuantityException(string message) : base(message) { }
        }

        public class InventoryRepository<T> where T : IInventoryItem
        {
            private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

            public void AddItem(T item)
            {
                if(_items.ContainsKey(item.Id))
                {
                    throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
                }
                _items[item.Id] = item;
            }

            public T GetItemById(int id)
            {
                if (!_items.ContainsKey(id))
                {
                    throw new ItemNotFoundException($"Item with ID {id} not found.");
                }
                return _items[id];
            }

            public void RemoveItem(int id)
            {
                if(!_items.Remove(id))
                {
                    throw new ItemNotFoundException($"Item with ID {id} not found.");
                }
            }

            public List<T> GetAllItems()
            {
                return new List<T>(_items.Values);
            } 

            public void UpdateQuantity(int id, int newQuantity)
            {
                if(newQuantity < 0)
                {
                    throw new InvalidQuantityException("Quantity cannot be negative.");
                }
                if (_items.ContainsKey(id))
                {
                    _items[id].Quantity = newQuantity;
                }
                else
                {
                    throw new ItemNotFoundException($"Item with ID {id} not found.");
                }
            }
        }
        public class WareHouseManager
        {
            private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
            private readonly InventoryRepository<GroceryItem>  _groceries = new InventoryRepository<GroceryItem>();
            
            public void SeedData()
            {
                try
                {
                    _electronics.AddItem(new ElectronicItem(1, "iPad", 10, "Apple", 35));
                    _electronics.AddItem(new ElectronicItem(2, "Microwave", 15, "Akai", 12));
                    _electronics.AddItem(new ElectronicItem(3, "Headphones", 20, "Oraimo", 6));

                    _groceries.AddItem(new GroceryItem(1, "Magerine", 50, DateTime.Now.AddMonths(4)));
                    _groceries.AddItem(new GroceryItem(2, "Bread Spread", 30, DateTime.Now.AddDays(7)));
                    _groceries.AddItem(new GroceryItem(3, "Tomatoes", 100, DateTime.Now.AddDays(14)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding data: {ex.Message}");
                }
            }

            public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
            {
                var items = repo.GetAllItems();
                Console.WriteLine($"\nAll {typeof(T).Name} Items:");
                foreach (var item in items)
                {
                    Console.WriteLine(item);
                }
            }

            public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
            {
                try
                {
                    var item = repo.GetItemById(id);
                    int newQuantity = item.Quantity + quantity;
                    repo.UpdateQuantity(id, newQuantity);
                    Console.WriteLine($"Stock increased for {item.Name}. New quantity: {newQuantity}");
                }
                catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message};");
                } catch (InvalidQuantityException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                } catch(Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }

            public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
            {
                try
                {
                    repo.RemoveItem(id);
                    Console.WriteLine($"Item with ID {id} removed successfully.");
                } catch(ItemNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }

            public static void Main()
            {
                var manager = new WareHouseManager();
                manager.SeedData();

                manager.PrintAllItems(manager._groceries);
                manager.PrintAllItems(manager._electronics);

                try
                {
                    manager._electronics.AddItem(new ElectronicItem(1, "iPad", 10, "Apple", 35));

                } catch(DuplicateItemException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");

                }

                try
                {
                    manager._electronics.RemoveItem(99);
                } catch (ItemNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                try
                {
                    manager._electronics.UpdateQuantity(1, -5);
                } catch (InvalidQuantityException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        
    }
}