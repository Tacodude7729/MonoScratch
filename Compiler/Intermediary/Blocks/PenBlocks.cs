using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class PenBlocks {

        public class ClearBlock : ItmScratchBlock {
            public ClearBlock(ScratchBlock block) : base(block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine("Program.Runtime.Renderer.PenClear();");
            }

            public static ClearBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new ClearBlock(scratchBlock);
        }

        public class StampBlock : ItmScratchBlock {
            public StampBlock(ScratchBlock block) : base(block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite)
                    ctx.Source.AppendLine("Program.Runtime.Renderer.PenStamp(this);");
            }

            public static StampBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new StampBlock(scratchBlock);
        }

        public class PenUpDownBlock : ItmScratchBlock {
            public readonly bool PenDown;

            public PenUpDownBlock(ScratchBlock block, bool penDown) : base(block) {
                PenDown = penDown;
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite)
                    ctx.Source.AppendLine($"PenDown = {(PenDown ? "true" : "false")};");
            }
        }

        public static PenUpDownBlock CreatePenUpBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new PenUpDownBlock(scratchBlock, false);
        public static PenUpDownBlock CreatePenDownBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new PenUpDownBlock(scratchBlock, true);

        public class SetPenColorBlock : ItmScratchBlock {
            public readonly ItmScratchBlockInput Color;

            public SetPenColorBlock(ScratchBlock block) : base(block) {
                Color = ItmScratchBlockInput.From(block.Inputs["COLOR"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite)
                    ctx.Source.AppendLine($"PenColor = Utils.ToColor({Color.GetCode(ctx, BlockReturnType.ANY)});");
            }

            public static SetPenColorBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new SetPenColorBlock(scratchBlock);
        }

        public class PenSizeOperationBlock : ItmScratchBlock {
            public readonly ItmScratchBlockInput Size;
            public readonly string Operation;

            public PenSizeOperationBlock(ScratchBlock block, string operation) : base(block) {
                Size = ItmScratchBlockInput.From(block.Inputs["SIZE"]);
                Operation = operation;
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite)
                    ctx.Source.AppendLine($"PenSize {Operation} {Size.GetCode(ctx, BlockReturnType.NUMBER)};");
            }
        }

        public static PenSizeOperationBlock CreateSetPenSizeBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new PenSizeOperationBlock(scratchBlock, "=");
        public static PenSizeOperationBlock CreateChangePenSizeByBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new PenSizeOperationBlock(scratchBlock, "+=");
    }
}