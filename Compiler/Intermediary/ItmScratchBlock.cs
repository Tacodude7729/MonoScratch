using System;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public enum BlockReturnType {
        STRING,
        NUMBER,
        BOOLEAN,
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

    public class ItmScratchHatBlock : ItmScratchBlock {
        public readonly string ListenerMethodName;
        public readonly string MethodName;

        public ItmScratchHatBlock(SourceGeneratorContext ctx, ScratchBlock block, string method) : base(block) {
            MethodName = ctx.GetNextSymbol(method + "Listener");
            ListenerMethodName = method;
        }

        public virtual void AppendMethodHeader(SourceGeneratorContext ctx) {
            ctx.Source.AppendLine($"public IEnumerable<YieldReason> {MethodName}()");
        }

        public virtual void AppendListenerMethodHeader(SourceGeneratorContext ctx) {
            ctx.Source.AppendLine($"public void {ListenerMethodName}()");
        }
    }

}