using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class RunnerService
    {
        public CarsContext db { get; set; }
        public RunnerService(CarsContext carsContext)
        {
            db = carsContext;
        }

        public PagingViewModel<Runner> getOrderLinesWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexRunnerMaxRows = length;
            return getAllRunners(currentPageIndex,null);
        }
        public PagingViewModel<Runner> getAllRunners(int currentPage,string? search)
        {
            List<Runner> allRunners = db.Runners.Where(r=>r.Enable).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).ToList();

            if(search!=null)
            {
                allRunners= db.Runners.Where(r => r.Enable && r.Name.Contains(search)).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).ToList();
            }
            try
            {
                if (allRunners is not null)
                {
                    if (search != null)
                    {
                        var result = paginate(allRunners, currentPage);
                        result.itemsCount= db.Runners.Where(r => r.Enable && r.Name.Contains(search)).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).Count();
                        double pageCount = (double)(result.itemsCount / Convert.ToDecimal(TablesMaxRows.IndexRunnerMaxRows));
                        result.PageCount = (int)Math.Ceiling(pageCount);
                        result.CurrentPageIndex = currentPage;
                        result.itemsCount = result.itemsCount;
                        return result;
                    }
                 return paginate(allRunners, currentPage);
                }
                return paginate(null, currentPage);
            }
            catch (Exception)
            {
                return paginate(null, currentPage);
            }
        }

     
        private PagingViewModel<Runner> paginate(List<Runner> runners, int currentPage)
        {
            PagingViewModel<Runner> viewModel = new PagingViewModel<Runner>();
            viewModel.items = runners.ToList();
            var itemsCount = db.Runners.Where(r => r.Enable).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexRunnerMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexRunnerMaxRows;

            return viewModel;
        }
        public long AddRunner(RunnerViewModel model)
        {
            try
            {
                if (model is not null)
                {
                    Runner addRunner = new Runner()
                    {
                        SystemUserCreate=model.SystemUserCreate,
                        Name = model.Name,
                        Details = model.Details,
                        Enable = true,
                    };
                    db.Runners.Add(addRunner);
                    db.SaveChanges();
                    return addRunner.RunnerID;
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public long DeleteRunner(long RunnerID)
        {
            try
            {
                Runner runner = db.Runners.FirstOrDefault(r => r.RunnerID == RunnerID && r.Enable);
                if (runner != null)
                {
                    var orderlines = db.OrderDetails.Where(or => or.RunnerID == runner.RunnerID).FirstOrDefault();
                    if (orderlines != null)
                    {
                        return -1;
                    }
                    runner.Enable = false;
                    db.SaveChanges();
                    return runner.RunnerID;
                }

                return 0;
            }
            catch (Exception)
            {

                return -1;
            }

        }


        internal long EditRunner(long RunnerID, Runner model)
        {
            try
            {
                Runner EditRunner = db.Runners.Where(r => r.Enable).FirstOrDefault(r =>r.RunnerID == RunnerID);
                if (EditRunner is null)
                {
                    return 0;
                }

                EditRunner.Name = model.Name;
                EditRunner.Details = model.Details;
                EditRunner.SystemUserUpdate = model.SystemUserUpdate;
                EditRunner.DTsUpdate= DateTime.Now;
                db.SaveChanges();
                return RunnerID;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public Runner GetRunnernByID(long RunnerId)
        {
            try
            {
               Runner runner = db.Runners.Where(r => r.RunnerID == RunnerId && r.Enable).FirstOrDefault();
                if (runner is not null)
                {
                    return runner;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
