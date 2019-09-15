using BaseLib.Exceptions;
using BaseLib.OraDataBase;
using BaseLib.OraDataBase.StoredProcedures;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BaseLib.Common
{
    public class FileData
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public OracleCommand FileCmd { get; set; }
        public string FilePath { get; set; }
        public HttpResponseMessage DownloadFile()
        {
            //var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(FileBytes) //new ByteArrayContent(stream.ToArray())
            };

            result.Content.Headers.ContentDisposition =
              new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
              {
                  FileName = HttpUtility.UrlEncode(FileName)

              };
         
            result.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");


            if (string.IsNullOrEmpty(ContentType))
            {
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(ContentType);
            }
            return result;
        }

        public Task<HttpResponseMessage> DownloadFile1()
        {
            //var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(FileBytes) //new ByteArrayContent(stream.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = FileName
                };

            result.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            if (string.IsNullOrEmpty(ContentType))
            {
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(ContentType);
            }
            return Task.FromResult(result);
        }
        


        /*   public void DownloadFile()
   {
       try
       {
           if (FileBytes == null || FileBytes.Length == 0)
           {
               if (FileCmd == null && String.IsNullOrEmpty(FilePath))
                   throw new Exception("DownloadFile Error: No FileBytes and No Oracle Command");

               //if (FileCmd != null && !String.IsNullOrEmpty(FilePath))
               //    throw new Exception("DownloadFile Error: No FilePath and No Oracle Command");

               HttpContext.Current.ApplicationInstance.CompleteRequest();
               HttpContext.Current.Response.ClearContent();
               HttpContext.Current.Response.ClearHeaders();

               var ckRsExportDone = new HttpCookie("rsDownloadDone");
               var now = DateTime.Now;
               ckRsExportDone.Value = "Downloading";
               ckRsExportDone.Expires = now.AddMinutes(5);
               HttpContext.Current.Response.Cookies.Add(ckRsExportDone);

               //response.setHeader(, request.getHeader("Origin"));
               HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
               HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "Content-Disposition");


               HttpContext.Current.Response.ContentType = string.IsNullOrEmpty(FileType) ? CommonFunctions.getFilesContentType(FileName) : FileType;
               HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + FileName + "\"");


               string error = string.Empty;

               if (FileCmd != null)
                   new OracleDb<PKG_ERROR_LOGS>().SequaltialFlushingReport(FileCmd, out error);

               else if (FilePath != null)

               {
                   HttpContext.Current.Response.AddHeader("Transfer-Encoding", "identity");

                   HttpContext.Current.Response.TransmitFile(FilePath);
                   HttpContext.Current.Response.Flush();
               }


               if (!string.IsNullOrEmpty(error))
                   throw new Exception(error);

               HttpContext.Current.ApplicationInstance.CompleteRequest();
           }
           else
           {
               HttpContext.Current.ApplicationInstance.CompleteRequest();
               HttpContext.Current.Response.ClearContent();
               HttpContext.Current.Response.ClearHeaders();

               HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
               HttpContext.Current.Response.AddHeader("Access-Control-Expose-Headers", "Content-Disposition");

               HttpContext.Current.Response.ContentType = FileType;
               HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=\"" + FileName + "\"");
               HttpContext.Current.Response.BinaryWrite(FileBytes);
               HttpContext.Current.Response.Flush();
               HttpContext.Current.ApplicationInstance.CompleteRequest();
           }
       }
       catch (Exception e)
       {
           HttpContext.Current.Response.Close();
           if (e.InnerException is UserExceptions) throw new UserExceptions(e.InnerException.Message);
           throw;
       }
   } */

    }
}

