using System;

namespace NatureRecorder.Manager.Entities
{
    public class Operation
    {
        public const string DateFormat = "yyyy-MM-dd";

        public bool Valid { get; set; }
        public OperationType Type { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FileName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
