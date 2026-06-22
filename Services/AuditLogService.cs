using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLXeMay.Class;
using QLXeMay.Infrastructure;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal sealed class AuditLogService : IAuditLogService
    {
        public IReadOnlyList<AuditLogEntry> Load(DateTime? fromDate, DateTime? toDate, string keyword, int maxRows)
        {
            if (maxRows <= 0) maxRows = 200;
            if (maxRows > 5000) maxRows = 5000;

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                Function.Param("@maxRows", maxRows)
            };

            string where = "WHERE 1=1";
            if (fromDate.HasValue)
            {
                where += " AND createdat >= @fromDate";
                parameters.Add(Function.Param("@fromDate", fromDate.Value.Date));
            }

            if (toDate.HasValue)
            {
                where += " AND createdat < @toDate";
                parameters.Add(Function.Param("@toDate", toDate.Value.Date.AddDays(1)));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where += " AND (eventtype LIKE @keyword OR username LIKE @keyword OR detail LIKE @keyword)";
                parameters.Add(Function.Param("@keyword", "%" + keyword.Trim() + "%"));
            }

            try
            {
                DataTable table = Function.GetDataToTable(
                    @"SELECT TOP (@maxRows) auditid, eventtype, username, userid, detail, createdat
                      FROM tblauditlog " + where + @"
                      ORDER BY createdat DESC, auditid DESC",
                    parameters.ToArray());

                List<AuditLogEntry> entries = new List<AuditLogEntry>();
                foreach (DataRow row in table.Rows)
                {
                    entries.Add(new AuditLogEntry
                    {
                        AuditId = Convert.ToInt32(row["auditid"]),
                        EventType = row["eventtype"].ToString(),
                        UserName = row["username"] == DBNull.Value ? string.Empty : row["username"].ToString(),
                        UserId = row["userid"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["userid"]),
                        Detail = row["detail"] == DBNull.Value ? string.Empty : row["detail"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["createdat"])
                    });
                }

                return entries;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Cannot load audit log.", ex);
                throw new InvalidOperationException("Không thể tải nhật ký hệ thống: " + ex.Message, ex);
            }
        }
    }
}
