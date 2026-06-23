using QLXeMay.Domain;
using System.Linq;

namespace QLXeMay.Tests
{
    internal static class Program
    {
        private static int failures;

        private static int Main()
        {
            TestLineTotal();
            TestSalesTotals();
            TestSuggestedSellingPrice();
            TestStockAfterSale();
            TestStockAfterPurchase();
            TestInvalidStockSale();
            TestPasswordPolicyAcceptsStrongPassword();
            TestPasswordPolicyRejectsWeakPassword();
            TestPasswordPolicyRejectsUserNameReuse();
            TestRolePermissionMatrix();
            TestAdministratorHasEveryFeature();
            TestManagerGroupAccess();
            TestSalesGroupAccess();
            TestWarehouseGroupAccess();
            TestViewerGroupAccess();
            TestCustomerGroupAccess();
            TestEveryFeaturePermissionIsGrantedToAdministrator();
            TestUnknownRoleHasNoAccess();

            if (failures == 0)
            {
                Console.WriteLine("All tests passed.");
                return 0;
            }

            Console.Error.WriteLine($"{failures} test(s) failed.");
            return 1;
        }

        private static void TestLineTotal()
        {
            AssertEqual(1_800_000, InvoiceCalculator.CalculateLineTotal(2, 1_000_000, 10), "line total with discount");
            AssertEqual(0, InvoiceCalculator.CalculateLineTotal(2, 1_000_000, 100), "line total with full discount");
            AssertEqual(2_000_000, InvoiceCalculator.CalculateLineTotal(2, 1_000_000, -5), "negative discount clamps to zero");
        }

        private static void TestSalesTotals()
        {
            AssertEqual(100_000, InvoiceCalculator.CalculateSalesTax(1_000_000), "sales tax");
            AssertEqual(500_000, InvoiceCalculator.CalculateSalesDeposit(1_000_000), "sales deposit");
        }

        private static void TestSuggestedSellingPrice()
        {
            AssertEqual(22_000_000, InvoiceCalculator.CalculateSuggestedSellingPrice(20_000_000), "suggested selling price");
        }

        private static void TestStockAfterSale()
        {
            AssertEqual(7, InvoiceCalculator.StockAfterSale(10, 3), "stock after sale");
            AssertEqual(10, InvoiceCalculator.StockAfterSaleRollback(7, 3), "stock rollback after sale line delete");
        }

        private static void TestStockAfterPurchase()
        {
            AssertEqual(13, InvoiceCalculator.StockAfterPurchase(10, 3), "stock after purchase");
            AssertEqual(7, InvoiceCalculator.StockAfterPurchaseRollback(10, 3), "stock rollback after purchase line delete");
            AssertEqual(0, InvoiceCalculator.StockAfterPurchaseRollback(2, 5), "purchase rollback never below zero");
        }

        private static void TestInvalidStockSale()
        {
            try
            {
                InvoiceCalculator.StockAfterSale(2, 3);
                Fail("sale quantity above stock should throw");
            }
            catch (InvalidOperationException)
            {
                Pass("sale quantity above stock throws");
            }
        }


        private static void TestPasswordPolicyAcceptsStrongPassword()
        {
            AssertEqual(0, PasswordPolicy.Validate("Str0ng@Pass!", "seller", "Nguyen Van A").Count, "strong password accepted");
        }

        private static void TestPasswordPolicyRejectsWeakPassword()
        {
            IReadOnlyList<string> errors = PasswordPolicy.Validate("abc", "seller", "Nguyen Van A");
            if (errors.Count >= 4)
            {
                Pass("weak password rejected with multiple policy errors");
                return;
            }

            Fail("weak password should return multiple policy errors");
        }

        private static void TestPasswordPolicyRejectsUserNameReuse()
        {
            IReadOnlyList<string> errors = PasswordPolicy.Validate("Seller@12345", "seller", "Nguyen Van A");
            if (errors.Any(e => e.Contains("tên đăng nhập")))
            {
                Pass("password containing username rejected");
                return;
            }

            Fail("password containing username should be rejected");
        }

