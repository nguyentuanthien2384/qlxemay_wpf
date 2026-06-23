using System;
using System.Collections.Generic;
using System.Linq;
using QLXeMay.Models;

namespace QLXeMay.Domain
{
    /// <summary>
    /// Single source of truth for role-based access control.
    /// Both the database seeder and the UI gating read from here so the two can never drift apart.
    /// </summary>
    public enum AppFeature
    {
        Employees,
        JobCatalog,
        Customers,
        Suppliers,
        Products,
        ProductCatalog,
        SalesInvoice,
        PurchaseInvoice,
        Search,
        Reports,
        AiAssistant,
        UserAdmin,
        AuditLog
    }

    public static class AccessControl
    {
        public const string Administrator = "Administrator";
        public const string Manager = "Manager";
        public const string Sales = "Sales";
        public const string Warehouse = "Warehouse";
        public const string Viewer = "Viewer";

        // Self-service shopper role. Lives outside the staff feature matrix on purpose:
        // a customer never enters the staff MainWindow, they get their own storefront shell.
        public const string Customer = "Customer";

        // System actor used to satisfy the mandatory employee FK on self-service orders.
        public const string OnlineSalesJobId = "CVONLINE";
        public const string OnlineSalesEmployeeId = "NVONLINE";

        private static readonly IReadOnlyDictionary<AppFeature, string> FeaturePermissionMap =
            new Dictionary<AppFeature, string>
            {
                [AppFeature.Employees] = PermissionNames.ManageEmployees,
                [AppFeature.JobCatalog] = PermissionNames.ManageEmployees,
                [AppFeature.Customers] = PermissionNames.ManageCustomers,
                [AppFeature.Suppliers] = PermissionNames.ManageSuppliers,
                [AppFeature.Products] = PermissionNames.ManageProducts,
                [AppFeature.ProductCatalog] = PermissionNames.ManageProducts,
                [AppFeature.SalesInvoice] = PermissionNames.SalesInvoice,
                [AppFeature.PurchaseInvoice] = PermissionNames.PurchaseInvoice,
                [AppFeature.Search] = PermissionNames.Search,
                [AppFeature.Reports] = PermissionNames.Reports,
                [AppFeature.AiAssistant] = PermissionNames.AiAssistant,
                [AppFeature.UserAdmin] = PermissionNames.UserAdmin,
                [AppFeature.AuditLog] = PermissionNames.AuditLog
            };

        // Every permission that is actually wired to a feature (no dead permissions).
        public static readonly IReadOnlyList<string> AllPermissions =
            FeaturePermissionMap.Values.Distinct().ToArray();

        private static readonly IReadOnlyDictionary<string, HashSet<string>> RolePermissionMap =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [Administrator] = new HashSet<string>(AllPermissions),
                [Manager] = new HashSet<string>
                {
                    PermissionNames.ManageEmployees,
                    PermissionNames.ManageCustomers,
                    PermissionNames.ManageSuppliers,
                    PermissionNames.ManageProducts,
                    PermissionNames.SalesInvoice,
                    PermissionNames.PurchaseInvoice,
                    PermissionNames.Search,
                    PermissionNames.Reports,
                    PermissionNames.AiAssistant
                },
                [Sales] = new HashSet<string>
                {
                    PermissionNames.ManageCustomers,
                    PermissionNames.SalesInvoice,
                    PermissionNames.Search,
                    PermissionNames.Reports,
                    PermissionNames.AiAssistant
                },
                [Warehouse] = new HashSet<string>
                {
                    PermissionNames.ManageSuppliers,
                    PermissionNames.ManageProducts,
                    PermissionNames.PurchaseInvoice,
                    PermissionNames.Search,
                    PermissionNames.Reports
                },
                [Viewer] = new HashSet<string>
                {
                    PermissionNames.Search,
                    PermissionNames.Reports
                },
                [Customer] = new HashSet<string>
                {
                    PermissionNames.ShopOrder
                }
            };

        public static IReadOnlyList<string> AllRoleNames => RolePermissionMap.Keys.ToArray();

        public static IReadOnlyList<string> PermissionsFor(string roleName)
        {
            if (roleName != null && RolePermissionMap.TryGetValue(roleName, out HashSet<string> permissions))
            {
                return permissions.ToArray();
            }

            return Array.Empty<string>();
        }

        public static string PermissionFor(AppFeature feature)
        {
            return FeaturePermissionMap[feature];
        }

        public static bool RoleHasPermission(string roleName, string permission)
        {
            return roleName != null
                && RolePermissionMap.TryGetValue(roleName, out HashSet<string> permissions)
                && permissions.Contains(permission);
        }

        public static bool RoleCanAccess(string roleName, AppFeature feature)
        {
            return RoleHasPermission(roleName, FeaturePermissionMap[feature]);
        }

        public static IReadOnlyList<AppFeature> FeaturesFor(string roleName)
        {
            return FeaturePermissionMap.Keys.Where(f => RoleCanAccess(roleName, f)).ToArray();
        }

        public static bool IsCustomer(string roleName)
        {
            return string.Equals(roleName, Customer, StringComparison.OrdinalIgnoreCase);
        }
    }
}
