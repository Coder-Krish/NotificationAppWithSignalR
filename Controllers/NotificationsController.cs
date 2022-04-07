using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationWithSignalR.DataAccess;
using NotificationWithSignalR.Hubs;
using NotificationWithSignalR.interfaces;
using NotificationWithSignalR.Models;

namespace NotificationWithSignalR.Controllers  
{  
    [Route("api/[controller]")]  
    [ApiController]  
    [EnableCors("CorsPolicy")]                
    public class NotificationsController : ControllerBase  
    {  
        private readonly MyDbContext _context;  
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public NotificationsController(MyDbContext context, IHubContext<BroadcastHub, IHubClient> hubContext, IHubContext<NotificationHub> notificationHub)  
        {  
            _context = context;  
            _hubContext = hubContext;  
            _notificationHub = notificationHub;
            
        }  
  
        // GET: api/Notifications/notificationcount  
        [Route("notificationcount")]  
        [HttpGet]  
        public async Task<ActionResult<NotificationCountResult>> GetNotificationCount()  
        {  
            var count = (from not in _context.Notification  
                         select not).CountAsync();  
            NotificationCountResult result = new NotificationCountResult  
            {  
                Count = await count  
            };
            // await _notificationHub.Clients.All.SendAsync("Notification",result); 
            await SendNotification(result);
            return result;  
        }  
  
        // GET: api/Notifications/notificationresult  
        [Route("notificationresult")]  
        [HttpGet]  
        public async Task<ActionResult<List<NotificationResult>>> GetNotificationMessage()  
        {  
            var results = from message in _context.Notification  
                        orderby message.Id descending  
                        select new NotificationResult  
                        {  
                            EmployeeName = message.EmployeeName,  
                            TranType = message.TranType  
                        };  
            return await results.ToListAsync();  
        }  
  
        // DELETE: api/Notifications/deletenotifications  
        [HttpDelete]  
        [Route("deletenotifications")]  
        public async Task<IActionResult> DeleteNotifications()  
        {  
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Notification");  
            await _context.SaveChangesAsync();

            var count = (from not in _context.Notification  
                         select not).CountAsync();  
            NotificationCountResult result = new NotificationCountResult  
            {  
                Count = await count  
            };
            await SendNotification(result);
            // await _hubContext.Clients.All.BroadcastMessage();  
  
            return NoContent();  
        }

        private async Task SendNotification(NotificationCountResult result)
        {
           await _notificationHub.Clients.All.SendAsync("Notification",result);
        }
    }  
} 