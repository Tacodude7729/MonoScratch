
using ScratchSharp.Project;
using System;

namespace MonoScratch.Compiler {

    public static class DataBlocks {

        public abstract class VariableOperationBlock : ItmScratchBlock {
            public readonly ItmScratchVariable Variable;
            public readonly ItmScratchBlockInput Value;

            public VariableOperationBlock(SourceGeneratorContext ctx, ScratchBlock block) : base(block) {
                Variable = ctx.GetVariable(block.Fields["VARIABLE"]);
                Value = ItmScratchBlockInput.From(block.Inputs["VALUE"]);
            }
        }

        public class SetVariableToBlock : VariableOperationBlock {
            public SetVariableToBlock(SourceGeneratorContext ctx, ScratchBlock block) : base(ctx, block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"{Variable.GetCode(ctx)}.Set({Value.GetPerferedValue(ctx)});");
            }

            public static SetVariableToBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                return new SetVariableToBlock(ctx, scratchBlock);
            }
        }

        public class ChangeVariableByBlock : VariableOperationBlock {
            public ChangeVariableByBlock(SourceGeneratorContext ctx, ScratchBlock block) : base(ctx, block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"{Variable.GetCode(ctx)}.Set({Variable.GetCodeNumber(ctx)} + {Value.GetNumberValue(ctx)});");
            }

            public static ChangeVariableByBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                return new ChangeVariableByBlock(ctx, scratchBlock);
            }
        }

    }
}