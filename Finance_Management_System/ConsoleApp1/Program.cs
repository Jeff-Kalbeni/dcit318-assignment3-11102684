using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FinanceManagementSystem
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // interface to process transaction
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // concrete class that implement ITransactionProcess
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processed bank transfer: Amount = {transaction.Amount}, Category = {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processed mobile money payment: Amount = {transaction.Amount}, Category = {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processed crypto wallet transaction: Amount = {transaction.Amount}, Category = {transaction.Category}");
        }
    }

    // the base class 
    public class Account
    {
        public string AccountNumber { get; }
        protected decimal Balance { get; set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Balance = initialBalance < 0 ? 0 : initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > 0) return;
            Balance += transaction.Amount;
        }
    }

    // Sealed class SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > 0) return;
            if (Math.Abs(transaction.Amount) > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }
            Balance += transaction.Amount;
            Console.WriteLine($"Updated balance for account {AccountNumber}: {Balance}");
        }
    }


    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            var savingsAccount = new SavingsAccount("08474584753", 1000);

            var transaction1 = new Transaction(1, DateTime.Now, -150, "Liquor");
            var transaction2 = new Transaction(2, DateTime.Now, -200, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now, -300, "Movies");

            var mobilemoney = new MobileMoneyProcessor();
            var bankTransfer = new BankTransferProcessor();
            var cryptoWallet = new CryptoWalletProcessor();

            mobilemoney.Process(transaction1);
            bankTransfer.Process(transaction2);
            cryptoWallet.Process(transaction3);

            savingsAccount.ApplyTransaction(transaction1);
            savingsAccount.ApplyTransaction(transaction2);
            savingsAccount.ApplyTransaction(transaction3);

            _transactions.Add(transaction1);
            _transactions.Add(transaction2);
            _transactions.Add(transaction3);

            Console.WriteLine($"Total transactions recorded: {_transactions.Count}");
        }
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
