
using ScratchSharp.Project;
using System;

namespace MonoScratch.Compiler {

    public static class ControlBlocks {

        public abstract class SubstackBlock : ItmScratchBlock {
            public readonly BlockBlockInput Substack;

            protected SubstackBlock(ScratchBlock block) : base(block) {
                if (block.Inputs.ContainsKey("SUBSTACK"))
                    Substack = ItmScratchBlockInput.From(block.Inputs["SUBSTACK"]) as BlockBlockInput
                        ?? throw new SystemException("SUBSTACK must be a block.");
                else Substack = new BlockBlockInput();
            }
        }

        public class RepeatBlock : SubstackBlock {
            public readonly ItmScratchBlockInput Times;

            public RepeatBlock(ScratchBlock block) : base(block) {
                Times = ItmScratchBlockInput.From(block.Inputs["TIMES"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                string i = ctx.GetNextSymbol("i", false);

                ctx.Source.AppendLine($"for (int {i} = 0; {i} < Math.Round((double) {Times.GetCode(ctx, BlockReturnType.NUMBER)}); {i}++)");
                ctx.Source.PushBlock();
                Substack.AppendExecute(ctx);
                ctx.AppendSoftYield();
                ctx.Source.PopBlock();
            }

            public static RepeatBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new RepeatBlock(scratchBlock);
        }

        public abstract class ConditionalSubstackBlock : SubstackBlock {
            public readonly ItmScratchBlockInput Condition;

            public ConditionalSubstackBlock(ScratchBlock block) : base(block) {
                Condition = ItmScratchBlockInput.From(block.Inputs["CONDITION"]);
            }
        }

        public class IfBlock : ConditionalSubstackBlock {
            public IfBlock(ScratchBlock block) : base(block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"if ({Condition.GetCode(ctx, BlockReturnType.BOOLEAN)})");
                ctx.Source.PushBlock();
                Substack.AppendExecute(ctx);
                ctx.Source.PopBlock();
            }

            public static IfBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new IfBlock(scratchBlock);
        }
    }
}