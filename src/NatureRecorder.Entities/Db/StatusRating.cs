using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class StatusRating
    {
        [Key]
        public int Id { get; set; }
        public int StatusSchemeId { get; set; }
        public string Name { get; set; }

        public StatusScheme Scheme { get; set; }
    }
}
