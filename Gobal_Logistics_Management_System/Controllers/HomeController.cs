using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Global_Logistics_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalContracts = await _context.Contracts.CountAsync();
            ViewBag.ActiveContracts = await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Active);
            ViewBag.ExpiringSoon = await _context.Contracts.CountAsync(c =>
                c.EndDate >= DateTime.Today && c.EndDate <= DateTime.Today.AddDays(30) && c.Status == ContractStatus.Active);
            ViewBag.TotalRequests = await _context.ServiceRequests.CountAsync();
            ViewBag.PendingRequests = await _context.ServiceRequests.CountAsync(r => r.Status == ServiceRequestStatus.Pending);
            ViewBag.TotalValue = await _context.ServiceRequests.SumAsync(r => r.Cost);

            ViewBag.RecentRequests = await _context.ServiceRequests
                .Include(r => r.Contract).ThenInclude(c => c.Client)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
