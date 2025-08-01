namespace DNAS.Domain.DAO.DbHelperModels.Login;

public class ProcSaveUserTrackingInput
{
    public int @UserId { get; set; } = 0;
    public bool @IsActive { get; set; } = false;
    public DateTime @LastLoginTime { get; set; }
    public string @SessionId { get; set; } = string.Empty;
}
