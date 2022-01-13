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

        public PagingViewModel<Runner> getAllRunners(int currentPage)
        {
            List<Runner> allRunners = db.Runners.Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).ToList();
            try
            {
                if (allRunners is not null)
                {
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
            var itemsCount = runners.Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexVendorMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexLaborMaxRows;

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
                Runner runner = db.Runners.FirstOrDefault(r => r.RunnerID == RunnerID);
                if (runner != null)
                {
                    db.Runners.Remove(runner);
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
                Runner EditRunner = db.Runners.FirstOrDefault(r =>r.RunnerID == RunnerID);
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
               Runner runner = db.Runners.Where(r => r.RunnerID == RunnerId).FirstOrDefault();
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
