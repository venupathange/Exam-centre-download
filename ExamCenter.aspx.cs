using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Theorem.EOrg.Core.BAL;
using Theorem.EOrg.Core.DEO;
using Theorem.EOrg.StudentCore.BAL;
using Theorem.EOrg.StudentCore.DEO;
using Theorem.EOrg.Framework.Utilities;
using DbLinq.Factory;
using System.Data;
using CrystalDecisions.Web;
using CrystalDecisions.Shared;
using CrystalDecisions.Enterprise;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.CrystalReports.Engine;
using BarCode;
using System.IO;
using System.Web.Services;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Utils;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.VBA;
using OfficeOpenXml.Table;
using System.Drawing;
using System.Web.Security;

public partial class Examination_ExamCenter : System.Web.UI.Page
{
    UserManager objUserManager = null;
    AdminManager objAdminManager = null;
    StudentManager _studentMgr = null;
    DataSet ds = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }


    protected void btnReport_Click(object sender, EventArgs e)
    {
        Examcentre();
    }
    private void Examcentre()
    {
        try
        {
            CreateStudentManagerInstance();
            DataSet ds = _studentMgr.GetExamCenterallocated(Convert.ToInt32(this.ddlexamtype.SelectedValue),
                                                               Convert.ToInt32(this.ddlexam.SelectedValue));

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet  

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Download");

                ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);

                int columncount = ds.Tables[0].Columns.Count;

                //Format the header for column 1-3  
                using (ExcelRange rng = ws.Cells["A1:D1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;               //Set Pattern for the background to Solid  
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue  
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                //Example how to Format Column 1 as numeric  
                using (ExcelRange col = ws.Cells[2, 1, 2 + ds.Tables[0].Rows.Count + 1, 1])
                {
                    //col.Style.Numberformat.Format = "#,##0.00";  
                    col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }

                for (int i = 0; i <= ds.Tables[0].Columns.Count; i++)
                {
                    //ws.Column(i).Width = 30;  
                    ws.Column(i + 1).AutoFit();
                    ws.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //Write it back to the client  
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=Examcentre_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");

                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }
       // update1.Update();
    }
    private void CreateStudentManagerInstance()
    {
        if (_studentMgr == null)
            _studentMgr = new StudentManager();
    }


}
