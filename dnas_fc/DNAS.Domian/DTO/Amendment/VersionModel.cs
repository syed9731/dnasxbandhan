namespace DNAS.Domain.DTO.Amendment
{
    public class VersionModel
    {
        public AmendVersion version { get; set; }= new();
    }
    public class AmendVersion
    {
        public int MajorRevision { get; set; } = 0;
        public int MinorRevision { get; set; } = 0;
    }
}
