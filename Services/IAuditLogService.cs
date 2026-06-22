using System;
using System.Collections.Generic;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IAuditLogService
    {
        IReadOnlyList<AuditLogEntry> Load(DateTime? fromDate, DateTime? toDate, string keyword, int maxRows);
    }
}
