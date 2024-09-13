using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace labo.signalr.api.Hubs
{
    public class UselessTasksHub : Hub
    {
        public static int nbUser;

        private readonly ApplicationDbContext _context;
        public UselessTasksHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Connexion()
        {
            nbUser++;
            await Clients.Caller.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
            await Clients.All.SendAsync("UserCount", nbUser);
        }

        [HttpPost]
        public async Task Add(string taskText)
        {
            UselessTask uselessTask = new UselessTask()
            {
                Completed = false,
                Text = taskText
            };
            _context.UselessTasks.Add(uselessTask);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task Complete(int id)
        {
            UselessTask? task = await _context.FindAsync<UselessTask>(id);
            if (task != null)
            {
                task.Completed = true;
                await _context.SaveChangesAsync();
            }
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
        }

        public async Task Déconnexion()
        {
            nbUser--;
            await Clients.All.SendAsync("UserCount", nbUser);
        }
    }
}
