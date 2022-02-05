
using ScratchSharp.Project;
using System;
using System.Collections.Generic;
using System.Text;

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
                CodeName = ctx.GetNextSymbol(Name, false);
            }
        }

        public class CallBlock : ItmScratchBlock {

            public readonly Dictionary<string, ItmScratchBlockInput> Inputs;
            public readonly string Proccode;

            public CallBlock(ScratchBlock block) : base(block) {
                Inputs = new Dictionary<string, ItmScratchBlockInput>();
                Proccode = block.Mutation?.ProcCode
                    ?? throw new SystemException("Expected mutation on procedure call.");

                foreach (KeyValuePair<string, BlockInput> input in block.Inputs) {
                    Inputs.Add(input.Key, ItmScratchBlockInput.From(input.Value));
                }
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.CurrentTarget?.Procedures.TryGetValue(Proccode, out DefinitionBlock? procedure) ?? false) {
                    StringBuilder line = new StringBuilder();

                    string yr = ctx.GetNextSymbol("yr");
                    if (procedure.YieldsThread)
                        line.Append($"foreach (YieldReason {yr} in ");

                    line.Append(procedure.MethodName);
                    line.Append("(");
                    int i = 0;
                    foreach (ProcedureArgument argument in procedure.ArgumentIdMap.Values) {
                        ItmScratchBlockInput input;
                        if (Inputs.ContainsKey(argument.ID)) {
                            input = Inputs[argument.ID];

                            if (argument.ArgType == ProcedureArgumentType.VALUE) {
                                line.Append("new MonoScratchValue(");
                                line.Append(input.GetCode(ctx, BlockReturnType.ANY));
                                line.Append(")");
                            } else {
                                line.Append(input.GetCode(ctx, BlockReturnType.BOOLEAN));
                            }
                        } else {
                            if (argument.ArgType == ProcedureArgumentType.BOOLEAN) {
                                line.Append("false");
                            } else {
                                throw new SystemException("No argument provided for string / number type proc call arg.");
                            }
                        }

                        if (++i != procedure.ArgumentCount) {
                            line.Append(", ");
                        }
                    }
                    line.Append(")");

                    if (procedure.YieldsThread) {
                        line.Append(")");
                        ctx.Source.AppendLine(line.ToString());
                        ctx.Source.PushBlock();
                        if (ctx.ScreenRefresh) ctx.Source.AppendLine($"yield return {yr};");
                        else ctx.Source.AppendLine($"if ({yr} != YieldReason.YIELD) yield return {yr};");
                        ctx.Source.PopBlock();
                    } else {
                        line.Append(";");
                        ctx.Source.AppendLine(line.ToString());
                    }

                } else {
                    switch (Proccode) {
                        case "\u200B\u200Blog\u200B\u200B %s":
                            ctx.Source.AppendLine($"Log.Info({Inputs["arg0"].GetCode(ctx, BlockReturnType.STRING)}, {SourceGenerator.StringValue("[" + (ctx.CurrentTarget?.Target.Name ?? "?") + "]")});");
                            break;
                        case "\u200B\u200Bwarn\u200B\u200B %s":
                            ctx.Source.AppendLine($"Log.Warn({Inputs["arg0"].GetCode(ctx, BlockReturnType.STRING)}, {SourceGenerator.StringValue("[" + (ctx.CurrentTarget?.Target.Name ?? "?") + "]")});");
                            break;
                        case "\u200B\u200Berror\u200B\u200B %s":
                            ctx.Source.AppendLine($"Log.Error({Inputs["arg0"].GetCode(ctx, BlockReturnType.STRING)}, {SourceGenerator.StringValue("[" + (ctx.CurrentTarget?.Target.Name ?? "?") + "]")});");
                            break;
                        case "\u200B\u200Bbreakpoint\u200B\u200B":
                            ctx.Source.AppendLine($"Log.Info(\"Hit Scratch Addons breakpoint.\", {SourceGenerator.StringValue("[" + (ctx.CurrentTarget?.Target.Name ?? "?") + "]")});");
                            break;
                    }
                }
            }

            public static CallBlock Create(SourceGeneratorContext ctx, ScratchBlock block) {
                return new CallBlock(block);
            }
        }

        public class DefinitionBlock : ItmScratchBlock {

            public readonly Dictionary<string, ProcedureArgument> ArgumentNameMap;
            public readonly Dictionary<string, ProcedureArgument> ArgumentIdMap;
            public readonly string Proccode;
            public readonly string MethodName;

            public readonly bool ScreenRefresh;

            public bool YieldsThread => ScreenRefresh;

            public int ArgumentCount => ArgumentIdMap.Count;

            public DefinitionBlock(SourceGeneratorContext ctx, ScratchBlock block, Dictionary<string, ProcedureArgument> nameMap, Dictionary<string, ProcedureArgument> idMap, string proccode, bool screenrefresh) : base(block) {
                Proccode = proccode;
                ArgumentNameMap = nameMap;
                ArgumentIdMap = idMap;
                ScreenRefresh = screenrefresh;
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

                Dictionary<string, ProcedureArgument> nameMap = new Dictionary<string, ProcedureArgument>();
                Dictionary<string, ProcedureArgument> idMap = new Dictionary<string, ProcedureArgument>();
                for (int i = 0; i < argumentsTypes.Count; i++) {
                    ProcedureArgument argument = new ProcedureArgument(ctx, mutationArguments[i], argumentsTypes[i]);
                    nameMap[argument.Name] = argument; // Blocks can have inputs with duplicate names, and they overwrite eachother
                    idMap.Add(argument.ID, argument); // But inputs which overrite eachother sitll have different ids.
                }
                return new DefinitionBlock(ctx, block, nameMap, idMap, proc, !(mutation.WithoutScreenRefresh ?? false));
            }
        }

        public class ArgumentReporterStringNumber : ItmScratchBlock {
            public readonly string ArgumentName;

            public ArgumentReporterStringNumber(ScratchBlock block) : base(block) {
                ArgumentName = block.Fields["VALUE"].Name;
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                if (!(ctx.CurrentProcedure?.ArgumentNameMap.TryGetValue(ArgumentName, out ProcedureArgument? argument) ?? false)) {
                    return "MonoScratchValue.ZERO";
                }
                return argument.CodeName;
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return BlockReturnType.VALUE;
            }

            public static ArgumentReporterStringNumber Create(SourceGeneratorContext ctx, ScratchBlock block) {
                return new ArgumentReporterStringNumber(block);
            }
        }

        public class ArgumentReporterBoolean : ItmScratchBlock {
            public readonly string ArgumentName;

            public ArgumentReporterBoolean(ScratchBlock block) : base(block) {
                ArgumentName = block.Fields["VALUE"].Name;
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                if (!(ctx.CurrentProcedure?.ArgumentNameMap.TryGetValue(ArgumentName, out ProcedureArgument? argument) ?? false)) {
                    return "false";
                }
                return argument.CodeName;
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return BlockReturnType.BOOLEAN;
            }

            public static ArgumentReporterBoolean Create(SourceGeneratorContext ctx, ScratchBlock block) {
                return new ArgumentReporterBoolean(block);
            }
        }

    }
}