namespace WarehouseInventory
{
    // Marker Interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // ElectronicItem
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
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }

    //GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"[Grocery] ID: {Id}, Name: {Name}, Qty: {Quantity}, Expires: {ExpiryDate:yyyy-MM-dd}";
    }

    // Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // WarehouseManager
    public class WarehouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new();
        private InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            // Electronics
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 50, "Sony", 6));

            // Groceries
            _groceries.AddItem(new GroceryItem(1, "Milk", 20, DateTime.Now.AddDays(10)));
            _groceries.AddItem(new GroceryItem(2, "Bread", 15, DateTime.Now.AddDays(3)));
            _groceries.AddItem(new GroceryItem(3, "Apples", 30, DateTime.Now.AddDays(7)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock updated for item {item.Name}. New quantity: {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }


    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WarehouseManager();

            manager.SeedData();

            Console.WriteLine("\n=== Grocery Items ===");
            manager.PrintAllItems(manager.GroceriesRepo);

            Console.WriteLine("\n=== Electronic Items ===");
            manager.PrintAllItems(manager.ElectronicsRepo);

            Console.WriteLine("\n=== Testing Exceptions ===");

            try
            {
                manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }

            manager.RemoveItemById(manager.GroceriesRepo, 99);

            try
            {
                manager.ElectronicsRepo.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }

            Console.WriteLine("\n=== End of Program ===");
        }
    }
}
