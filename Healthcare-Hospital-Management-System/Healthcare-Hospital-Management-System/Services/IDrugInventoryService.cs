namespace Healthcare_Hospital_Management_System.Services
{
    public interface IDrugInventoryService
    {
        Task<int> GetStockLevelAsync(string drugName, CancellationToken cancellationToken);
        Task UpdateStockLevelAsync(string drugName, int newStockLevel, CancellationToken cancellationToken);
        event EventHandler<LowStockEventArgs> LowStockDetected;
    }

}
