using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Consts
{
    public static class TablesMaxRows
    {
        public static int IndexOrdersMaxRows { get; set; }
        public static int IndexOrdersDraftMaxRows { get; set; }
        public static int IndexOrderLinesMaxRows { get; set; }
        public static int IndexOrderLinesUsedMaxRows { get; set; }
        public static int IndexPricingMaxRows { get; set; }
        public static int IndexQuotationMaxRows { get; set; }
        public static int IndexLaborMaxRows { get; set; }
        public static int IndexFinanceOrderLinesMaxRows { get; set; }
        public static int IndexInventoryOrderLinesMaxRows { get; set; }
        public static int IndexPurchasingMaxRows { get; set; }
        public static int IndexVendorMaxRows { get; set; }
        public static int IndexRunnerMaxRows { get; set; }
        public static int IndexAllOrderLinesRows { get; set; }
        public static int IndexRunnerOrderLinesRows { get; set; }
        public static int IndexBrandRows { get; set; }
        public static int IndexBrandModelsRows { get; set; }
        public static int IndexModelYearsRows { get; set; }

        public static int IndexDeliverysRows { get; set; }
        static TablesMaxRows()
        {
            IndexOrdersMaxRows = 10;
            IndexOrdersDraftMaxRows = 10;
            IndexOrderLinesMaxRows = 10;
            IndexOrderLinesUsedMaxRows = 10;
            IndexPricingMaxRows = 10;
            IndexLaborMaxRows = 10;
            IndexQuotationMaxRows = 10;
            IndexPurchasingMaxRows = 10;
            IndexFinanceOrderLinesMaxRows = 10;
            IndexVendorMaxRows = 10;
            IndexRunnerMaxRows = 10;
            IndexAllOrderLinesRows = 10;
            IndexInventoryOrderLinesMaxRows = 10;
            IndexRunnerOrderLinesRows = 10;
            IndexDeliverysRows = 10;
            IndexBrandRows = 10;
            IndexBrandModelsRows = 10;
            IndexModelYearsRows = 10;
        }
    }
}
