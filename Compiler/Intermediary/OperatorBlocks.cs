using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class OperatorBlocks {

        private abstract class TwoNumberOperatorBlock : ItmScratchBlock {

            public TwoNumberOperatorBlock(ScratchBlock scratchBlock) : base(scratchBlock) {
                BlockInput a = scratchBlock.Inputs[""];
            }
        }
        
    }
}