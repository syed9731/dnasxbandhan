using System.ComponentModel.DataAnnotations;


namespace DNAS.Domian.DTO.FYI
{
    public class FyiModel
    {
        public long FYIId { get; set; }

        [Required(ErrorMessage = "Note Id is required.")]
        public long NoteId { get; set; }

        [Required(ErrorMessage = "Sender User Id is required.")]
        public int WhoTagged { get; set; }

        [Required(ErrorMessage = "Receiver User Id is required.")]
        public int ToWhome { get; set; }

        public DateTime TaggedTime { get; set; } = DateTime.Now;
    }
}
