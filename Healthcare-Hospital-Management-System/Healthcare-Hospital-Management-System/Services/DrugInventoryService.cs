namespace Healthcare_Hospital_Management_System.Services
{
    public class DrugInventoryService : IDrugInventoryService
    {
        private static Dictionary<string, int> _drugStock = new(StringComparer.OrdinalIgnoreCase);

        private const int LowStockThreshold = 10;

        public event EventHandler<LowStockEventArgs>? LowStockDetected;

        public DrugInventoryService()
        {
            _drugStock = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<int> GetStockLevelAsync(string drugName, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                if (_drugStock.TryGetValue(drugName, out var stockLevel))
                {
                    return stockLevel;
                }

                return 0;
            }, cancellationToken);
        }

        public async Task UpdateStockLevelAsync(string drugName, int newStockLevel, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _drugStock[drugName] = newStockLevel;

                if (newStockLevel < LowStockThreshold)
                {
                    OnLowStockDetected(drugName, newStockLevel);
                }
            }, cancellationToken);
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
