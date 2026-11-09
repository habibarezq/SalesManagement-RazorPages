using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace SalesManagement.Pages.Purchases
{
    public class DetailsModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public DetailsModel(SalesManagementDbContext context) => _context = context;

        public PurchaseInvoice Invoice { get; set; } = null!;
        public string CurrentStatus { get; set; } = "";
        public decimal Total { get; set; }
        public List<SelectListItem> Products { get; set; } = new();

        public class AddItemInput
        {
            [Required(ErrorMessage = "Please choose a product.")]
            public int? ProductId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
            public int Quantity { get; set; } = 1;

            [Range(0.01, 1000000, ErrorMessage = "Unit cost must be greater than 0.")]
            public decimal UnitPrice { get; set; }
        }

        [BindProperty] public AddItemInput Input { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            return LoadInvoice(id) ? Page() : NotFound();
        }

        private bool LoadInvoice(int id)
        {
            var invoice = _context.PurchaseInvoices.Include(i => i.Supplier)
                .Include(i => i.PurchaseInvoiceItems).ThenInclude(x => x.Product)
                .Include(i => i.PurchaseInvoiceHistories)
                .AsSplitQuery()
                .FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null) return false;

            Invoice = invoice;
            CurrentStatus = StatusOf(invoice);
            Total = invoice.PurchaseInvoiceItems.Sum(x => x.Quantity * x.Price);
            Products = _context.Products.OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.Name })
                .ToList();
            return true;
        }

        private static string StatusOf(PurchaseInvoice i) =>
            i.PurchaseInvoiceHistories
                .OrderByDescending(h => h.Date).ThenByDescending(h => h.HistoryId)
                .Select(h => h.Status).FirstOrDefault() ?? "Unknown";

        private IActionResult Fail(int id, string message)
        {
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, message);
            LoadInvoice(id);
            return Page();
        }

        public IActionResult OnPostAddItem(int id)
        {
            var invoice = _context.PurchaseInvoices
                .Include(i => i.PurchaseInvoiceItems)
                .Include(i => i.PurchaseInvoiceHistories)
                .AsSplitQuery()
                .FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null) return NotFound();

            if (StatusOf(invoice) != "Pending")
                return Fail(id, "Items can only be changed while the invoice is Pending.");

            if (!ModelState.IsValid) { LoadInvoice(id); return Page(); }

            var line = invoice.PurchaseInvoiceItems.FirstOrDefault(x => x.ProductId == Input.ProductId);
            if (line != null)
            {
                // Same product again: merge, using a weighted-average unit cost
                var newQty = line.Quantity + Input.Quantity;
                line.Price = Math.Round(
                    (line.Quantity * line.Price + Input.Quantity * Input.UnitPrice) / newQty, 2);
                line.Quantity = newQty;
            }
            else
            {
                invoice.PurchaseInvoiceItems.Add(new PurchaseInvoiceItem
                {
                    ProductId = Input.ProductId!.Value,
                    Quantity = Input.Quantity,
                    Price = Input.UnitPrice
                });
            }
            _context.SaveChanges();
            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRemoveItem(int id, int productId)
        {
            var invoice = _context.PurchaseInvoices
                .Include(i => i.PurchaseInvoiceHistories)
                .FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null) return NotFound();
            if (StatusOf(invoice) != "Pending")
                return Fail(id, "Items can only be changed while the invoice is Pending.");

            var line = _context.PurchaseInvoiceItems.Find(id, productId);
            if (line != null) { _context.PurchaseInvoiceItems.Remove(line); _context.SaveChanges(); }
            return RedirectToPage(new { id });
        }

        public IActionResult OnPostPay(int id)
        {
            var invoice = _context.PurchaseInvoices
                .Include(i => i.PurchaseInvoiceItems).ThenInclude(x => x.Product)
                .Include(i => i.PurchaseInvoiceHistories)
                .AsSplitQuery()
                .FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null) return NotFound();

            if (StatusOf(invoice) != "Pending")
                return Fail(id, "Only pending invoices can be paid.");
            if (!invoice.PurchaseInvoiceItems.Any())
                return Fail(id, "Cannot pay an invoice with no items.");

            foreach (var line in invoice.PurchaseInvoiceItems)
            {
                line.Product.Quantity += line.Quantity;
                _context.StockHistories.Add(new StockHistory
                {
                    ProductId = line.ProductId,
                    QuantityChange = line.Quantity,
                    Reason = "Purchase",
                    PurchaseInvoiceId = invoice.InvoiceId
                });
            }
            invoice.PurchaseInvoiceHistories.Add(new PurchaseInvoiceHistory { Status = "Paid" });

            _context.SaveChanges();
            TempData["Success"] = "Purchase invoice paid. Stock updated.";
            return RedirectToPage(new { id });
        }

        public IActionResult OnPostCancel(int id)
        {
            var invoice = _context.PurchaseInvoices
                .Include(i => i.PurchaseInvoiceItems).ThenInclude(x => x.Product)
                .Include(i => i.PurchaseInvoiceHistories)
                .AsSplitQuery()
                .FirstOrDefault(i => i.InvoiceId == id);
            if (invoice == null) return NotFound();

            var status = StatusOf(invoice);
            if (status != "Pending" && status != "Paid")
                return Fail(id, "Only Pending or Paid invoices can be cancelled.");

            if (status == "Paid")
            {
                // Can't remove stock that has already been sold
                var check_list = invoice.PurchaseInvoiceItems
                    .Where(x => x.Product.Quantity < x.Quantity).ToList();
                if (check_list.Any())
                    return Fail(id, "Cannot cancel: stock already used for " +
                        string.Join(", ", check_list.Select(x => x.Product.Name)));

                foreach (var line in invoice.PurchaseInvoiceItems)
                {
                    line.Product.Quantity -= line.Quantity;
                    _context.StockHistories.Add(new StockHistory
                    {
                        ProductId = line.ProductId,
                        QuantityChange = -line.Quantity,
                        Reason = "Purchase cancelled",
                        PurchaseInvoiceId = invoice.InvoiceId
                    });
                }
            }
            invoice.PurchaseInvoiceHistories.Add(new PurchaseInvoiceHistory { Status = "Cancelled" });

            _context.SaveChanges();
            TempData["Success"] = "Purchase invoice cancelled.";
            return RedirectToPage(new { id });
        }
    }
}
