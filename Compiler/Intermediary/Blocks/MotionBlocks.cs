using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class MotionBlocks {

        public class GotoXYBlock : ItmScratchBlock {
            public ItmScratchBlockInput X, Y;

            public GotoXYBlock(ScratchBlock block) : base(block) {
                X = ItmScratchBlockInput.From(block.Inputs["X"]);
                Y = ItmScratchBlockInput.From(block.Inputs["Y"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite) {
                    ctx.Source.AppendLine($"SetXY({X.GetCode(ctx, BlockReturnType.NUMBER)}, {Y.GetCode(ctx, BlockReturnType.NUMBER)});");
                }
            }

            public static GotoXYBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new GotoXYBlock(scratchBlock);
        }

        public class GotoXBlock : ItmScratchBlock {
            public ItmScratchBlockInput X;

            public GotoXBlock(ScratchBlock block) : base(block) {
                X = ItmScratchBlockInput.From(block.Inputs["X"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite) {
                    ctx.Source.AppendLine($"X = {X.GetCode(ctx, BlockReturnType.NUMBER)};");
                }
            }

            public static GotoXBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new GotoXBlock(scratchBlock);
        }


        public class GotoYBlock : ItmScratchBlock {
            public ItmScratchBlockInput Y;

            public GotoYBlock(ScratchBlock block) : base(block) {
                Y = ItmScratchBlockInput.From(block.Inputs["Y"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite) {
                    ctx.Source.AppendLine($"Y = {Y.GetCode(ctx, BlockReturnType.NUMBER)};");
                }
            }

            public static GotoYBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new GotoYBlock(scratchBlock);
        }

        public class ChangeXByBlock : ItmScratchBlock {
            public ItmScratchBlockInput DX;

            public ChangeXByBlock(ScratchBlock block) : base(block) {
                DX = ItmScratchBlockInput.From(block.Inputs["DX"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite) {
                    ctx.Source.AppendLine($"X += {DX.GetCode(ctx, BlockReturnType.NUMBER)};");
                }
            }

            public static ChangeXByBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new ChangeXByBlock(scratchBlock);
        }


        public class ChangeYByBlock : ItmScratchBlock {
            public ItmScratchBlockInput DY;

            public ChangeYByBlock(ScratchBlock block) : base(block) {
                DY = ItmScratchBlockInput.From(block.Inputs["DY"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite) {
                    ctx.Source.AppendLine($"Y += {DY.GetCode(ctx, BlockReturnType.NUMBER)};");
                }
            }

            public static ChangeYByBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new ChangeYByBlock(scratchBlock);
        }

    }
}