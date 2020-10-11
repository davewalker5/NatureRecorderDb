using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class StatusScheme
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
