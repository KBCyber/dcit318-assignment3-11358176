using System;
using System.Collections.Generic;

namespace DCIT318_FinanceApp
{
    public record Transaction(
        int Id,
        DateTime Date,
        decimal Amount,
        string Category
    );

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing {transaction.Category} → Amount: {transaction.Amount:F2} on {transaction.Date:d}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processing {transaction.Category} → Amount: {transaction.Amount:F2} on {transaction.Date:d}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processing {transaction.Category} → Amount: {transaction.Amount:F2} on {transaction.Date:d}");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be empty.", nameof(accountNumber));

            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");

            AccountNumber = accountNumber;
            Balance = initialBalance;
        }
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] Deducted {transaction.Amount:F2}. New balance: {Balance:F2}");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] {transaction.Category} of {transaction.Amount:F2} applied. Updated balance: {Balance:F2}");
        }
    }
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System Demo ===\n");

            // i) Create SavingsAccount
            var account = new SavingsAccount(accountNumber: "SA-001", initialBalance: 1000m);
            Console.WriteLine($"Created SavingsAccount {account.AccountNumber} with initial balance {account.Balance:F2}\n");

            var t1 = new Transaction(Id: 1, Date: DateTime.Now, Amount: 120.50m, Category: "Groceries");
            var t2 = new Transaction(Id: 2, Date: DateTime.Now, Amount: 300m, Category: "Utilities");
            var t3 = new Transaction(Id: 3, Date: DateTime.Now, Amount: 700m, Category: "Entertainment");

            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            bankTransfer.Process(t2);
            cryptoWallet.Process(t3);
            Console.WriteLine();

            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);
            Console.WriteLine();

            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("=== Transactions Summary ===");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"Id:{tx.Id} | {tx.Date:d} | {tx.Category} | {tx.Amount:F2}");
            }

            Console.WriteLine($"\nFinal Balance: {account.Balance:F2}");
            Console.WriteLine("\n=== End ===");
        }

        public static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