        private static void TestRolePermissionMatrix()
        {
            AssertPermissions(AccessControl.Administrator, new[]
            {
                "ManageEmployees", "ManageCustomers", "ManageSuppliers", "ManageProducts",
                "SalesInvoice", "PurchaseInvoice", "Search", "Reports", "AiAssistant", "UserAdmin", "AuditLog"
            });
            AssertPermissions(AccessControl.Manager, new[]
            {
                "ManageEmployees", "ManageCustomers", "ManageSuppliers", "ManageProducts",
                "SalesInvoice", "PurchaseInvoice", "Search", "Reports", "AiAssistant"
            });
            AssertPermissions(AccessControl.Sales, new[]
            {
                "ManageCustomers", "SalesInvoice", "Search", "Reports", "AiAssistant"
            });
            AssertPermissions(AccessControl.Warehouse, new[]
            {
                "ManageSuppliers", "ManageProducts", "PurchaseInvoice", "Search", "Reports"
            });
            AssertPermissions(AccessControl.Viewer, new[] { "Search", "Reports" });
        }

        private static void TestAdministratorHasEveryFeature()
        {
            foreach (AppFeature feature in Enum.GetValues<AppFeature>())
            {
                AssertCanAccess(AccessControl.Administrator, feature, true);
            }
        }

        private static void TestManagerGroupAccess()
        {
            AssertCanAccess(AccessControl.Manager, AppFeature.Employees, true);
            AssertCanAccess(AccessControl.Manager, AppFeature.Products, true);
            AssertCanAccess(AccessControl.Manager, AppFeature.SalesInvoice, true);
            AssertCanAccess(AccessControl.Manager, AppFeature.PurchaseInvoice, true);
            AssertCanAccess(AccessControl.Manager, AppFeature.UserAdmin, false);
            AssertCanAccess(AccessControl.Manager, AppFeature.AuditLog, false);
        }

        private static void TestSalesGroupAccess()
        {
            AssertCanAccess(AccessControl.Sales, AppFeature.Customers, true);
            AssertCanAccess(AccessControl.Sales, AppFeature.SalesInvoice, true);
            AssertCanAccess(AccessControl.Sales, AppFeature.AiAssistant, true);
            AssertCanAccess(AccessControl.Sales, AppFeature.PurchaseInvoice, false);
            AssertCanAccess(AccessControl.Sales, AppFeature.Products, false);
            AssertCanAccess(AccessControl.Sales, AppFeature.Employees, false);
        }

        private static void TestWarehouseGroupAccess()
        {
            AssertCanAccess(AccessControl.Warehouse, AppFeature.PurchaseInvoice, true);
            AssertCanAccess(AccessControl.Warehouse, AppFeature.Products, true);
            AssertCanAccess(AccessControl.Warehouse, AppFeature.Suppliers, true);
            AssertCanAccess(AccessControl.Warehouse, AppFeature.SalesInvoice, false);
            AssertCanAccess(AccessControl.Warehouse, AppFeature.Customers, false);
            AssertCanAccess(AccessControl.Warehouse, AppFeature.AiAssistant, false);
        }

        private static void TestViewerGroupAccess()
        {
            AssertCanAccess(AccessControl.Viewer, AppFeature.Search, true);
            AssertCanAccess(AccessControl.Viewer, AppFeature.Reports, true);
            AssertCanAccess(AccessControl.Viewer, AppFeature.SalesInvoice, false);
            AssertCanAccess(AccessControl.Viewer, AppFeature.PurchaseInvoice, false);
            AssertCanAccess(AccessControl.Viewer, AppFeature.Products, false);
            AssertCanAccess(AccessControl.Viewer, AppFeature.UserAdmin, false);
        }

