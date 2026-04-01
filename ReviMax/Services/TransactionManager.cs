using System;
using Autodesk.Revit.DB;
using ReviMax.Config;

namespace ReviMax.Services
{
    public static class TransactionManager
    {
        //private static readonly ILogger logger = MicrosoftLogger.CreateLogger<ReviMaxApp>();
        public static void StartTransaction(this Document doc,string transactionName, Action<Document> action)
        {
            
            using var transaction = new Transaction(doc, transactionName);

            try
            {
                transaction.Start();
                ReviMaxLog.Information($"Transaction {transactionName} started.");
                action(doc);
                if (transaction.GetStatus() == TransactionStatus.Started)
                {
                    transaction.Commit();
                    ReviMaxLog.Information($"Transaction {transactionName} committed.");
                }
                if (transaction.GetStatus() == TransactionStatus.Committed)
                {
                    ReviMaxLog.Information($"Transaction {transactionName} completed successfully.");
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                if (transaction.GetStatus() == TransactionStatus.Started)
                {
                    transaction.RollBack();
                    ReviMaxLog.Information($"Transaction {transactionName} rolled back due to operation cancellation.");
                }

                throw;
            }
            catch (Exception ex)
            {
                if (transaction.GetStatus() == TransactionStatus.Started)
                {
                    transaction.RollBack();
                }
                ReviMaxLog.Warning($"Transaction Failed {transactionName} failed: {ex.Message}");
                throw;
            }

        }
    }
}
