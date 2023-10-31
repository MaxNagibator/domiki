using Domiki.Data;
using Microsoft.EntityFrameworkCore;

namespace Domiki.Web.Data
{
    public class UnitOfWork : IDisposable
    {
        private bool isRollbacked = false;
        private bool isCommitted = false;
        public Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction Transaction { get; }
        public ApplicationDbContext Context { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            Transaction = context.Database.BeginTransaction();
            Context = context;
        }

        public void Commit()
        {
            if (isCommitted || isRollbacked)
            {
                throw new Exception("commit or rollback has been called.");
            }

            Transaction.Commit();

            Console.WriteLine("COMMIT");
            isCommitted = true;
        }

        public void Rollback()
        {
            if (isCommitted || isRollbacked)
            {
                throw new Exception("commit or rollback has been called.");
            }

            Transaction.Rollback();
            isRollbacked = true;
        }

        public void Dispose()
        {
            if (!isCommitted && !isRollbacked)
            {
                Console.WriteLine("Rollback");
                Rollback();
            }

            Transaction.Dispose();
        }
    }
}
