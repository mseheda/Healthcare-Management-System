namespace Healthcare_Hospital_Management_System.Services
{
    public class DrugInventoryService : IDrugInventoryService
    {
        private static Dictionary<string, int> _drugStock = new(StringComparer.OrdinalIgnoreCase);

        private const int LowStockThreshold = 10;

        public event EventHandler<LowStockEventArgs> LowStockDetected;

        public DrugInventoryService()
        {
            _drugStock = new Dictionary<string, int>();
        }

        public async Task<int> GetStockLevelAsync(string drugName, CancellationToken cancellationToken)
        {
            if (_drugStock.TryGetValue(drugName, out var stockLevel))
            {
                return stockLevel;
            }

            return 0;
        }

        public async Task UpdateStockLevelAsync(string drugName, int newStockLevel, CancellationToken cancellationToken)
        {
            _drugStock[drugName] = newStockLevel;

            if (newStockLevel < LowStockThreshold)
            {
                OnLowStockDetected(drugName, newStockLevel);
            }
        }

        protected virtual void OnLowStockDetected(string drugName, int stockLevel)
        {
            LowStockDetected?.Invoke(this, new LowStockEventArgs(drugName, stockLevel));
        }
    }

    public class LowStockEventArgs : EventArgs
    {
        public string DrugName { get; }
        public int StockLevel { get; }

        public LowStockEventArgs(string drugName, int stockLevel)
        {
            DrugName = drugName;
            StockLevel = stockLevel;
        }
    }
}
