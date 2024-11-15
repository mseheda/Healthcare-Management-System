namespace Healthcare_Hospital_Management_System.Services
{
    public class StaffNotificationService
    {
        public StaffNotificationService(IDrugInventoryService drugInventoryService)
        {
            drugInventoryService.LowStockDetected += OnLowStockDetected;
        }

        private void OnLowStockDetected(object sender, LowStockEventArgs e)
        {
            Console.WriteLine($"Low stock alert: {e.DrugName} has only {e.StockLevel} units left.");
        }
    }

}
