
using ScratchSharp.Project;
using System;
using System.Collections.Generic;

namespace MonoScratch.Compiler {

    public static class ProcedureBlocks {

        public enum ProcedureArgumentType {
            VALUE,
            BOOLEAN
        }

        public class ProcedureArgument {
            public readonly ProcedureArgumentType ArgType;
            public readonly string Name;
            public readonly string ID;

            public readonly string CodeName;

            public ProcedureArgument(SourceGeneratorContext ctx, BlockMutationArgument arg, ProcedureArgumentType type) {
                ArgType = type;
                ID = arg.ID;
                Name = arg.Name ?? throw new SystemException($"Expected name on block argument '{ID}'.");
                CodeName = ctx.GetNextSymbol(Name);
            }
        }

        public class DefinitionBlock : ItmScratchBlock {

            public readonly List<ProcedureArgument> Arguments;
            public readonly string Proccode;
            public readonly string MethodName;

            public DefinitionBlock(SourceGeneratorContext ctx, ScratchBlock block, List<ProcedureArgument> arguments, string proccode) : base(block) {
                Proccode = proccode;
                Arguments = arguments;
                MethodName = ctx.GetNextSymbol(proccode.Replace("%s", "").Replace("%b", ""));
            }

            public static DefinitionBlock Create(SourceGeneratorContext ctx, ScratchBlock block) {
                BlockBlockInput procedurePrototypeInput = ItmScratchBlockInput.From(block.Inputs["custom_block"]) as BlockBlockInput
                    ?? throw new SystemException("custom_block input must be a block!");

                ScratchBlock? procedurePrototype;
                if (!(ctx.CurrentTarget?.Target.Blocks.TryGetValue(procedurePrototypeInput.ID!, out procedurePrototype) ?? false))
                    throw new SystemException($"Couldn't find custom block prototype with id {procedurePrototypeInput.ID}.");

                if (procedurePrototype.Opcode != "procedures_prototype")
                    throw new SystemException($"Expected procedures_prototype at block '{procedurePrototype.ID}'.");

                BlockMutation mutation = procedurePrototype.Mutation
                    ?? throw new SystemException($"Expected mutation on '{procedurePrototype.ID}'.");

                List<BlockMutationArgument> mutationArguments = mutation.Arguments
                    ?? throw new SystemException($"Expected mutation arguments on '{procedurePrototype.ID}'.");

                List<ProcedureArgumentType> argumentsTypes = new List<ProcedureArgumentType>();
                string proc = mutation.ProcCode;
                for (int i = 0; i < proc.Length; i++) {
                    if (proc[i] == '\\' && i != proc.Length - 1) {
                        if (proc[i + 1] == '%') {
                            ++i;
                        }
                    } else if (proc[i] == '%') {
                        if (i == proc.Length - 1)
                            throw new SystemException($"Invalid proccode '{proc}' in '{procedurePrototype.ID}'.");
                        ProcedureArgumentType type;
                        switch (proc[i + 1]) {
                            case 'n':
                            case 's':
                                type = ProcedureArgumentType.VALUE;
                                break;
                            case 'b':
                                type = ProcedureArgumentType.BOOLEAN;
                                break;
                            default:
                                throw new SystemException($"Invalid proccode '{proc}' in '{procedurePrototype.ID}'. Unknown argument type '{proc[i + 1]}'.");
                        }
                        argumentsTypes.Add(type);
                    }
                }

                if (argumentsTypes.Count != mutationArguments.Count)
                    throw new SystemException($"Invalid proccode '{proc}' in '{procedurePrototype.ID}'. Expected {mutationArguments.Count} arguments. Found {argumentsTypes.Count}.");

                List<ProcedureArgument> arguments = new List<ProcedureArgument>(argumentsTypes.Count);
                for (int i = 0; i < argumentsTypes.Count; i++) {
                    arguments.Add(new ProcedureArgument(ctx, mutationArguments[i], argumentsTypes[i]));
                }

                return new DefinitionBlock(ctx, block, arguments, proc);
            }
        }
    }
}