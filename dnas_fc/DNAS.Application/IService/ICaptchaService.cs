using DNAS.Application.Common.Implementation;

namespace DNAS.Application.IService
{
    public interface ICaptchaService
    {
        string GenerateRandomString(int length, CharType type);
        byte[] CreateCAPTCHAImage(string text, string backgroundColor, string foreColor, int width = 120, int height = 40, string fontFamily = "Arial");
    }
}
