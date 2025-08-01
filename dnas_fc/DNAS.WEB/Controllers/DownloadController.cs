using DNAS.Application.Common.Interface;
using DNAS.Application.Features.Note.Download;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DNAS.WEB.Controllers
{
    public class DownloadController(ISender iSender, IEncryption encryption) : Controller
    {
        private readonly ISender _iSender = iSender;
        private readonly IEncryption _encryption = encryption;
        [HttpGet]
        public async Task<IActionResult> file(string AttatchedId)
        {
           AttatchedId=_encryption.AesDecrypt(AttatchedId);
           var Response = await _iSender.Send(new FilesCommand(AttatchedId));
            if(string.IsNullOrWhiteSpace(Response.Data.FileName))
            {
                return NotFound();
            }
            var contentType = "application/octet-stream";
            // Return the file as a download
            return File(Response.Data.fileByte, contentType, Response.Data.FileName);
        }
    }
}
