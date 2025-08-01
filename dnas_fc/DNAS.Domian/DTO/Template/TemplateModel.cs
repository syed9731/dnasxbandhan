using System.Globalization;

namespace DNAS.Domian.DTO.Template
{
    public class TemplateModelData
    {
        public FilterTemplateModel FilterTemplates { get; set; } = new();
        public IEnumerable<TemplateModel> TemplateList { get; set; } = [];
    }
    public class TemplateModel
    {
        public string TemplateId { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string TemplateBody { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPublish { get; set; } = false;
    }
    public class FilterTemplateModel
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
    }
}
