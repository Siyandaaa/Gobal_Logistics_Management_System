using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Global_Logistics_Management_System.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string? searchString, string? region)
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString) ||
                                        c.ContactDetails.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(region))
            {
                query = query.Where(c => c.Region == region);
            }

            var clients = await query.OrderBy(c => c.Name).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.RegionFilter = region;
            ViewBag.Regions = await _context.Clients.Select(c => c.Region).Distinct().ToListAsync();
            ViewBag.ActiveContracts = await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Active);

            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ContactDetails,Region")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Client '{client.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Client '{client.Name}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5 (optional)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                // Check if client has contracts (prevent deletion if any)
                var hasContracts = await _context.Contracts.AnyAsync(c => c.ClientId == id);
                if (hasContracts)
                {
                    TempData["Error"] = "Cannot delete client with existing contracts.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Client deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id) => _context.Clients.Any(e => e.ClientId == id);
    }
}

