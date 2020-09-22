using System.IO;
using System.Reflection;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class HelpCommand : CommandBase
    {
        public HelpCommand()
        {
            Type = CommandType.help;
            MinimumArguments = 0;
            MaximiumArguments = 0;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(HelpCommand)).Location);
                string helpFile = Path.Combine(path, "Content", "help.txt");

                using (StreamReader reader = new StreamReader(helpFile))
                {
                    context.Output.WriteLine();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        context.Output.WriteLine(line);
                    }
                    context.Output.Flush();
                }
            }
        }
    }
}
