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
