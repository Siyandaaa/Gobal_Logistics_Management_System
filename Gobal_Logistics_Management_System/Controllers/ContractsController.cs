using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.Models.Entities;
using Global_Logistics_Management_System.Models.ViewModels;
using Global_Logistics_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Global_Logistics_Management_System.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;
        private readonly SearchService _searchService;
        private readonly IWebHostEnvironment _env;

        public ContractsController(ApplicationDbContext context, FileService fileService, SearchService searchService, IWebHostEnvironment env)
        {
            _context = context;
            _fileService = fileService;
            _searchService = searchService;
            _env = env;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, ContractStatus? status)
        {
            var contracts = await _searchService.SearchContractsAsync(from, to, status);
            var viewModel = new ContractIndexViewModel
            {
                Contracts = contracts,
                FilterFrom = from,
                FilterTo = to,
                FilterStatus = status
            };
            return View(viewModel);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            if (contract == null) return NotFound();
            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name");
            return View(new ContractCreateViewModel());
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                return View(model);
            }

            string? filePath = null;
            if (model.SignedAgreement != null)
            {
                try
                {
                    filePath = await _fileService.SaveSignedAgreementAsync(model.SignedAgreement);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("SignedAgreement", ex.Message);
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", model.ClientId);
                    return View(model);
                }
            }

            var contract = new Contract
            {
                ClientId = model.ClientId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ServiceLevel = model.ServiceLevel,
                SignedAgreementPath = filePath
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/DownloadAgreement/5
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, contract.SignedAgreementPath.TrimStart('/'));
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", $"Contract_{id}_Agreement.pdf");
        }
    }
}
