using DNAS.Domain.DTO.Note;

namespace DNAS.Application.IEntityRepository
{
    public interface IFetch
    {
       ValueTask<SendBackNoteDto> FetchSendBackNoteByNoteId(long noteId);
    }
}
