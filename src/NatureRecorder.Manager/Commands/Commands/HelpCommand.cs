using System;
using System.IO;
using System.Reflection;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Manager.Commands.Base;

namespace NatureRecorder.Manager.Commands.Commands
{
    public class HelpCommand : CommandBase
    {
        public HelpCommand()
        {
            Type = CommandType.help;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredContext = CommandContext.All;
        }

        public override void Run(NatureRecorderFactory factory, CommandContext context, string[] arguments)
        {
            if (ValidForContext(context) && ArgumentCountCorrect(arguments))
            {
                string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(HelpCommand)).Location);
                string helpFile = Path.Combine(path, "Content", "Help.txt");

                using (StreamReader reader = new StreamReader(helpFile))
                {
                    Console.WriteLine();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Console.WriteLine(line);
                    }
                }
            }
        }
    }
}
