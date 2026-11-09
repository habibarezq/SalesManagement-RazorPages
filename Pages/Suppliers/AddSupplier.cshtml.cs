using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Models;

namespace SalesManagement.Pages.Suppliers
{
    public class AddSupplierModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public AddSupplierModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Supplier newSupplier { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Suppliers.Add(newSupplier);
            _context.SaveChanges();

            TempData["Success"] = "Supplier added.";
            return RedirectToPage("Index");   // was "/Suppliers", which isn't a page
        }
    }
}
