using System;
using System.Data;

namespace QLXeMay.Models
{
    internal sealed class PartyInfo
    {
        public PartyInfo(string name, string address = "", string phone = "")
        {
            Name = name ?? string.Empty;
            Address = address ?? string.Empty;
            Phone = phone ?? string.Empty;
        }

        public string Name { get; }
        public string Address { get; }
        public string Phone { get; }
    }

    internal sealed class SalesInvoiceRequest
    {
        public string InvoiceNo { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
    }

    internal sealed class PurchaseInvoiceRequest
    {
        public string InvoiceNo { get; set; }
        public string EmployeeId { get; set; }
        public string SupplierId { get; set; }
        public string ProductId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Discount { get; set; }
    }

    internal sealed class SalesInvoiceSnapshot
    {
        public string InvoiceNo { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
        public double Deposit { get; set; }
        public DataTable Details { get; set; }
    }

    internal sealed class PurchaseInvoiceSnapshot
    {
        public string InvoiceNo { get; set; }
        public string EmployeeId { get; set; }
        public string SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double Total { get; set; }
        public DataTable Details { get; set; }
    }

    internal sealed class SalesInvoiceOperationResult
    {
        public double LineTotal { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
        public double Deposit { get; set; }
        public DataTable Details { get; set; }
    }

    internal sealed class PurchaseInvoiceOperationResult
    {
        public double LineTotal { get; set; }
        public double Total { get; set; }
        public DataTable Details { get; set; }
    }
}
