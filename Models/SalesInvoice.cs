using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class SalesInvoice
{
    public int InvoiceId { get; set; }

    public int CustomerId { get; set; }

    public DateTime Date { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<SalesInvoiceHistory> SalesInvoiceHistories { get; set; } = new List<SalesInvoiceHistory>();

    public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

    public virtual ICollection<StockHistory> StockHistories { get; set; } = new List<StockHistory>();
}
