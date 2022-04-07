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
    public class EmployeesController : ControllerBase  
    {  
        private readonly MyDbContext _context;  
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IHubContext<EmployeeHub> _emmployeeHub;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public EmployeesController(MyDbContext context, IHubContext<BroadcastHub, IHubClient> hubContext, IHubContext<EmployeeHub> empployeeHub, IHubContext<NotificationHub> notificationHub)  
        {  
            _context = context;  
            _hubContext = hubContext;
            _emmployeeHub = empployeeHub;
            _notificationHub = notificationHub;
        }  
  
        // GET: api/Employees  
        [HttpGet]  
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()  
        {  
            return await _context.Employee.ToListAsync();  
        }  
  
        // GET: api/Employees/5  
        [HttpGet("{id}")]  
        public async Task<ActionResult<Employee>> GetEmployee(string id)  
        {  
            var employee = await _context.Employee.FindAsync(id);  
  
            if (employee == null)  
            {  
                return NotFound();  
            }  
  
            return employee;  
        }  
  
        // PUT: api/Employees/5  
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754  
        [HttpPut("{id}")]  
        public async Task<IActionResult> PutEmployee(string id, Employee employee)  
        {  
            if (id != employee.Id)  
            {  
                return BadRequest();  
            }  
  
            _context.Entry(employee).State = EntityState.Modified;  
  
            Notification notification = new Notification()  
            {  
                EmployeeName = employee.Name,  
                TranType = "Edit"  
            };  
            _context.Notification.Add(notification);  
  
            try  
            {  
                await _context.SaveChangesAsync();
                await GetEmployeeListAndSendToClient();
                await GetNotificationCount();
                // await _hubContext.Clients.All.BroadcastMessage();  
            }  
            catch (DbUpdateConcurrencyException)  
            {  
                if (!EmployeeExists(id))  
                {  
                    return NotFound();  
                }  
                else  
                {  
                    throw;  
                }  
            }  
  
            return NoContent();  
        }

        private async Task GetEmployeeListAndSendToClient()
        {
            var employeeList = await _context.Employee.ToListAsync(); 
            await _emmployeeHub.Clients.All.SendAsync("EmployeeList", employeeList);
        }

        private async Task<object> GetNotificationCount()
        {
            var count = (from not in _context.Notification
                         select not).Count();
            NotificationCountResult result = new NotificationCountResult
            {
                Count = count
            };
            await _notificationHub.Clients.All.SendAsync("Notification",result);
            return count;
        }

        // POST: api/Employees  
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754  
        [HttpPost]  
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)  
        {  
            employee.Id = Guid.NewGuid().ToString();  
            _context.Employee.Add(employee);  
  
            Notification notification = new Notification()  
            {  
                EmployeeName = employee.Name,  
                TranType = "Add"  
            };  
            _context.Notification.Add(notification);  
  
            try  
            {  
                await _context.SaveChangesAsync();
                await GetEmployeeListAndSendToClient();

                // await _employeeHub.Clients.All.SendAsync("sendEmployeeList",employeeList);
                await GetNotificationCount();
                // await _hubContext.Clients.All.BroadcastMessage();  
            }  
            catch (DbUpdateException)  
            {  
                if (EmployeeExists(employee.Id))  
                {  
                    return Conflict();  
                }  
                else  
                {  
                    throw;  
                }  
            }  
  
            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);  
        }  
  
        // DELETE: api/Employees/5  
        [HttpDelete("{id}")]  
        public async Task<IActionResult> DeleteEmployee(string id)  
        {  
            var employee = await _context.Employee.FindAsync(id);  
            if (employee == null)  
            {  
                return NotFound();  
            }  
  
            Notification notification = new Notification()  
            {  
                EmployeeName = employee.Name,  
                TranType = "Delete"  
            };  
  
            _context.Employee.Remove(employee);  
            _context.Notification.Add(notification);  
  
            await _context.SaveChangesAsync();  
            await GetEmployeeListAndSendToClient();
  
            // await _employeeHub.Clients.All.SendAsync("sendEmployeeList",employeeList);
            await GetNotificationCount();
             //await _hubContext.Clients.All.BroadcastMessage();  
  
            return NoContent();  
        }  
  
        private bool EmployeeExists(string id)  
        {  
            return _context.Employee.Any(e => e.Id == id);  
        }  
    }  
} 