using Microsoft.AspNetCore.SignalR;
using NotificationWithSignalR.interfaces;
using NotificationWithSignalR.Models;

namespace NotificationWithSignalR.Hubs  
{  
    public class BroadcastHub : Hub<IHubClient>  
    { 
    }  
} 