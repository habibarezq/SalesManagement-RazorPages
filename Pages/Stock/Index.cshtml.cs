using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;
using SalesManagement.Pages; // adjust to your namespace

namespace SalesManagement.Pages.Stock; // adjust

public class IndexModel : PageModel
{
    private readonly SalesManagementDbContext _context;
    public IndexModel(SalesManagementDbContext context) => _context = context;

    [BindProperty(SupportsGet = true)]
    public int? ProductId { get; set; }

    public List<SelectListItem> Products { get; set; } = new();
    public List<MovementRow> Movements { get; set; } = new();

    public class MovementRow //It's a view model, a DTO shaped for the screen, so the query only fetches what the table shows.
    {
        public int HistoryId { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; } = "";
        public int QuantityChange { get; set; }
        public string? Reason { get; set; }
        public int? SalesInvoiceId { get; set; }
        public int? PurchaseInvoiceId { get; set; }
    }

    public async Task OnGetAsync()
    {
        Products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            })
            .ToListAsync();

        var query = _context.StockHistories.AsNoTracking().AsQueryable();

        if (ProductId.HasValue)
            query = query.Where(h => h.ProductId == ProductId.Value);

        Movements = await query
            .OrderByDescending(h => h.Date)
            .ThenByDescending(h => h.StockhistoryId)
            .Select(h => new MovementRow
            {
                HistoryId = h.StockhistoryId,
                Date = h.Date,
                ProductName = h.Product.Name,
                QuantityChange = h.QuantityChange,
                Reason = h.Reason,
                SalesInvoiceId = h.SalesInvoiceId,
                PurchaseInvoiceId = h.PurchaseInvoiceId
            })
            .Take(200)
            .ToListAsync();
    }
}