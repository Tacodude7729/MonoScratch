
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

            protected override BlockYieldType CalculateYieldType(SourceGeneratorContext ctx)
                => Substack.GetYieldType(ctx);
        }

        public class ForeverBlock : SubstackBlock {
            public ForeverBlock(ScratchBlock block) : base(block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine("while (true)");
                ctx.Source.PushBlock();
                Substack.AppendExecute(ctx);
                ctx.AppendSoftYield();
                ctx.Source.PopBlock();
            }

            public static ForeverBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new ForeverBlock(scratchBlock);
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

            protected override BlockYieldType CalculateYieldType(SourceGeneratorContext ctx)
                => BlockUtils.BiggestYield(Substack.GetYieldType(ctx), BlockYieldType.YIELD);

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

        public class IfElseBlock : ConditionalSubstackBlock {
            public readonly BlockBlockInput Substack2;

            protected IfElseBlock(ScratchBlock block) : base(block) {
                if (block.Inputs.ContainsKey("SUBSTACK2"))
                    Substack2 = ItmScratchBlockInput.From(block.Inputs["SUBSTACK2"]) as BlockBlockInput
                        ?? throw new SystemException("SUBSTACK must be a block.");
                else Substack2 = new BlockBlockInput();
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"if ({Condition.GetCode(ctx, BlockReturnType.BOOLEAN)})");
                ctx.Source.PushBlock();
                Substack.AppendExecute(ctx);
                ctx.Source.PopBlock();
                ctx.Source.AppendLine("else");
                ctx.Source.PushBlock();
                Substack2.AppendExecute(ctx);
                ctx.Source.PopBlock();
            }

            protected override BlockYieldType CalculateYieldType(SourceGeneratorContext ctx)
                => BlockUtils.BiggestYield(Substack.GetYieldType(ctx), Substack2.GetYieldType(ctx));

            public static IfElseBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new IfElseBlock(scratchBlock);
        }

        public class RepeatUntilBlock : ConditionalSubstackBlock {
            public RepeatUntilBlock(ScratchBlock block) : base(block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"while (!{Condition.GetCode(ctx, BlockReturnType.BOOLEAN)})");
                ctx.Source.PushBlock();
                Substack.AppendExecute(ctx);
                ctx.AppendSoftYield();
                ctx.Source.PopBlock();
            }
            
            protected override BlockYieldType CalculateYieldType(SourceGeneratorContext ctx)
                => BlockUtils.BiggestYield(Substack.GetYieldType(ctx), BlockYieldType.YIELD);

            public static RepeatUntilBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new RepeatUntilBlock(scratchBlock);
        }

        public class StopBlock : ItmScratchBlock {
            public readonly string StopOption;

            public StopBlock(ScratchBlock block) : base(block) {
                StopOption = block.Fields["STOP_OPTION"].Name;
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                switch (StopOption) {
                    case "this script":
                        if (ctx.CanYield)
                            ctx.Source.AppendLine("yield break;");
                        else
                            ctx.Source.AppendLine("return;");
                        return;
                }
                throw new SystemException($"Stop option {StopOption} not implimented.");
            }

            public static StopBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new StopBlock(scratchBlock);
        }
    }
}