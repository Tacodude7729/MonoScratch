using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class OperatorBlocks {

        public abstract class TwoNumberOperatorBlock : ItmScratchBlock {
            public ItmScratchBlockInput Num1, Num2;

            public TwoNumberOperatorBlock(ScratchBlock scratchBlock) : base(scratchBlock) {
                Num1 = ItmScratchBlockInput.From(scratchBlock.Inputs["NUM1"]);
                Num2 = ItmScratchBlockInput.From(scratchBlock.Inputs["NUM2"]);
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return BlockReturnType.NUMBER;
            }
        }

        public class AddBlock : TwoNumberOperatorBlock {
            public AddBlock(ScratchBlock scratchBlock) : base(scratchBlock) {
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"{Num1.GetCode(ctx, BlockReturnType.NUMBER)} + {Num2.GetCode(ctx, BlockReturnType.NUMBER)}";
            }

            public static AddBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                return new AddBlock(scratchBlock);
            }
        }
    }
}