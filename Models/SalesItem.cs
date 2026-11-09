using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class SalesItem
{
    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual SalesInvoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
