using DNAS.Application.Common.Interface;

namespace DNAS.Application.Common.Implementation
{
    internal class FileValidation:IFileValidation
    {
        public bool CheckMimeType(string mimeType)
        {

            switch (mimeType)
            {
                case "image/jpeg":          //for .jpeg file
                case "image/png":           //for .png file
                case "image/jpg":           //for .jpg file
                case "application/pdf":     //for .pdf file
                case "application/msword":  //for .doc file
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":     //for .docx file
                case "application/vnd.ms-excel":        //for .xls file
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":       //for .xlsx file
                case "application/x-zip-compressed":    //for .zip file
                    return true;
                default: 
                    return false;
            }            
        }
    }
}
