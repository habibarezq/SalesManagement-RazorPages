using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class PurchaseInvoiceItem
{
    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual PurchaseInvoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
