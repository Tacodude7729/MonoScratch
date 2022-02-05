using System;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class SensingBlocks {
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