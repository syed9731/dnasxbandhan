namespace DNAS.Application.Common.Interface
{
    public interface ICheckExtension
    {
        bool CheckFileExtension(string extension);
        string AddSpace(string valuename);

        (string, string) DateRangeToDate(string type, string startDate, string endDate);
    }
}
