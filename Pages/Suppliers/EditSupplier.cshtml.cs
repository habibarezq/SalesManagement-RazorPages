using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Models;

namespace SalesManagement.Pages.Suppliers
{
    public class EditSupplierModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public EditSupplierModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        public Supplier Supplier { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            Supplier = supplier;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            Supplier = supplier;

            // Only these four fields can be changed by the form (prevents over-posting).
            var updated = await TryUpdateModelAsync(supplier, "Supplier",
                s => s.Name, s => s.Address, s => s.Phone, s => s.Email);

            if (!updated) return Page();

            await _context.SaveChangesAsync();
            TempData["Success"] = "Supplier saved.";
            return RedirectToPage("Index");
        }
    }
}
