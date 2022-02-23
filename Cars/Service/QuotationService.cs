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
    public class QuotationService
    {
        public CarsContext db { get; set; }
        public QuotationService(CarsContext carsContext)
        {
            db = carsContext;
        }
        public PagingViewModel<Quotation> getQuotations(int currentPage ,string userId)
        {
            var quotations = db.Quotations.Include("Status").Where(c => c.StatusID == 2&& c.SystemUserCreate == userId).Skip((currentPage - 1) * TablesMaxRows.IndexQuotationMaxRows).Take(TablesMaxRows.IndexQuotationMaxRows).ToList();
            PagingViewModel<Quotation> viewModel = new PagingViewModel<Quotation>();
            viewModel.items = quotations.ToList();
            var itemsCount = db.Quotations.Where(c => c.StatusID == 2).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexQuotationMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexQuotationMaxRows;
            return viewModel;

        }
        internal PagingViewModel<Quotation> SearchQuotations(string search,string UserId)
        {
            var quotations = db.Quotations.Where(c => c.StatusID == 2&&c.SystemUserCreate == UserId && c.QuotationID == long.Parse(search)).Include("Status").Take(100).ToList();
            PagingViewModel<Quotation> viewModel = new PagingViewModel<Quotation>();
            viewModel.items = quotations.ToList();
            var itemsCount = quotations.Count;
            double pageCount = 1;
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = 1;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = 100;
            return viewModel;
        }
        internal long getCountOrderLines(string UserId)
        {
          return db.OrderDetails.Include(c=>c.Order).Where(c => c.StatusID == 2 && c.WorkflowID == 4 && !c.QuotationID.HasValue && c.Order.SystemUserCreate == UserId).Count();
        }

        internal List<OrderDetails> getOrderLines(string UserId)
        {
           return db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 4 && !c.QuotationID.HasValue && c.Order.SystemUserCreate == UserId).Include(c=>c.UserBranch.Branch).Include(c=>c.Order).Include(c => c.Order.Customer).Include(c => c.Order.Customer.CustomerContacts).ToList();

        }

        public PagingViewModel<Quotation> getQuotationsWithChangelength(int currentPageIndex, int length,string UserId)
        {
            TablesMaxRows.IndexQuotationMaxRows = length;
            return getQuotations(currentPageIndex, UserId);
        }

        readonly object _object = new object();
        internal CreateQuotationViewModel CreateQuotation(List<long> ids ,string User)
        {
            try
            {
                lock (_object)
                {
                    var SelectedOrderLines = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 4 && c.QuotationID.HasValue && ids.Contains(c.OrderDetailsID)).ToList();
                    if (SelectedOrderLines is not null && SelectedOrderLines.Count > 0)
                    {
                        CreateQuotationViewModel model = new CreateQuotationViewModel()
                        {
                            status = -2,
                            orderDetails = SelectedOrderLines,
                        };
                        return model;
                    }
                   var OrderDetails= db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 4 && !c.QuotationID.HasValue && ids.Contains(c.OrderDetailsID)).ToList();
                    Quotation quotation = new Quotation()
                    {
                        OrderDetails = OrderDetails,
                        StatusID = 2,
                        DTsCreate = DateTime.Now,
                        SystemUserCreate = User,
                    };
                    db.Quotations.Add(quotation);
                    foreach (var item in OrderDetails)
                    {
                        item.DTsWorflowEnter = DateTime.Now;
                    }
                    db.SaveChanges();
                    QuotationStatusLogs log = new QuotationStatusLogs()
                    {
                        StatusID = 2,
                        QuotationID = quotation.QuotationID,
                        SystemUserID = User,
                        DTsCreate = DateTime.Now,
                    };
                    db.QuotationStatusLogs.Add(log);
                    db.SaveChanges();
                    CreateQuotationViewModel model2 = new CreateQuotationViewModel()
                    {
                        status = 1,
                        Quotation = quotation,
                        QuotationId = quotation.QuotationID
                    };
                    return model2;

                }
            }
            catch (Exception)
            {

                CreateQuotationViewModel model = new CreateQuotationViewModel()
                {
                    status = -1,
                  
                };
                return model;
            }
           
        }

        internal int Confirmation(long quotationId,string user)
        {
            Quotation quotation = db.Quotations.Include(c=>c.OrderDetails).FirstOrDefault(c => c.QuotationID == quotationId);
            if (quotation is not null && quotation.SystemUserCreate==user)
            {
                quotation.Confirmed = true;
                quotation.SystemUserUpdate = user;
                quotation.DTsUpdate = DateTime.Now;
                foreach (var orderDetails in quotation.OrderDetails)
                {
                    orderDetails.WorkflowID = orderDetails.Price.Value >= 1000 ? 5 : 6;
                    db.WorkflowOrderDetailsLogs.Add(new WorkflowOrderDetailsLog {
                        DTsCreate =DateTime.Now,
                        Active=true,
                        OrderDetailsID=orderDetails.OrderDetailsID,
                        SystemUserID=user,
                        WorkflowID=  4,                        
                    });
                }
                db.SaveChanges();
                return 1;
            }
            return -1;
        }

        internal Quotation getQuotationByQuotationID(long quotationID ,string userId)
        {
         return db.Quotations.Include(c=>c.QuotationDocument).Include(c => c.OrderDetails).Include("OrderDetails.Order.Vehicle").Include("OrderDetails.UserBranch.Branch").FirstOrDefault(c => c.QuotationID == quotationID && c.SystemUserCreate== userId);       
        }

        internal void CreateFilePath(string path ,long quotationID,string filename,string User)
        {
            db.QuotationDocuments.Add(new QuotationDocument() { DTsCreate=DateTime.Now, Path = path ,QuotationID = quotationID ,SystemUserID = User,Comment=filename});
            db.SaveChanges();
        }
    }
}
