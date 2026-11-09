using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Models;

namespace SalesManagement.Pages.Customers
{
    public class EditCustomerModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public EditCustomerModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
                return NotFound();

            Customer = customer;
            return Page();
        }

        public IActionResult OnPost()
        {
            // Remove any navigation properties that aren't posted, e.g.:
            ModelState.Remove("Customer.SalesInvoices");

            if (!ModelState.IsValid)
                return Page();

            _context.Customers.Update(Customer);
            _context.SaveChanges();

            TempData["Success"] = "Customer saved.";
            return RedirectToPage("Index");
        }
    }
}