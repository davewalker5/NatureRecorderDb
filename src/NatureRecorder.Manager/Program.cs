using System;
using NatureRecorder.Manager.Entities;
using NatureRecorder.Manager.Logic;

namespace NatureRecorder.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Nature Recorder Database Management {version}");

            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                new CommandRunner().Run(op);
            }
            else
            {
                string executable = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("Usage:");
                Console.WriteLine($"[1] {executable} add username password");
                Console.WriteLine($"[2] {executable} setpassword username password");
                Console.WriteLine($"[3] {executable} delete username");
                Console.WriteLine($"[4] {executable} check csv_file_path");
                Console.WriteLine($"[5] {executable} import csv_file_path");
                Console.WriteLine($"[6] {executable} export csv_file_path");
                Console.WriteLine($"[7] {executable} summary yyyy-mm-dd");
                Console.WriteLine($"[8] {executable} update");
            }
        }
    }
}
