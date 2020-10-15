using System;
using System.IO;
using NatureRecorder.BusinessLogic.Factory;
using NatureRecorder.Interpreter.Base;
using NatureRecorder.Interpreter.Entities;

namespace NatureRecorder.Tests.Wrappers
{
    /// <summary>
    /// Wrapper to allow direct tests of the CommandBase protected methods
    /// </summary>
    public class CommandBaseWrapper : CommandBase
    {
        private readonly CommandContext _context;

        public CommandBaseWrapper(NatureRecorderFactory factory)
        {
            _context = new CommandContext
            {
                Factory = factory
            };
        }

        public override void Run(CommandContext context)
        {
        }

        /// <summary>
        /// Wrapper for the get "entity type" methods
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetEntityId(EntityType type, string name)
        {
            int? id;

            _context.Arguments = new string[] { name };

            switch (type)
            {
                case EntityType.Category:
                    id = GetCategory(_context, 0);
                    break;
                case EntityType.Species:
                    id = GetSpecies(_context, 0);
                    break;
                case EntityType.Location:
                    id = GetLocation(_context, 0);
                    break;
                default:
                    id = null;
                    break;
            }

            return id;
        }

        /// <summary>
        /// Wrapper for the "get date from string" method
        /// </summary>
        /// <param name="representation"></param>
        /// <returns></returns>
        public DateTime? GetDate(string representation)
        {
            DateTime? result;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter output = new StreamWriter(stream))
                {
                    result = GetDateFromArgument(representation, output);
                }
            }

            return result;
        }
    }
}
