using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace SalesManagement.Pages.Purchases;

public class IndexModel : PageModel
{
    private readonly SalesManagementDbContext _context;   // use your context's real name

    public IndexModel(SalesManagementDbContext context) => _context = context;

    public class InvoiceRow
    {
        public int InvoiceId { get; set; }
        public string SupplierName { get; set; } = "";
        public DateTime Date { get; set; }
        public string Status { get; set; } = "";
        public decimal Total { get; set; }
    }

    public List<InvoiceRow> Invoices { get; set; } = new();

    public void OnGet()
    {
        Invoices = _context.PurchaseInvoices
            .AsNoTracking()
            .OrderByDescending(i => i.Date)
            .Select(i => new InvoiceRow
            {
                InvoiceId = i.InvoiceId,
                SupplierName = i.Supplier.Name,
                Date = i.Date,
                Status = i.PurchaseInvoiceHistories
                    .OrderByDescending(h => h.Date)
                    .ThenByDescending(h => h.HistoryId)
                    .Select(h => h.Status)
                    .FirstOrDefault() ?? "Unknown",
                Total = i.PurchaseInvoiceItems.Sum(s => s.Quantity * s.Price)
            })
            .ToList();
    }
}