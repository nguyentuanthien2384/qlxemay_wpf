using System.Data;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IPurchaseInvoiceService
    {
        DataTable LoadEmployees();
        DataTable LoadSuppliers();
        DataTable LoadProducts();
        DataTable LoadInvoiceNumbers();
        string GetEmployeeName(string employeeId);
        PartyInfo GetSupplierInfo(string supplierId);
        string GetProductName(string productId);
        PurchaseInvoiceOperationResult SaveLine(PurchaseInvoiceRequest request);
        PurchaseInvoiceOperationResult DeleteLine(string invoiceNo, string productId, int quantity);
        void CancelInvoice(string invoiceNo);
        PurchaseInvoiceSnapshot LoadInvoice(string invoiceNo);
    }
}
