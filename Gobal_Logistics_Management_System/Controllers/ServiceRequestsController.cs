using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.DesignPatterns.Factory;
using Global_Logistics_Management_System.DesignPatterns.Observer;
using Global_Logistics_Management_System.Models.Entities;
using Global_Logistics_Management_System.Models.ViewModels;
using Global_Logistics_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Global_Logistics_Management_System.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ContractValidationService _validation;
        private readonly ICurrencyService _currencyService;
        private readonly ServiceRequestSubject _subject;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ContractValidationService validation,
            ICurrencyService currencyService,
            ServiceRequestSubject subject)
        {
            _context = context;
            _validation = validation;
            _currencyService = currencyService;
            _subject = subject;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index(ServiceRequestStatus? status, int? contractId)
        {
            var query = _context.ServiceRequests
                .Include(r => r.Contract)
                    .ThenInclude(c => c.Client)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            if (contractId.HasValue)
                query = query.Where(r => r.ContractId == contractId.Value);

            var requests = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            ViewBag.StatusFilter = status;
            ViewBag.ContractIdFilter = contractId;
            ViewBag.StatusList = new SelectList(Enum.GetValues<ServiceRequestStatus>());

            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var request = await _context.ServiceRequests
                .Include(r => r.Contract)
                    .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);

            if (request == null) return NotFound();

            return View(request);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var request = await _context.ServiceRequests
                .Include(r => r.Contract)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null) return NotFound();

            ViewBag.StatusList = new SelectList(Enum.GetValues<ServiceRequestStatus>(), request.Status);
            return View(request);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Cost,CostUSD,Status")] ServiceRequest request)
        {
            if (id != request.ServiceRequestId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = new SelectList(Enum.GetValues<ServiceRequestStatus>(), request.Status);
                return View(request);
            }

            try
            {
                var existing = await _context.ServiceRequests.FindAsync(id);
                if (existing == null) return NotFound();

                // Only allow updating specific fields
                existing.Description = request.Description;
                existing.Cost = request.Cost;
                existing.CostUSD = request.CostUSD;
                existing.Status = request.Status;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                // Notify observers of status change
                await _subject.NotifyAsync(existing);

                TempData["Success"] = "Service request updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRequestExists(request.ServiceRequestId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Details), new { id = request.ServiceRequestId });
        }

        // GET: ServiceRequests/Create?contractId=5
        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);

            if (contract == null) return NotFound();

            if (!_validation.CanCreateServiceRequest(contract))
            {
                TempData["Error"] = "Cannot create request: contract is expired or on hold.";
                return RedirectToAction("Details", "Contracts", new { id = contractId });
            }

            var model = new ServiceRequestCreateViewModel
            {
                ContractId = contractId,
                ContractNumber = $"CT-{contractId:D4}",
                ClientName = contract.Client?.Name
            };
            return View(model);
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
        {
            var contract = await _context.Contracts.FindAsync(model.ContractId);
            if (contract == null) return NotFound();

            if (!_validation.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError("", "Contract is not active.");
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);

            decimal finalCost = 0;
            if (model.CostUSD.HasValue)
            {
                var rate = await _currencyService.GetUsdToZarRateAsync();
                finalCost = _currencyService.ConvertUsdToZar(model.CostUSD.Value, rate);
            }

            var baseRequest = ServiceRequestFactory.CreateRequest(
                model.RequestType, model.ContractId, model.Description, model.CostUSD);

            var serviceRequest = new ServiceRequest
            {
                ContractId = model.ContractId,
                Description = baseRequest.Description,
                Cost = finalCost,
                CostUSD = baseRequest.CostUSD,
                Status = ServiceRequestStatus.Pending
            };

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            await _subject.NotifyAsync(serviceRequest);

            TempData["Success"] = "Service request created successfully.";
            return RedirectToAction("Details", "Contracts", new { id = model.ContractId });
        }

        // AJAX: GET ServiceRequests/CalculateZar?usdAmount=100
        [HttpGet]
        public async Task<IActionResult> CalculateZar(decimal usdAmount)
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            var zar = _currencyService.ConvertUsdToZar(usdAmount, rate);
            return Json(new { rate, zar });
        }

        private bool ServiceRequestExists(int id) => _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
    }
}
