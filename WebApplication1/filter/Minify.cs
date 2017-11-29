using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.filter
{
    public class Minify : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //You can check if the content type is CSS/JS here and prevent the filter running on HTML pages 

            filterContext.HttpContext.Response.Filter = new MinifyFilter(filterContext.HttpContext.Response.Filter);

            base.OnActionExecuting(filterContext);
        }
    }

    public class MinifyFilter : MemoryStream
    {
        private StringBuilder outputString = new StringBuilder();
        private Stream outputStream = null;

        public MinifyFilter(Stream outputStream)
        {
            this.outputStream = outputStream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            outputString.Append(Encoding.UTF8.GetString(buffer));
        }

        public override void Close()
        {
            //Call the minifier here, your data is in outputString
            string result = outputString.ToString();

            result = Regex.Replace(result, @"\s+", " ");
            result = Regex.Replace(result, @"\s*\n\s*", "\n");
            result = Regex.Replace(result, @"\s*\>\s*\<\s*", "><");
            result = Regex.Replace(result, @"<!--(.*?)-->", "");   //Remove comments
            result = result.Replace(Environment.NewLine, string.Empty);

            byte[] rawResult = Encoding.UTF8.GetBytes(result);
            outputStream.Write(rawResult, 0, rawResult.Length);

            base.Close();
            outputStream.Close();
        }
    }
}