using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace   SalesManagement.Pages.Suppliers
{
    public class SuppliersModel : PageModel
    {

    private readonly SalesManagementDbContext _context;
    public SuppliersModel(SalesManagementDbContext context)
        {
            _context=context;
        }

        public List<Supplier> suppliers {get; set;} =new();
        public void OnGet()
        {
            suppliers=_context.Suppliers
                                .ToList();
        }
        public IActionResult OnPostDelete(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null) return NotFound();

            if (_context.PurchaseInvoices.Any(i => i.SupplierId == id))
            {
                TempData["Error"] = "This supplier has invoices and can't be deleted.";
                return RedirectToPage("Index");
            }

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            TempData["Success"] = "Supplier deleted.";
            return RedirectToPage("Index");
        }
    }
}
