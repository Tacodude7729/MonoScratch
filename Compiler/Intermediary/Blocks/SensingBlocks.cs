using System;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class SensingBlocks {

        public class MouseDownBlock : ItmScratchBlock {

            public MouseDownBlock(ScratchBlock block) : base(block) { }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"SrcatchInput.IsMouseDown()";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType)
                => BlockReturnType.BOOLEAN;

            public static MouseDownBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new MouseDownBlock(scratchBlock);
        }

        public class KeyPressedBlock : ItmScratchBlock {
            public ItmScratchBlockInput KeyOptions;

            public KeyPressedBlock(ScratchBlock block) : base(block) {
                KeyOptions = ItmScratchBlockInput.From(block.Inputs["KEY_OPTION"]);
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return $"SrcatchInput.IsKeyDown({KeyOptions.GetCode(ctx, BlockReturnType.STRING)})";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType)
                => BlockReturnType.BOOLEAN;

            public static KeyPressedBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new KeyPressedBlock(scratchBlock);
        }

        public class KeyOptionsBlock : ItmScratchBlock {
            public readonly string KeyName;

            public KeyOptionsBlock(ScratchBlock block) : base(block) {
                KeyName = block.Fields["KEY_OPTION"].Name;
            }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return SourceGenerator.StringValue(KeyName);
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType)
                => BlockReturnType.STRING;

            public static KeyOptionsBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new KeyOptionsBlock(scratchBlock);
        }

        public class TimerBlock : ItmScratchBlock {
            public TimerBlock(ScratchBlock block) : base(block) { }

            public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
                return "Program.Runtime.Timer";
            }

            public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) =>
                BlockReturnType.NUMBER;

            public static TimerBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new TimerBlock(scratchBlock);
        }

        public class ResetTimerBlock : ItmScratchBlock {
            public ResetTimerBlock(ScratchBlock block) : base(block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine("Program.Runtime.TimerStopwatch.Restart();");
            }

            public static ResetTimerBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
                new ResetTimerBlock(scratchBlock);
        }
    }
}