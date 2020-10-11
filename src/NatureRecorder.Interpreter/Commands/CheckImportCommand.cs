using System.Diagnostics.CodeAnalysis;
using NatureRecorder.Entities.Exceptions;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Interpreter.Commands
{
    public class CheckImportCommand : DataExchangeCommandBase
    {
        public CheckImportCommand()
        {
            Type = CommandType.check;
            MinimumArguments = 2;
            MaximiumArguments = 2;
            RequiredMode = CommandMode.All;
        }

        public override void Run(CommandContext context)
        {
            if (ValidForCommandMode(context) && ArgumentCountCorrect(context))
            {
                // The first argument is the import type
                DataExchangeType type = GetDataExchangeType(context);
                switch (type)
                {
                    case DataExchangeType.Sightings:
                        CheckSightingsImport(context);
                        break;
                    case DataExchangeType.Status:
                        CheckStatusImport(context);
                        break;
                    default:
                        string message = "Cannot check data for unknown import type";
                        throw new UnknownDataExchangeTypeException(message);
                }
            }
        }

        /// <summary>
        /// Check a sightings import file for new lookups
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void CheckSightingsImport(CommandContext context)
        {
            context.Factory.SightingsImport.DetectNewLookups(context.Arguments[1]);
            context.Factory.SightingsImport.WriteNewLookupsToStream(context.Output);
            context.Output.Flush();
        }

        /// <summary>
        /// Check a conservation status import file for new lookups
        /// </summary>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        private void CheckStatusImport(CommandContext context)
        {
            context.Factory.SpeciesStatusImport.DetectNewLookups(context.Arguments[1]);
            context.Factory.SpeciesStatusImport.WriteNewLookupsToStream(context.Output);
            context.Output.Flush();
        }
    }
}
