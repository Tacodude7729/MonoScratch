using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class LooksBlocks {

        public class ShowHideBlock : ItmScratchBlock {
            public readonly bool Show;

            public ShowHideBlock(ScratchBlock block, bool show) : base(block) {
                Show = show;
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                if (ctx.IsInSprite)
                    ctx.Source.AppendLine($"Visible = {(Show ? "true" : "false")};");
            }
        }

        public static ShowHideBlock CreateShowBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new ShowHideBlock(scratchBlock, true);
        public static ShowHideBlock CreateHideBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new ShowHideBlock(scratchBlock, false);


    }
}