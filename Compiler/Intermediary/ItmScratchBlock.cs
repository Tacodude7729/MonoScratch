using System;
using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public enum BlockReturnType {
        STRING,
        NUMBER,
        BOOLEAN,
        VALUE,
        ANY // Can be a 'string', 'double', 'bool' or 'MonoScratchValue'
    }

    public enum BlockYieldType {
        NONE = 0,
        YIELD = 1,
        YIELD_TICK = 2 // This is both STATUS_PROMISE_WAIT and STATUS_YIELD_TICK in scratch-vm
    }

    public class ItmScratchBlock {

        public readonly string Opcode;
        public readonly ScratchBlock Block;

        private BlockYieldType? YieldType;

        public ItmScratchBlock(ScratchBlock block) {
            Opcode = block.Opcode;
            Block = block;
            YieldType = null;
        }

        public virtual void AppendExecute(SourceGeneratorContext ctx)
            => throw new NotImplementedException($"Can't execute block '{Opcode}'.");

        public virtual string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType)
            => throw new NotImplementedException($"Can't evaluate block '{Opcode}'.");

        public virtual BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType)
            => throw new NotImplementedException($"Can't evaluate block '{Opcode}'.");

        public BlockYieldType GetYieldType(SourceGeneratorContext ctx) {
            if (YieldType != null) return YieldType.Value;
            return (YieldType = CalculateYieldType(ctx)).Value;
        }

        protected virtual BlockYieldType CalculateYieldType(SourceGeneratorContext ctx)
            => BlockYieldType.NONE;
    }

    public abstract class ItmScratchHatBlock : ItmScratchBlock {
        public readonly string ListenerMethodName;
        public abstract string RunnerMethodName { get; }

        public ItmScratchHatBlock(SourceGeneratorContext ctx, ScratchBlock block, string method) : base(block) {
            ListenerMethodName = ctx.GetNextSymbol(method);
        }

        public virtual void AppendListenerMethodHeader(SourceGeneratorContext ctx) {
            if (GetYieldType(ctx) == BlockYieldType.NONE)
                ctx.Source.AppendLine($"public void {ListenerMethodName}()");
            else
                ctx.Source.AppendLine($"public IEnumerable<YieldReason> {ListenerMethodName}()");
        }

        protected override BlockYieldType CalculateYieldType(SourceGeneratorContext ctx) {
            return ctx.GetHatYieldType(this);
        }

        public abstract void AppendRunnerMethod(SourceGeneratorContext ctx, List<ItmScratchHatBlock> hats);
    }

    public class ItmScratchSimpleHatBlock : ItmScratchHatBlock {
        public override string RunnerMethodName { get; }

        public virtual bool RestartExistingThreads => false;

        public ItmScratchSimpleHatBlock(SourceGeneratorContext ctx, ScratchBlock block, string method) : base(ctx, block, method + "Listener") {
            RunnerMethodName = "On" + method;
        }

        public override void AppendRunnerMethod(SourceGeneratorContext ctx, List<ItmScratchHatBlock> hats) {
            ctx.Source.AppendLine($"public void {RunnerMethodName}()");
            ctx.Source.PushBlock();

            foreach (ItmScratchHatBlock hat in hats) {
                ctx.Source.AppendLine($"Program.Runtime.StartThread({hat.ListenerMethodName}, {(RestartExistingThreads ? "true" : "false")});");
            }

            ctx.Source.PopBlock();
        }
    }

    public class DummyBlock : ItmScratchBlock {
        public DummyBlock(ScratchBlock block) : base(block) { }
        public override void AppendExecute(SourceGeneratorContext ctx) { }
        public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) => "0";
        public override BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType) =>
            BlockReturnType.NUMBER;

        public static DummyBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) =>
            new DummyBlock(scratchBlock);
    }

}