using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace SalesManagement.Pages
{
    public class IndexModel : PageModel
    {
        public const int LowStockLimit = 5;

        private readonly SalesManagementDbContext _context;
        public IndexModel(SalesManagementDbContext context) => _context = context;

        public decimal PaidRevenue { get; set; }
        public int PendingSales { get; set; }
        public int PendingPurchases { get; set; }
        public int LowStockCount { get; set; }
        public List<RecentSale> RecentSales { get; set; } = new();
        public List<LowStockRow> LowStock { get; set; } = new();

        public class RecentSale
        {
            public int InvoiceId { get; set; }
            public string CustomerName { get; set; } = "";
            public DateTime Date { get; set; }
            public string Status { get; set; } = "";
            public decimal Total { get; set; }
        }

        public class LowStockRow
        {
            public string Name { get; set; } = "";
            public int Quantity { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Same "latest history row = current status" rule used on the list pages.
            PaidRevenue = await _context.SalesItems
                .Where(s => s.Invoice.SalesInvoiceHistories
                    .OrderByDescending(h => h.Date).ThenByDescending(h => h.HistoryId)
                    .Select(h => h.Status).FirstOrDefault() == "Paid")
                .SumAsync(s => (decimal?)(s.Quantity * s.Price)) ?? 0m;

            PendingSales = await _context.SalesInvoices
                .CountAsync(i => i.SalesInvoiceHistories
                    .OrderByDescending(h => h.Date).ThenByDescending(h => h.HistoryId)
                    .Select(h => h.Status).FirstOrDefault() == "Pending");

            PendingPurchases = await _context.PurchaseInvoices
                .CountAsync(i => i.PurchaseInvoiceHistories
                    .OrderByDescending(h => h.Date).ThenByDescending(h => h.HistoryId)
                    .Select(h => h.Status).FirstOrDefault() == "Pending");

            LowStockCount = await _context.Products.CountAsync(p => p.Quantity <= LowStockLimit);

            LowStock = await _context.Products
                .AsNoTracking()
                .Where(p => p.Quantity <= LowStockLimit)
                .OrderBy(p => p.Quantity).ThenBy(p => p.Name)
                .Take(6)
                .Select(p => new LowStockRow { Name = p.Name, Quantity = p.Quantity })
                .ToListAsync();

            RecentSales = await _context.SalesInvoices
                .AsNoTracking()
                .OrderByDescending(i => i.Date)
                .Take(5)
                .Select(i => new RecentSale
                {
                    InvoiceId = i.InvoiceId,
                    CustomerName = i.Customer.Name,
                    Date = i.Date,
                    Status = i.SalesInvoiceHistories
                        .OrderByDescending(h => h.Date)
                        .ThenByDescending(h => h.HistoryId)
                        .Select(h => h.Status)
                        .FirstOrDefault() ?? "Unknown",
                    Total = i.SalesItems.Sum(s => s.Quantity * s.Price)
                })
                .ToListAsync();
        }
    }
}
