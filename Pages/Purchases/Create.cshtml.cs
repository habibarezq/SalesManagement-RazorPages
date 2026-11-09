using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace SalesManagement.Pages.Purchases
{
    public class CreateModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public CreateModel(SalesManagementDbContext context) => _context = context;

        [BindProperty]
        [Required(ErrorMessage = "Please choose a supplier.")]
        public int? SupplierId { get; set; }

        public SelectList Suppliers { get; set; } = null!;

        public void OnGet() => LoadSuppliers();

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) { LoadSuppliers(); return Page(); }

            var invoice = new PurchaseInvoice { SupplierId = SupplierId!.Value };
            invoice.PurchaseInvoiceHistories.Add(new PurchaseInvoiceHistory { Status = "Pending" });

            _context.PurchaseInvoices.Add(invoice);
            _context.SaveChanges();   // header and Pending history in one transaction

            TempData["Success"] = "Purchase invoice created.";
            return RedirectToPage("Details", new { id = invoice.InvoiceId });
        }

        private void LoadSuppliers() =>
          Suppliers = new SelectList(_context.Suppliers.OrderBy(s => s.Name), "SupplierId", "Name");
    }
}
