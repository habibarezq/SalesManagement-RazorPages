using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class PurchaseInvoiceHistory
{
    public int HistoryId { get; set; }

    public int PurchaseInvoiceId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual PurchaseInvoice PurchaseInvoice { get; set; } = null!;
}
