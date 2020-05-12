using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NatureRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Species
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public Category Category { get; set; }
    }
}
