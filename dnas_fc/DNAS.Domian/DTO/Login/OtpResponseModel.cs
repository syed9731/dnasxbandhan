namespace DNAS.Domain.DTO.Login
{
    public class OtpResponseModel
    {
        public bool OTPSent { get; set; }
        public string OTP { get; set; } = string.Empty;
        public string OTPError { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
    }
}
