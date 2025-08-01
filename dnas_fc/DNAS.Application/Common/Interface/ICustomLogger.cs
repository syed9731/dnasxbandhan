namespace DNAS.Application.Common.Interface
{
    public interface ICustomLogger
    {
        void LogwriteInfo(string message, string filename);
        void LogwriteError(string message, string filename);
        void LogwriteWarning(string message, string filename);
    }
}
