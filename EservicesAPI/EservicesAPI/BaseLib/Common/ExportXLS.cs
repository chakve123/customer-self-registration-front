using BaseLib.Attributes;
using BaseLib.ExtensionMethods;
using BaseLib.OraDataBase;
using BaseLib.OraDataBase.StoredProcedures;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BaseLib.Common
{
    public class ExportXLS
    {
        public static void ExportNew<TDataSource>(object data, Dictionary<string, string> fields, string pageID = null) where TDataSource : class, new()
        {
            var hssfworkbook = new HSSFWorkbook();
            ISheet gridSheet = CreateSheet("Grid", hssfworkbook);
            ICellStyle dateStyle = GetDateStyle(hssfworkbook);
            ICellStyle cellStyle = GetCellStyle(hssfworkbook);

            SetHeader(fields, gridSheet, hssfworkbook);
            int i = 1;

            var list = (data as Dictionary<string, object>);
            var listRows = list["Rows"] as List<object>;
            var listFields = list["Fields"] as List<string>;
            var dsObject = new TDataSource();

            foreach (List<object> item in listRows)
            {
                IRow row = gridSheet.CreateRow(i);

                int j = 0;
                foreach (string field in fields.Keys)
                {
                    var prop = typeof(TDataSource).GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    object[] attr = prop.GetCustomAttributes(false);

                    int index = listFields.IndexOf(field);

                    if (index >= 0)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.CellStyle = cellStyle;

                        if (item[index] == null && prop.GetValue(dsObject, null) == null)
                            cell.SetCellValue("");
                        else if (prop.PropertyType.IsNumericType())
                        {
                            cell.SetCellValue(item[index].ToString().ToNumber<double>());
                        }
                        else if (attr.Length > 0)
                        {
                            if (attr[0] is DateTimeFormat)
                            {
                                ICell dateCell = row.CreateCell(j);
                                dateCell.SetCellValue(DateTime.Parse(item[index].ToString()));
                                dateCell.CellStyle = dateStyle;
                            }
                            else if (attr[0] is NumberFormat)
                                cell.SetCellValue(double.Parse(item[index].ToString()));
                            else
                                cell.SetCellValue(item[index].ToString());
                        }
                        else
                            cell.SetCellValue(item[index].ToString());

                        j++;
                    }
                }

                //if (!string.IsNullOrEmpty(pageID) && i % 1000 == 0)
                //    DataProviderManager<PKG_ERROR_LOGS>.Provider.Progress(pageID, i, null);

                i++;
            }

            SetCellSize<TDataSource>(fields, gridSheet, hssfworkbook);

            //using (var fileStream = File.Create(ConfigurationManager.AppSettings.GetValues("GlobalTempFolder")[0] + pageID + ".xls"))
            //{
            //    hssfworkbook.Write(fileStream);
            //}

            const string filename = "Export.xls";
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            var now = DateTime.Now;
            var cookieRsDownloading = new HttpCookie("rsDownloading")
            {
                Value = "Downloading",
                Expires = now.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Add(cookieRsDownloading);

            HttpContext.Current.Response.Clear();

            var file = new MemoryStream();
            hssfworkbook.Write(file);

            file.Position = 0;

            CommonFunctions.SequantialFlushing(file);
            file.Close();
            file.Dispose();
            HttpContext.Current.Response.End();
        }

        public static void ExportWaybill<TDataSource>(object data, Dictionary<string, string> fields, string pageID = null) where TDataSource : class, new()
        {
            var hssfworkbook = new HSSFWorkbook();

            ISheet gridSheet = hssfworkbook.CreateSheet("Grid");
            ISheet invoiceSheet = hssfworkbook.CreateSheet("ანგარიშ-ფაქტურა");
            invoiceSheet.TabColorIndex = HSSFColor.RED.index;

            gridSheet.DefaultColumnWidth = 18;
            gridSheet.PrintSetup.Landscape = true;
            gridSheet.PrintSetup.PaperSize = (short)PaperSize.A4;
            gridSheet.FitToPage = true;
            gridSheet.PrintSetup.FitHeight = 0;
            gridSheet.PrintSetup.FitWidth = 1;

            invoiceSheet.DefaultColumnWidth = 18;
            invoiceSheet.PrintSetup.Landscape = true;
            invoiceSheet.PrintSetup.PaperSize = (short)PaperSize.A4;
            invoiceSheet.FitToPage = true;
            invoiceSheet.PrintSetup.FitHeight = 0;
            invoiceSheet.PrintSetup.FitWidth = 1;


            IFont font = hssfworkbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.FontName = "Sylfaen";
            font.Boldweight = (short)FontBoldWeight.BOLD;

            ICellStyle headerStyle = hssfworkbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.CENTER;
            headerStyle.SetFont(font);
            headerStyle.WrapText = true;

            ICellStyle dateStyle = hssfworkbook.CreateCellStyle();
            dateStyle.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("dd/MM/yyyy HH:mm:ss");
            dateStyle.BorderLeft = BorderStyle.THIN;
            dateStyle.BorderRight = BorderStyle.THIN;
            dateStyle.BorderTop = BorderStyle.THIN;
            dateStyle.BorderBottom = BorderStyle.THIN;

            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.BorderLeft = BorderStyle.THIN;
            cellStyle.BorderRight = BorderStyle.THIN;
            cellStyle.BorderTop = BorderStyle.THIN;
            cellStyle.BorderBottom = BorderStyle.THIN;

            ICellStyle hlinkStyle = hssfworkbook.CreateCellStyle();
            IFont hlinkFont = hssfworkbook.CreateFont();
            hlinkFont.Underline = (byte)FontUnderlineType.SINGLE;
            hlinkFont.Color = HSSFColor.RED.index;
            hlinkFont.FontHeight = 300;
            hlinkStyle.SetFont(hlinkFont);
            hlinkStyle.Alignment = HorizontalAlignment.CENTER;
            hlinkStyle.VerticalAlignment = VerticalAlignment.CENTER;

            IRow row = gridSheet.CreateRow(0);
            IRow invoiceRow = invoiceSheet.CreateRow(0);

            int i = 0;
            int j = 0;
            foreach (string key in fields.Keys)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(fields[key]);
                cell.CellStyle = headerStyle;

                if (key == "WAYBILL_NUMBER" || key == "DELIVERY_DATE")
                {
                    ICell invoiceCell = invoiceRow.CreateCell(j);
                    invoiceCell.SetCellValue(fields[key]);
                    invoiceCell.CellStyle = headerStyle;
                    j++;
                }

                i++;
            }

            i = 2;
            int k = 1;
            object[] attr;
            var list = (data as Dictionary<string, object>);
            var listRows = list["Rows"] as List<object>;
            var listFields = list["Fields"] as List<string>;
            var dsObject = new TDataSource();
            foreach (List<object> item in listRows)
            {
                row = gridSheet.CreateRow(i);

                j = 0;
                foreach (string field in fields.Keys)
                {
                    var prop = typeof(TDataSource).GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    attr = prop.GetCustomAttributes(false);

                    int index = listFields.IndexOf(field);

                    if (index >= 0)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.CellStyle = cellStyle;

                        if (item[index] == null && prop.GetValue(dsObject, null) == null)
                            cell.SetCellValue("");
                        else if (prop.PropertyType.IsNumericType())
                        {
                            cell.SetCellValue(item[index].ToString().ToNumber<double>());
                        }
                        else if (attr.Length > 0)
                        {
                            if (attr[0] is DateTimeFormat)
                            {
                                ICell dateCell = row.CreateCell(j);
                                dateCell.SetCellValue(DateTime.Parse(item[index].ToString()));
                                dateCell.CellStyle = dateStyle;
                            }
                            else if (attr[0] is NumberFormat)
                                cell.SetCellValue(double.Parse(item[index].ToString()));
                            else
                                cell.SetCellValue(item[index].ToString());
                        }
                        else
                            cell.SetCellValue(item[index].ToString());

                        j++;
                    }
                }

                var indexWaybill = listFields.IndexOf("WAYBILL_NUMBER");
                var indexDate = listFields.IndexOf("DELIVERY_DATE");

                if (item[indexWaybill] != null && item[indexDate] != null)
                {
                    invoiceRow = invoiceSheet.CreateRow(k);
                    ICell invoiceCell = invoiceRow.CreateCell(0);
                    invoiceCell.CellStyle = cellStyle;
                    invoiceCell.SetCellValue(item[indexWaybill].ToString());

                    invoiceCell = invoiceRow.CreateCell(1);
                    invoiceCell.CellStyle = cellStyle;
                    object dt = string.IsNullOrEmpty(item[indexDate].ToString()) ? typeof(TDataSource).GetProperty("DELIVERY_DATE", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).GetValue(dsObject, null) : item[indexDate];
                    invoiceCell.SetCellValue(DateTime.Parse(dt.ToString()).ToString("dd.MM.yyyy"));

                    k++;
                }

                //if (!string.IsNullOrEmpty(pageID) && i % 1000 == 0)
                //    DataProviderManager<PKG_COMMON>.Provider.Progress(pageID, i, null);

                i++;
            }

            i = 0;
            foreach (string field in fields.Keys)
            {
                gridSheet.AutoSizeColumn(i);
                PropertyInfo p = typeof(TDataSource).GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                attr = p.GetCustomAttributes(typeof(DateTimeFormat), false);
                if (gridSheet.GetColumnWidth(i) > 4000 && attr.Length == 0 && p.PropertyType != typeof(decimal))
                    gridSheet.SetColumnWidth(i, 4000);
                else if (attr.Length > 0)
                    gridSheet.SetColumnWidth(i, 4500);

                i++;
            }



            //const string filename = "Export.xls";
            //HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            //var now = DateTime.Now;
            //var cookieRsDownloading = new HttpCookie("rsDownloading")
            //{
            //    Value = "Downloading",
            //    Expires = now.AddDays(-1)
            //};
            //HttpContext.Current.Response.Cookies.Add(cookieRsDownloading);

            //HttpContext.Current.Response.Clear();
            using (var fileStream = File.Create(ConfigurationManager.AppSettings.GetValues("GlobalTempFolder")[0] + Guid.NewGuid() + ".xls"))
            {
                hssfworkbook.Write(fileStream);
            }

            //var file = new MemoryStream();
            //hssfworkbook.Write(file);

            //file.Position = 0;

            //CommonFunctions.SequantialFlushing(file);

            //file.Close();
            //file.Dispose();
            //HttpContext.Current.Response.End();
        }

        public static ISheet CreateSheet(string sheetName, HSSFWorkbook workbook)
        {
            ISheet gridSheet = workbook.CreateSheet(sheetName);
            gridSheet.PrintSetup.Landscape = true;
            gridSheet.PrintSetup.PaperSize = (short)PaperSize.A4;
            gridSheet.FitToPage = true;
            gridSheet.PrintSetup.FitHeight = 0;
            gridSheet.PrintSetup.FitWidth = 1;

            return gridSheet;
        }

        public static IFont GetFont(string fontName, HSSFWorkbook workbook)
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.FontName = fontName;
            font.Boldweight = (short)FontBoldWeight.BOLD;

            return font;
        }

        public static ICellStyle GetHeaderStyle(IFont font, HSSFWorkbook workbook)
        {
            ICellStyle headerStyle = workbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.CENTER;
            headerStyle.SetFont(font);
            headerStyle.WrapText = true;

            return headerStyle;
        }

        public static ICellStyle GetDateStyle(HSSFWorkbook workbook)
        {
            ICellStyle dateStyle = workbook.CreateCellStyle();
            dateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd/MM/yyyy HH:mm:ss");
            dateStyle.BorderLeft = BorderStyle.THIN;
            dateStyle.BorderRight = BorderStyle.THIN;
            dateStyle.BorderTop = BorderStyle.THIN;
            dateStyle.BorderBottom = BorderStyle.THIN;

            return dateStyle;
        }

        public static ICellStyle GetCellStyle(HSSFWorkbook workbook)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.BorderLeft = BorderStyle.THIN;
            cellStyle.BorderRight = BorderStyle.THIN;
            cellStyle.BorderTop = BorderStyle.THIN;
            cellStyle.BorderBottom = BorderStyle.THIN;

            return cellStyle;
        }

        public static void SetCellSize<TDataSource>(Dictionary<string, string> fields, ISheet gridSheet, HSSFWorkbook workbook) where TDataSource : class, new()
        {
            int i = 0;
            foreach (string field in fields.Keys)
            {
                gridSheet.AutoSizeColumn(i);
                PropertyInfo p = typeof(TDataSource).GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (p == null)
                    continue;
                object[] attr = p.GetCustomAttributes(typeof(DateTimeFormat), false);
                if (gridSheet.GetColumnWidth(i) > 4000 && attr.Length == 0 && p.PropertyType != typeof(decimal))
                    gridSheet.SetColumnWidth(i, 4000);
                else if (attr.Length > 0)
                    gridSheet.SetColumnWidth(i, 4700);

                i++;
            }
        }

        public static void SetHeader(Dictionary<string, string> fields, ISheet gridSheet, HSSFWorkbook workbook)
        {
            IFont font = GetFont("Sylfaen", workbook);
            ICellStyle headerStyle = GetHeaderStyle(font, workbook);
            IRow row = gridSheet.CreateRow(0);

            int i = 0;
            foreach (string key in fields.Keys)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(fields[key]);
                cell.CellStyle = headerStyle;

                i++;
            }
        }

    }
}
