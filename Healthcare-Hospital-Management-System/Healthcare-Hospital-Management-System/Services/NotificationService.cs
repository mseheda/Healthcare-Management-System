namespace HealthcareHospitalManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            string filePath = "notifications.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"Notification: {message}");
            }
        }
    }

}
