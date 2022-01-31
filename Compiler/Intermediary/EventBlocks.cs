
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class EventBlocks {

        public const string GreenFlagMethodName = "OnGreenFlag";

        public static ItmScratchHatBlock GreenFlagClicked(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
            return new ItmScratchHatBlock(ctx, scratchBlock, GreenFlagMethodName);
        }

    }
}