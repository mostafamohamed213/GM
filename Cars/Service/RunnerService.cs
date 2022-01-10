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

        public List<Runner> getAllRunners()
        {
            List<Runner> allRunners = db.Runners.ToList();
            try
            {
                if (allRunners is not null)
                {
                    return allRunners;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
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


        internal long EditRunner(long RunnerID, RunnerViewModel model)
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
