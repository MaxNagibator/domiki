using Domiki.Web.Business.Core;
using Domiki.Web.Data;
using System.Timers;

namespace Domiki.Web.Business
{
    public class CalculateInfo
    {
        public int PlayerId { get; set; }
        public int ObjectId { get; set; }

        /// <summary>
        /// дата когда событие должно выполнится
        /// </summary>
        public DateTime Date { get; set; }
        public CalculateTypes Type { get; set; }
    }

    public enum CalculateTypes
    {
        Domiks = 1,
    }

    public class Calculator
    {
        private IServiceProvider _serviceProvider;
        private List<CalculateInfo> _datas;
        private DateTime? _minDate;
        private System.Timers.Timer t;
        private bool _isInit;
        private int _errorCount = 0;

        public Calculator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Insert(CalculateInfo cData)
        {
            if (_datas == null)
            {
                Init();
            }
            // DbLogsWorker.WriteExecuteMessage(null, "Calculator - add data: " + cData.PlayerId + " - " + cData.ObjectId + " - " + cData.Type, LogsMessageTypes.Calculator);
            var index = _datas.FindIndex(x => x.Date > cData.Date);
            if (index == -1)
            {
                _datas.Add(cData);
            }
            else
            {
                _datas.Insert(index, cData);
            }
            _minDate = _datas[0].Date;
        }

        public void Remove(int playerId, long objectId, CalculateTypes type)
        {
            if (_datas == null)
            {
                Init();
            }
            var index = _datas.FindIndex(x => x.ObjectId == objectId && x.Type == type && x.PlayerId == playerId);
            if (index == -1)
            {
                return;
            }
            // DbLogsWorker.WriteExecuteMessage(null, "Calculator - remove data: " + playerId + " - " + objectId + " - " + type, LogsMessageTypes.Calculator);
            _datas.RemoveAt(index);
            if (index == 0)
            {
                _minDate = _datas[0].Date;
            }
        }

        public void CheckInit()
        {
            //_isInit = true;
            if (_datas == null && _isInit == false)
            {
                _isInit = true;
                Init();
                if (_datas.Count > 0)
                {
                    _minDate = _datas[0].Date;
                }
                // DbLogsWorker.WriteExecuteMessage(null, "Calculator - init: count = " + _datas.Count + " , min date = " + _minDate, LogsMessageTypes.Calculator);
            }
        }

        private void Init()
        {
            _datas = GetCalculateDates();
            LastOrderDate = DateTime.Now;

            var th = new Thread(Execute);
            th.Start();
        }

        private void Execute()
        {
            t = new System.Timers.Timer();
            t.Interval = 25;
            t.Elapsed += Tick;
            t.Start();
        }
        private DateTime LastOrderDate;

        private void Tick(object sender, ElapsedEventArgs e)
        {
            if (_minDate != null)
            {
                var date = DateTimeHelper.GetNowDate();
                if (_minDate <= date)
                {
                    t.Stop();
                    var calcDate = _datas[0];
#if !DEBUG
                    try
                    {
#endif
                    var startDate = DateTime.Now;
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        CalculatorTick vasv = scope.ServiceProvider.GetRequiredService<CalculatorTick>();
                        UnitOfWork uow = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                        var result = vasv.Calculate(date, calcDate);
                        uow.Context.SaveChanges();
                        uow.Commit();


                    var time = (DateTime.Now - startDate).TotalMilliseconds;
                 //   DbLogsWorker.WriteExecuteMessage(null, "Calculator - tick success: " + calcDate.PlayerId + " - " + calcDate.ObjectId + " - " + calcDate.Type + " " + time + "ms", LogsMessageTypes.Calculator);
                    if (result)
                    {
                        _datas.Remove(calcDate);
                        _minDate = _datas.Count > 0 ? (DateTime?)_datas[0].Date : null;
                    //    DbLogsWorker.WriteExecuteMessage(null, "Calculator - tick remove data: " + calcDate.PlayerId + " - " + calcDate.ObjectId + " - " + calcDate.Type, LogsMessageTypes.Calculator);
                    }
                    }
                    _errorCount = 0;
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        _errorCount++;
                        DbLogsWorker.WriteExecuteMessage(null, "Calculator - tick trable: " + calcDate.PlayerId + " - " + calcDate.ObjectId + " - " + calcDate.Type + " | message: " + ex.Message, LogsMessageTypes.Calculator, stack: ex.StackTrace);
                    }
#endif

                    if (_errorCount < 10)
                    {
                        t.Start();
                    }

                }
                else if ((date - LastOrderDate).TotalSeconds > 3600)
                {
                    //todo это костыль, так как были траблы с очередью, но я вроде уже нашёл косяк, но псть пока побудет
                    var prevMinDate = _minDate;
                    _datas = _datas.OrderBy(x => x.Date).ToList();
                    _minDate = _datas.Count > 0 ? (DateTime?)_datas[0].Date : null;
                   // DbLogsWorker.WriteExecuteMessage(null, "Calculator - order datas: count = " + _datas.Count + " prevMinDate = " + prevMinDate + " mindate = " + _minDate, LogsMessageTypes.Calculator);
                    LastOrderDate = date;
                }
            }
        }


        public List<CalculateInfo> GetCalculateDates()
        {
           return new List<CalculateInfo>();
            //using (var context = new Data.MinerContext())
            //{
            //    var dates = new List<CalculateInfo>();
            //    var dbPlayerBuildings = context.PlayerBuilding.Where(s => s.UpgradeSeconds != null).ToList();
            //    foreach (var dbStorage in dbPlayerBuildings)
            //    {
            //        var compliteDate = ((DateTime)dbStorage.UpgradeCalculateDate).AddSeconds((double)dbStorage.UpgradeSeconds);
            //        dates.Add(new CalculateInfo
            //        {
            //            PlayerId = dbStorage.PlayerId,
            //            ObjectId = dbStorage.Id,
            //            Date = compliteDate,
            //            Type = CalculateTypes.Building,
            //        });
            //    }

            //    dates = dates.OrderBy(x => x.Date).ToList();
            //    return dates;
            //}
        }
    }
}
