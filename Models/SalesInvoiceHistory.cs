using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class SalesInvoiceHistory
{
    public int HistoryId { get; set; }

    public int SalesInvoiceId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual SalesInvoice SalesInvoice { get; set; } = null!;
}
