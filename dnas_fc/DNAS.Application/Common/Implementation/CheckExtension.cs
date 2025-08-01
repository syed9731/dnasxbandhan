using DNAS.Application.Common.Interface;


using System.Globalization;
using System.Text.RegularExpressions;

namespace DNAS.Application.Common.Implementation
{
    internal class CheckExtension : ICheckExtension
    {
        private readonly string dateformat = "dd/MM/yyyy";
        public string AddSpace(string valuename)
        {
            return Regex.Replace(valuename, "[A-Z]", " $0", RegexOptions.None, TimeSpan.FromSeconds(5));
        }

        public bool CheckFileExtension(string extension)
        {
            switch (extension.ToUpper())
            {
                case ".JPG":
                case ".JPEG":
                case ".PNG":
                case ".PDF":
                case ".DOC":
                case ".DOCX":
                case ".XLS":
                case ".XLSX":
                case ".ZIP":
                    return true;
                default:
                    return false;
            }
        }

        public (string, string) DateRangeToDate(string type,string startDate,string endDate)
        {
            DateTime today = DateTime.Now;            
            if (type == "last-7-days")
            {
                startDate = today.AddDays(-7).ToString(dateformat, CultureInfo.InvariantCulture);
                endDate = today.ToString(dateformat, CultureInfo.InvariantCulture);
            }
            if (type == "last-30-days")
            {
                startDate = today.AddDays(-30).ToString(dateformat, CultureInfo.InvariantCulture);
                endDate = today.ToString(dateformat, CultureInfo.InvariantCulture);

            }
            
            return (startDate, endDate);
        }
    }
}
