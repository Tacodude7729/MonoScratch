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

    public class ItmScratchBlock {

        public readonly string Opcode;
        public readonly ScratchBlock Block;

        public ItmScratchBlock(ScratchBlock block) {
            Opcode = block.Opcode;
            Block = block;
        }

        public virtual void AppendExecute(SourceGeneratorContext ctx)
            => throw new NotImplementedException($"Can't execute block '{Opcode}'.");

        public virtual string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType)
            => throw new NotImplementedException($"Can't evaluate block '{Opcode}'.");

        public virtual BlockReturnType GetValueCodeReturnType(SourceGeneratorContext ctx, BlockReturnType requestedType)
            => throw new NotImplementedException($"Can't evaluate block '{Opcode}'.");
    }

    public abstract class ItmScratchHatBlock : ItmScratchBlock {
        public readonly string ListenerMethodName;
        public abstract string RunnerMethodName { get; }

        public ItmScratchHatBlock(SourceGeneratorContext ctx, ScratchBlock block, string method) : base(block) {
            ListenerMethodName = ctx.GetNextSymbol(method);
        }

        public virtual void AppendListenerMethodHeader(SourceGeneratorContext ctx) {
            ctx.Source.AppendLine($"public IEnumerable<YieldReason> {ListenerMethodName}()");
        }

        public abstract void AppendRunnerMethod(SourceGeneratorContext ctx, List<ItmScratchHatBlock> hats);
    }

    public class ItmScratchSimpleHatBlock : ItmScratchHatBlock {
        public override string RunnerMethodName { get; }

        public ItmScratchSimpleHatBlock(SourceGeneratorContext ctx, ScratchBlock block, string method) : base(ctx, block, method + "Listener") {
            RunnerMethodName = ctx.GetNextSymbol("On " + method);
        }

        public override void AppendRunnerMethod(SourceGeneratorContext ctx, List<ItmScratchHatBlock> hats) {
            ctx.Source.AppendLine($"public void {RunnerMethodName}()");
            ctx.Source.PushBlock();

            foreach (ItmScratchHatBlock hat in hats) {
                ctx.Source.AppendLine($"Utils.StartThread({hat.ListenerMethodName});");
            }

            ctx.Source.PopBlock();
        }
    }

}