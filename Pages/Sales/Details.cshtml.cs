using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesManagement.Pages.Sales;


public class AddItemInput
{
    [Required(ErrorMessage = "Choose a product.")]
    public int? ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; } = 1;
}


public class DetailsModel : PageModel
{
    private readonly SalesManagementDbContext _context;

    public DetailsModel(SalesManagementDbContext context) => _context = context;

    public SalesInvoice Invoice { get; set; } = null!;
    public string CurrentStatus { get; set; } = "";
    public decimal Total => Invoice.SalesItems.Sum(i => i.Quantity * i.Price);

    // inside DetailsModel:
    [BindProperty]
    public AddItemInput Input { get; set; } = new();

    public SelectList Products { get; set; } = null!;

    public IActionResult OnGet(int id) => LoadInvoice(id) ? Page() : NotFound();

    public IActionResult OnPostAddItem(int id)
    {
        if (!LoadInvoice(id)) return NotFound();

        if (CurrentStatus != "Pending")
        {
            ModelState.AddModelError("", "Only pending invoices can be changed.");
            return Page();
        }
        if (!ModelState.IsValid) return Page();

        var product = _context.Products.Find(Input.ProductId!.Value);
        if (product == null)
        {
            ModelState.AddModelError("", "Product not found.");
            return Page();
        }

        var line = Invoice.SalesItems.FirstOrDefault(i => i.ProductId == product.ProductId);
        var newQty = (line?.Quantity ?? 0) + Input.Quantity;   // combined quantity

        if (newQty > product.Quantity)
        {
            ModelState.AddModelError("", $"Not enough stock for {product.Name}. Available: {product.Quantity}, requested in total: {newQty}.");
            return Page();
        }

        if (line != null)
            line.Quantity = newQty;                              // merge into existing line
        else
            _context.SalesItems.Add(new SalesItem
            {
                InvoiceId = id,
                ProductId = product.ProductId,
                Quantity = Input.Quantity,
                Price = product.Price                            // price at time of invoice
            });

        _context.SaveChanges();
        return RedirectToPage(new { id });
    }

    public IActionResult OnPostRemoveItem(int id, int productId)
    {
        if (!LoadInvoice(id)) return NotFound();

        if (CurrentStatus == "Pending")
        {
            var line = _context.SalesItems.Find(id, productId);  // composite key: (invoiceId, productId)
            if (line != null)
            {
                _context.SalesItems.Remove(line);
                _context.SaveChanges();
            }
        }
        return RedirectToPage(new { id });
    }

    public IActionResult OnPostPay(int id)
    {
        var invoice = _context.SalesInvoices
                        .Include(i => i.SalesItems).ThenInclude(si => si.Product)
                        .Include(i => i.SalesInvoiceHistories)
                        .AsSplitQuery()
                        .FirstOrDefault(i => i.InvoiceId == id);
        if (invoice == null) return NotFound();
        var status=invoice.SalesInvoiceHistories
            .OrderByDescending(h => h.Date)
            .ThenByDescending(h => h.HistoryId)
            .Select(h => h.Status)
            .FirstOrDefault() ?? "Unknown";
        if (status != "Pending")
            return Fail(id, "Only Pending invoices can be Paid");
        if(!invoice.SalesItems.Any())
            return Fail(id, "Cannot pay an invoice with no items.");

        //Re-check stock for every line before changing anything
        var check_list = invoice.SalesItems.Where(i => i.Product.Quantity < i.Quantity).ToList();
        if (check_list.Any())
        {
            return Fail(id, "Not enough stock for: " +
                      string.Join(", ", check_list.Select(i => i.Product.Name)));

        }
        foreach(var line in invoice.SalesItems)
        {
            line.Product.Quantity -= line.Quantity;
            //update stock history for each item
            _context.StockHistories.Add(new StockHistory
            {
                ProductId = line.ProductId,
                QuantityChange = -line.Quantity,
                Reason = "Sale",
                SalesInvoiceId = invoice.InvoiceId
            });
        }

        invoice.SalesInvoiceHistories.Add(new SalesInvoiceHistory { Status = "Paid" });

        _context.SaveChanges();
        TempData["Success"] = "Invoice paid.";        // add this line
        return RedirectToPage(new { id });
    }

    public IActionResult OnPostCancel(int id)
    {
               var invoice = _context.SalesInvoices
                           .Include(i => i.SalesItems).ThenInclude(si => si.Product)
                        .Include(i => i.SalesInvoiceHistories)
                        .AsSplitQuery()
                        .FirstOrDefault(i => i.InvoiceId == id);
        if (invoice == null) return NotFound();
        var status = invoice.SalesInvoiceHistories
            .OrderByDescending(h => h.Date)
            .ThenByDescending(h => h.HistoryId)
            .Select(h => h.Status)
            .FirstOrDefault() ?? "Unknown";
        if (status != "Pending" && status != "Paid")
        {
            return Fail(id, "Only Pending or Paid invoices can be Cancelled");

        }
        if (status == "Paid")
        {
            //reverse stock dedecution made at pay time
            Console.WriteLine("reversing");
            //load the salesitems collection
            foreach (var line in invoice.SalesItems)
            {
                line.Product.Quantity += line.Quantity;
                _context.StockHistories.Add(new StockHistory
                {
                    ProductId = line.ProductId,
                    QuantityChange = line.Quantity,        // positive
                    Reason = "Sale cancelled",
                    SalesInvoiceId = invoice.InvoiceId
                });
            }
        }
        invoice.SalesInvoiceHistories.Add(new SalesInvoiceHistory { Status = "Cancelled" });

        _context.SaveChanges();
        TempData["Success"] = "Invoice cancelled.";   // add this line
        return RedirectToPage(new { id });
    }
    private IActionResult Fail(int id, string message)
    {
        ModelState.Clear();                              // see note below
        ModelState.AddModelError(string.Empty, message); // empty key = page-level error
        LoadInvoice(id);                                 // reload data so the page can redisplay
        return Page();
    }

    private bool LoadInvoice(int id)
    {
        var invoice = _context.SalesInvoices
            .Include(i => i.Customer)
            .Include(i => i.SalesItems).ThenInclude(si => si.Product)
            .Include(i => i.SalesInvoiceHistories)
            .AsSplitQuery()
            .FirstOrDefault(i => i.InvoiceId == id);

        if (invoice == null) return false;

        Invoice = invoice;
        CurrentStatus = invoice.SalesInvoiceHistories
            .OrderByDescending(h => h.Date)
            .ThenByDescending(h => h.HistoryId)
            .Select(h => h.Status)
            .FirstOrDefault() ?? "Unknown";

        Products = new SelectList(_context.Products.OrderBy(p => p.Name), "ProductId", "Name");
        return true;
    }
}