using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DTO.Note.Download
{
    public class FilesModel
    {
        public string FileName { get; set; }
        public byte[] fileByte { get; set; }

    }
}
