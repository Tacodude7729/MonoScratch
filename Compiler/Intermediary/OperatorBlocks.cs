using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class OperatorBlocks {

        public class TwoNumberOperatorBlock : ItmScratchBlock {
            public ItmScratchBlockInput Num1, Num2;
            public readonly string Operation;

            public TwoNumberOperatorBlock(ScratchBlock scratchBlock, string operation) : base(scratchBlock) {
                Num1 = ItmScratchBlockInput.From(scratchBlock.Inputs["NUM1"]);
                Num2 = ItmScratchBlockInput.From(scratchBlock.Inputs["NUM2"]);
                Operation = operation;
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"({Num1.GetCode(ctx, BlockReturnType.NUMBER)} {Operation} {Num2.GetCode(ctx, BlockReturnType.NUMBER)})";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) => BlockReturnType.NUMBER;
        }

        public static ItmScratchBlock CreateAddBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoNumberOperatorBlock(scratchBlock, "+");
        public static ItmScratchBlock CreateSubtractBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoNumberOperatorBlock(scratchBlock, "-");
        public static ItmScratchBlock CreateMultiplyBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoNumberOperatorBlock(scratchBlock, "*");

        public class TwoNumberFunctionOperatorBlock : TwoNumberOperatorBlock {
            public readonly string FunctionName;

            public TwoNumberFunctionOperatorBlock(ScratchBlock scratchBlock, string functionName) : base(scratchBlock, "/") {
                FunctionName = functionName;
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"{FunctionName}({Num1.GetCode(ctx, BlockReturnType.NUMBER)}, {Num2.GetCode(ctx, BlockReturnType.NUMBER)})";
            }
        }

        // This is needed to handle the Num2 == 0 case.
        public static ItmScratchBlock CreateDevideBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoNumberFunctionOperatorBlock(scratchBlock, "Utils.Devide");

        public class TwoValueBooleanOperatorBlock : ItmScratchBlock {
            public ItmScratchBlockInput Op1, Op2;
            public readonly string Operation;
            public readonly BlockReturnType OperationTarget;

            public TwoValueBooleanOperatorBlock(ScratchBlock scratchBlock, string operation, BlockReturnType operationTarget) : base(scratchBlock) {
                Op1 = ItmScratchBlockInput.From(scratchBlock.Inputs, "OPERAND1");
                Op2 = ItmScratchBlockInput.From(scratchBlock.Inputs, "OPERAND2");
                Operation = operation;
                OperationTarget = operationTarget;
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"({Op1.GetCode(ctx, OperationTarget)} {Operation} {Op2.GetCode(ctx, OperationTarget)})";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) => BlockReturnType.BOOLEAN;
        }

        public class TwoValueBooleanCompareBlock : TwoValueBooleanOperatorBlock {
            public TwoValueBooleanCompareBlock(ScratchBlock scratchBlock, string operation) : base(scratchBlock, operation, BlockReturnType.ANY) { }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"(MonoScratchValue.Compare({Op1.GetCode(ctx, OperationTarget)}, {Op2.GetCode(ctx, OperationTarget)}) {Operation} 0)";
            }
        }

        public static TwoValueBooleanCompareBlock CreateGraterThanBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoValueBooleanCompareBlock(scratchBlock, ">");
        public static TwoValueBooleanCompareBlock CreateLessThanBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoValueBooleanCompareBlock(scratchBlock, "<");
        public static TwoValueBooleanCompareBlock CreateEqualsBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoValueBooleanCompareBlock(scratchBlock, "==");

        public static TwoValueBooleanOperatorBlock CreateAndBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoValueBooleanOperatorBlock(scratchBlock, "&&", BlockReturnType.BOOLEAN);
        public static TwoValueBooleanOperatorBlock CreateOrBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoValueBooleanOperatorBlock(scratchBlock, "||", BlockReturnType.BOOLEAN);

        public class NotBlock : ItmScratchBlock {
            public ItmScratchBlockInput Operand;

            public NotBlock(ScratchBlock block) : base(block) {
                Operand = ItmScratchBlockInput.From(block.Inputs, "OPERAND");
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"(!{Operand.GetCode(ctx, BlockReturnType.BOOLEAN)})";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) => BlockReturnType.BOOLEAN;

            public static NotBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new NotBlock(scratchBlock);
        }

        public class JoinBlock : ItmScratchBlock {
            public ItmScratchBlockInput Str1, Str2;

            public JoinBlock(ScratchBlock block) : base(block) {
                Str1 = ItmScratchBlockInput.From(block.Inputs["STRING1"]);
                Str2 = ItmScratchBlockInput.From(block.Inputs["STRING2"]);
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"{Str1.GetCode(ctx, BlockReturnType.STRING)} + {Str2.GetCode(ctx, BlockReturnType.STRING)}";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) => BlockReturnType.STRING;

            public static JoinBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new JoinBlock(scratchBlock);
        }

        public static ItmScratchBlock CreateModBlock(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new TwoNumberFunctionOperatorBlock(scratchBlock, "Utils.Mod");


        public class RoundBlock : ItmScratchBlock {
            public ItmScratchBlockInput Num;

            public RoundBlock(ScratchBlock block) : base(block) {
                Num = ItmScratchBlockInput.From(block.Inputs["NUM"]);
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"Math.Round({Num.GetCode(ctx, BlockReturnType.NUMBER)})";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) => BlockReturnType.NUMBER;

            public static RoundBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new RoundBlock(scratchBlock);
        }


    }
}