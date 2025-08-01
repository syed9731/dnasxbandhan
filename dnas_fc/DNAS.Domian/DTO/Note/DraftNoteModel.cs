using DNAS.Domian.DTO.Login;

namespace DNAS.Domian.DTO.Note
{
    public class DraftNoteModel
    {
        public NoteModel noteModel { get; set; }= new NoteModel();
        public IEnumerable<UserMasterModel> userMaster {  get; set; }=new List<UserMasterModel>();
    }
    
}
