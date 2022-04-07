using NotificationWithSignalR.Models;

namespace NotificationWithSignalR.interfaces  
{  
    public interface IHubClient  
    {  
        Task BroadcastMessage();
        // Task SendEmployeeList(List<Employee> employeeList);

        // Task SendNotification(int notificationCount);
    }  
}  