        private static void TestCustomerGroupAccess()
        {
            AssertPermissions(AccessControl.Customer, new[] { "ShopOrder" });

            if (AccessControl.RoleHasPermission(AccessControl.Customer, "ShopOrder"))
            {
                Pass("customer has ShopOrder permission");
            }
            else
            {
                Fail("customer should have ShopOrder permission");
            }

            // A shopper must never reach any staff feature.
            AssertCanAccess(AccessControl.Customer, AppFeature.SalesInvoice, false);
            AssertCanAccess(AccessControl.Customer, AppFeature.Products, false);
            AssertCanAccess(AccessControl.Customer, AppFeature.Customers, false);
            AssertCanAccess(AccessControl.Customer, AppFeature.Reports, false);
            AssertCanAccess(AccessControl.Customer, AppFeature.Search, false);
            AssertCanAccess(AccessControl.Customer, AppFeature.UserAdmin, false);

            // ShopOrder is customer-only; staff roles never get it.
            if (!AccessControl.RoleHasPermission(AccessControl.Administrator, "ShopOrder")
                && !AccessControl.RoleHasPermission(AccessControl.Sales, "ShopOrder"))
            {
                Pass("ShopOrder is not granted to staff roles");
            }
            else
            {
                Fail("ShopOrder should be customer-only");
            }

            if (AccessControl.IsCustomer("Customer") && !AccessControl.IsCustomer("Sales") && !AccessControl.IsCustomer(null))
            {
                Pass("IsCustomer detects the customer role only");
            }
            else
            {
                Fail("IsCustomer should detect the customer role only");
            }
        }

        private static void TestEveryFeaturePermissionIsGrantedToAdministrator()
        {
            foreach (AppFeature feature in Enum.GetValues<AppFeature>())
            {
                string permission = AccessControl.PermissionFor(feature);
                if (AccessControl.RoleHasPermission(AccessControl.Administrator, permission))
                {
                    Pass("admin has permission for feature " + feature);
                }
                else
                {
                    Fail("admin missing permission for feature " + feature);
                }
            }
        }

        private static void TestUnknownRoleHasNoAccess()
        {
            if (AccessControl.PermissionsFor("Ghost").Count == 0
                && !AccessControl.RoleCanAccess("Ghost", AppFeature.Search)
                && !AccessControl.RoleCanAccess(null, AppFeature.Reports))
            {
                Pass("unknown / null role has no access");
                return;
            }

            Fail("unknown / null role should have no access");
        }

        private static void AssertPermissions(string roleName, string[] expected)
        {
            var actual = new HashSet<string>(AccessControl.PermissionsFor(roleName));
            var expectedSet = new HashSet<string>(expected);
            if (actual.SetEquals(expectedSet))
            {
                Pass("permission set for role " + roleName);
                return;
            }

            string missing = string.Join(",", expectedSet.Except(actual));
            string extra = string.Join(",", actual.Except(expectedSet));
            Fail($"permission set for role {roleName}: missing=[{missing}] extra=[{extra}]");
        }

        private static void AssertCanAccess(string roleName, AppFeature feature, bool expected)
        {
            bool actual = AccessControl.RoleCanAccess(roleName, feature);
            if (actual == expected)
            {
                Pass($"{roleName} access to {feature} = {expected}");
                return;
            }

            Fail($"{roleName} access to {feature}: expected {expected}, actual {actual}");
        }

        private static void AssertEqual(double expected, double actual, string name)
        {
            if (Math.Abs(expected - actual) <= 0.0001)
            {
                Pass(name);
                return;
            }

            Fail($"{name}: expected {expected}, actual {actual}");
        }

        private static void AssertEqual(int expected, int actual, string name)
        {
            if (expected == actual)
            {
                Pass(name);
                return;
            }

            Fail($"{name}: expected {expected}, actual {actual}");
        }

        private static void Pass(string name)
        {
            Console.WriteLine("[PASS] " + name);
        }

        private static void Fail(string name)
        {
            failures++;
            Console.Error.WriteLine("[FAIL] " + name);
        }
    }
}
