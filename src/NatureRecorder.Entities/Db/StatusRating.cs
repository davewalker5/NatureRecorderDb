using System.ComponentModel.DataAnnotations;

namespace NatureRecorder.Entities.Db
{
    public class StatusRating
    {
        [Key]
        public int Id { get; set; }
        public int StatusSchemeId { get; set; }
        public string Name { get; set; }

        public StatusScheme Scheme { get; set; }
    }
}
