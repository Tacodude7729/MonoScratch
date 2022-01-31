
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

                ctx.Source.AppendLine($"for (int {i} = 0; {i} < Math.Round((double) {Times.GetNumberValue(ctx)}); {i}++)");
                ctx.Source.PushBlock();
                Substack.AppendExecute(ctx);
                ctx.Source.AppendThreadYield();
                ctx.Source.PopBlock();
            }

            public static RepeatBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                return new RepeatBlock(scratchBlock);
            }
        }
    }
}