using Domiki.Web.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading;

namespace Domiki.Web.Business.Core
{
    public class CalculatorTick
    {
        private DomikManager _domikManager;

        public CalculatorTick(DomikManager domikManager)
        {
            _domikManager = domikManager;
        }

        public bool Calculate(DateTime date, CalculateInfo calcInfo)
        {
            switch (calcInfo.Type)
            {
                case CalculateTypes.Domiks:
                    return _domikManager.FinishDomik(date, calcInfo);
            }
            return false;
        }
    }
}
