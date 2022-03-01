using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using Microsoft.EntityFrameworkCore;
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

        public PagingViewModel<OrderDetails> getOrderLinesWithChangelength(int currentPageIndex, int length,string runnerID)
        {
            TablesMaxRows.IndexRunnerMaxRows = length;
            return getAllRunners(currentPageIndex,null,runnerID);
        }
        public PagingViewModel<OrderDetails> getAllRunners(int currentPage,string search,string runnerID)
        {
            List<OrderDetails> allRunners = db.OrderDetails.Where(r=>r.RunnerID== runnerID).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).Include(c => c.Order.Vehicle).ToList();

            if(search!=null)
            {
                allRunners= db.OrderDetails.Where(r => r.RunnerID == runnerID && r.Items.Contains(search)).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).Include(c => c.Order.Vehicle).ToList();
            }
            try
            {
                if (allRunners is not null)
                {
                    if (search != null)
                    {
                        var result = paginate(allRunners, currentPage,runnerID);
                        result.itemsCount= db.OrderDetails.Where(r => r.RunnerID == runnerID && r.Items.Contains(search)).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).Count();
                        double pageCount = (double)(result.itemsCount / Convert.ToDecimal(TablesMaxRows.IndexRunnerMaxRows));
                        result.PageCount = (int)Math.Ceiling(pageCount);
                        result.CurrentPageIndex = currentPage;
                        result.itemsCount = result.itemsCount;
                        return result;
                    }
                 return paginate(allRunners, currentPage,runnerID);
                }
                return paginate(null, currentPage,runnerID);
            }
            catch (Exception)
            {
                return paginate(null, currentPage,runnerID);
            }
        }

     
        private PagingViewModel<OrderDetails> paginate(List<OrderDetails> runners, int currentPage,string runnerID)
        {
            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = runners.ToList();
            var itemsCount = db.OrderDetails.Where(r => r.RunnerID == runnerID).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexRunnerMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexRunnerMaxRows;

            return viewModel;
        }

        public OrderDetails GetRunnernByID(long orderID, string RunnerId)
        {
            try
            {
                OrderDetails runner = db.OrderDetails.Where(r => r.OrderDetailsID==orderID && r.RunnerID == RunnerId).Include(c => c.Order.Vehicle).FirstOrDefault();
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
        /*  public long AddRunner(RunnerViewModel model)
          {
              try
              {
                  if (model is not null)
                  {
                      OrderDetails addRunner = new OrderDetails()
                      {
                          SystemUserCreate=model.SystemUserCreate,
                          Name = model.Name,
                          Details = model.Details,
                          Enable = true,
                      };
                      db.OrderDetails.Add(addRunner);
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
                      //var orderlines = db.OrderDetails.Where(or => or.RunnerID == runner.RunnerID).FirstOrDefault();
                      //if (orderlines != null)
                      //{
                      //    return -1;
                      //}
                      //runner.Enable = false;
                      //db.SaveChanges();
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
          }*/


    }
}
