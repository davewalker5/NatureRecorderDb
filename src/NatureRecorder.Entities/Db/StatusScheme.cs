using System.ComponentModel.DataAnnotations;

namespace NatureRecorder.Entities.Db
{
    public class StatusScheme
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
