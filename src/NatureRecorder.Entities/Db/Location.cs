using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
