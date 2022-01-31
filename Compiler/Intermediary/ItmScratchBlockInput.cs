using ScratchSharp.Project;
using System;

namespace MonoScratch.Compiler {
    public abstract class ItmScratchBlockInput {

        public static ItmScratchBlockInput From(BlockInput input) {
            return From(input.Block);
        }

        public static ItmScratchBlockInput From(BlockInputPrimitive input) {
            if (input is BlockInputPrimitiveID inputId) {
                return new BlockBlockInput(inputId);
            } else if (input is BlockInputPrimitiveVariableList varList) {
                return new VariableBlockInput(varList);
            } else if (input is BlockInputPrimitiveBroadcast broadcast) {
                return new BroadcastBlockInput(broadcast);
            } else if (input is BlockInputPrimitiveRaw raw) {
                return new RawBlockInput(raw);
            }
            throw new SystemException();
        }

        // public abstract string GetCode(SourceGeneratorContext ctx, BlockReturnType type);

        public virtual string GetPerferedValue(SourceGeneratorContext ctx) => throw new NotImplementedException();
        public virtual string GetNumberValue(SourceGeneratorContext ctx) => throw new NotImplementedException();
        public virtual string GetStringValue(SourceGeneratorContext ctx) => throw new NotImplementedException();
    }

    public class RawBlockInput : ItmScratchBlockInput {
        public readonly string Value;

        public RawBlockInput(BlockInputPrimitiveRaw input) {
            Value = input.Value;
        }

        public override string GetPerferedValue(SourceGeneratorContext ctx) {
            return BlockUtils.InterperateValue(Value);
        }

        public override string GetStringValue(SourceGeneratorContext ctx) {
            return SourceGenerator.StringValue(Value);
        }

        public override string GetNumberValue(SourceGeneratorContext ctx) {
            return BlockUtils.InterperateString(Value).ToString();
        }
    }

    public class BlockBlockInput : ItmScratchBlockInput {
        public readonly string? ID;

        public BlockBlockInput() {
            ID = null;
        }

        public BlockBlockInput(BlockInputPrimitiveID input) {
            ID = input.ID;
        }

        public void AppendExecute(SourceGeneratorContext ctx) {
            if (ID != null)
                ctx.AppendBlocks(ID);
        }

        // public override 
    }

    // For variables and lists
    public class VariableBlockInput : ItmScratchBlockInput {
        public readonly string ID;

        public VariableBlockInput(BlockInputPrimitiveVariableList input) {
            ID = input.ID;
        }

        public ItmScratchVariable GetVariable(SourceGeneratorContext ctx) {
            return ctx.GetVariable(ID) ?? throw new SystemException();
        }

        public override string GetPerferedValue(SourceGeneratorContext ctx) {
            // It would be nice to not have to convert everything to a string here.
            //  It's not obvious how to fix it though...
            return GetVariable(ctx).GetCodeString(ctx);
        }

        public override string GetStringValue(SourceGeneratorContext ctx) {
            return GetVariable(ctx).GetCodeString(ctx);
        }

        public override string GetNumberValue(SourceGeneratorContext ctx) {
            return GetVariable(ctx).GetCodeNumber(ctx);
        }
    }

    public class BroadcastBlockInput : ItmScratchBlockInput {
        public readonly string ID;

        public BroadcastBlockInput(BlockInputPrimitiveBroadcast input) {
            ID = input.ID;
        }
    }
}