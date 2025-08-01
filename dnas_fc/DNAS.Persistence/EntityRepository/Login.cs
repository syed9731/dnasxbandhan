using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DAO.DbHelperModels.CheckPredefinePassword;
using DNAS.Domain.DAO.DbHelperModels.FetchUserData;
using DNAS.Domain.DAO.DbHelperModels.GetMasterData;
using DNAS.Domain.DAO.DbHelperModels.GetRecoverPassword;
using DNAS.Domain.DAO.DbHelperModels.VerifyRecoverPasswordUser;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class Login(ICustomLogger logger, IHttpContextAccessor haccess, IDapperFactory iDapperFactory) : ILogin
    {
        public readonly ICustomLogger _logger = logger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<UserMasterResponse>> getMasterData(object inparam)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                ProcUserLoginOutput DBResponse = await _iDapperFactory.ExecuteSpDapperAsync<UserMasterResponse, ProcUserLoginOutput>
                    (SpName: OraStoredProcedureNames.ProcUserLogin, inparam);
                Response.Data = DBResponse.UserMaster;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during getMasterData------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Response;
        }
        public async Task<CommonResponse<UserMasterResponse>> getUserDataByUserId(object inparam)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                ProcUserLoginOutput DBResponse = await _iDapperFactory.ExecuteSpDapperAsync<UserMasterResponse, ProcUserLoginOutput>
                    (SpName: OraStoredProcedureNames.ProcUserDataByUserId, inparam);
                Response.Data = DBResponse.UserMaster;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during getMasterData------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Response;
        }
        public async Task<CommonResponse<RecoverPasswordResponse>> getRecoverPassword(object inparam)
        {
            CommonResponse<RecoverPasswordResponse> Response = new();
            try
            {
                ProcFetchRecoverPasswordOutput DbResponse = await _iDapperFactory.ExecuteSpDapperAsync<RecoverPasswordResponse, ProcFetchRecoverPasswordOutput>
                    (SpName: OraStoredProcedureNames.ProcFetchRecoverPassword, inparam);
                Response.Data = DbResponse.RecoverPassword;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during getRecoverPassword------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Response;
        }
        public async Task<CommonResponse<UserMasterResponse>> checkPredefinePassword(object inparam)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                ProcCheckPredefinedPasswordOutput DbResponse = await _iDapperFactory.ExecuteSpDapperAsync<UserMasterResponse, ProcCheckPredefinedPasswordOutput>(OraStoredProcedureNames.ProcCheckPredefinedPassword, inparam);
                Response.Data= DbResponse.UserMaster;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during checkPredefinePassword------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Response;
        }
        public async Task<CommonResponse<ChangePasswordModel>> VerifyRecoverPasswordUser(object inparam)
        {
            CommonResponse<ChangePasswordModel> Response = new();
            try
            {
                ProcVerifyRecoverPasswordUserOutput DbResponse = await _iDapperFactory.ExecuteSpDapperAsync<ChangePasswordModel, ProcVerifyRecoverPasswordUserOutput>
                    (SpName: OraStoredProcedureNames.ProcVerifyRecoverPasswordUser, inparam);
                Response.Data= DbResponse.ChangePassword;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during VerifyRecoverPasswordUser------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Response;
        }
        public async Task<UserMasterModel> FetchUserData(object inparam)
        {
            FetchUserDataOutput Response = new();
            try
            {
                Response = await _iDapperFactory.ExecuteSpDapperAsync<UserMasterModel, FetchUserDataOutput>
                    (SpName: OraStoredProcedureNames.ProcFetchUserData, inparam);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during FetchUserData------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Response.UserMaster;
        }
        public async Task<UserTrackingModel> CheckUsertracking(object inparam)
        {
            try
            {
                UserTrackingData Response = await _iDapperFactory.ExecuteSpDapperAsync<UserTrackingModel, UserTrackingData>
                    (SpName: OraStoredProcedureNames.ProcFetchUserTracking, inparam);
                return Response.UserTracking;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during CheckUsertracking------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new();
            }

        }
        public async Task<UserTrackingModel> Usertracking(object inparam)
        {
            try
            {
                UserTrackingData Response = await _iDapperFactory.ExecuteSpDapperAsync<UserTrackingModel, UserTrackingData>
                    (SpName: OraStoredProcedureNames.ProcUserTracking, inparam);
                return Response.UserTracking;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during CheckUsertracking------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new();
            }

        }
        public async Task<CommonResponse<UserMasterResponse>> CheckUserExists(object inparam)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                ProcUserLoginOutput DBResponse = await _iDapperFactory.ExecuteSpDapperAsync<UserMasterResponse, ProcUserLoginOutput>
                    (SpName: OraStoredProcedureNames.ProcCheckUserExists, inparam);
                Response.Data = DBResponse.UserMaster;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during getMasterData------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Response;
        }
    }
}
