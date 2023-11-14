using Domiki.Web.Business;
using Domiki.Web.Business.Core;
using Domiki.Web.Data;

namespace Domiki.Web.Tests
{
    public class TestCalculator : ICalculator
    {
        private Func<UnitOfWork> _uowFactory;
        private Func<UnitOfWork, CalculatorTick> _calculatorTickFactory;

        public TestCalculator(Func<UnitOfWork> uowFactory, Func<UnitOfWork, CalculatorTick> calculatorTickFactory)
        {
            _uowFactory = uowFactory;
            _calculatorTickFactory = calculatorTickFactory;
        }

        public void Insert(CalculateInfo calcDate)
        {
            using (var uow = _uowFactory())
            {
                CalculatorTick calculatorTick = _calculatorTickFactory(uow);
                calculatorTick.Calculate(DateTime.Now.AddYears(217), calcDate);
                uow.Context.SaveChanges();
                uow.Commit();
            }
        }

        public void Remove(int playerId, long objectId, CalculateTypes type)
        {
        }

        public void CheckInit()
        {
        }
    }
}
