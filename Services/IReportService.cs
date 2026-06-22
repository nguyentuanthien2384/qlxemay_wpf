using System;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IReportService
    {
        ReportResult BuildReport(ReportMode mode, DateTime fromDate, DateTime toDate, string groupKey);
    }
}
