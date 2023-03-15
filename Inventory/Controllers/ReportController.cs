using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.ViewModels;

namespace Inventory.Controllers
{
    public class ReportController : MyController
    {
        ReportViewModel reportViewModel = new ReportViewModel();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Filter(string reportName)
        {
            reportViewModel.ReportName = reportName;

            if (reportName.Equals(Resource.SaleAmountOnlyReport))
            {                
                reportViewModel.ControllerName = "RpSaleAmountOnly";
                reportViewModel.ActionName = "SaleAmountOnlyReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountOnlyRp";
            }
            else if (reportName.Equals(Resource.SaleAmountSummaryReport))
            {
                reportViewModel.ControllerName = "RpSaleAmountSummary";
                reportViewModel.ActionName = "SaleAmountSummaryReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleAmountSummaryRp";
            }
            else if (reportName.Equals(Resource.SaleItemSimpleReport))
            {
                reportViewModel.ControllerName = "RpSaleItemSimple";
                reportViewModel.ActionName = "SaleItemSimpleReport";
                reportViewModel.HtmlGroupElementID = "subMenuSaleRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuSaleItemSimpleRp";
                reportViewModel.IsShowMenuFilter = true;
            }
            else if (reportName.Equals(Resource.PurchaseAmountOnlyReport))
            {
                reportViewModel.ControllerName = "RpPurchaseAmountOnly";
                reportViewModel.ActionName = "PurchaseAmountOnlyReport";
                reportViewModel.HtmlGroupElementID = "subMenuPurchaseRpGp";
                reportViewModel.HtmlActiveElementID = "subMenuPurchaseAmountOnlyRp";
            }

            return View(reportViewModel);
        }
    }
}