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

        public abstract string GetCode(SourceGeneratorContext ctx, BlockReturnType type);
    }

    public class RawBlockInput : ItmScratchBlockInput {
        public readonly string Value;

        public RawBlockInput(BlockInputPrimitiveRaw input) {
            Value = input.Value;
        }

        public override string GetCode(SourceGeneratorContext ctx, BlockReturnType type) {
            switch (type) {
                case BlockReturnType.STRING:
                    return SourceGenerator.StringValue(Value);
                case BlockReturnType.NUMBER:
                    return BlockUtils.InterperateString(Value).ToString();
                case BlockReturnType.BOOLEAN:
                    throw new SystemException("Cannot convert from raw block input to boolean.");
                case BlockReturnType.ANY:
                    return BlockUtils.InterperateValue(Value);
            }
            throw new SystemException("");
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

        public override string GetCode(SourceGeneratorContext ctx, BlockReturnType type) {
            if (ID == null)
                throw new SystemException("Cannot evaluate a null block block input.");

            ItmScratchBlock? block = null;

            if (!(ctx.CurrentTarget?.Blocks.TryGetValue(ID, out block) ?? false))
                throw new SystemException($"Cannot find block '{ID}'.");

            BlockReturnType returnType = block.GetValueCodeReturnType(ctx, type);
            string code = block.GetValueCode(ctx, type);

            switch (type) {
                case BlockReturnType.STRING:
                    switch (returnType) {
                        case BlockReturnType.STRING:
                        case BlockReturnType.ANY:
                            return code;
                        case BlockReturnType.NUMBER:
                            return code + ".ToString()";
                    }
                    break;
                case BlockReturnType.NUMBER:
                    switch (returnType) {
                        case BlockReturnType.ANY:
                        case BlockReturnType.NUMBER:
                            return code;
                        case BlockReturnType.STRING:
                            return $"Utils.StringToNumber({code})";
                    }
                    break;
                case BlockReturnType.BOOLEAN:
                    switch (returnType) {
                        case BlockReturnType.ANY:
                        case BlockReturnType.BOOLEAN:
                            return code;
                        case BlockReturnType.STRING:
                            return $"(({code}) ? \"true\" : \"false\")";
                        case BlockReturnType.NUMBER:
                            return $"(({code}) ? 1 : 0)";
                    }
                    break;
                case BlockReturnType.ANY:
                    if (returnType == BlockReturnType.ANY)
                        return code;
                    break;
            }
            throw new SystemException($"Cannot convert from {returnType} to {type}.");
        }
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

        public override string GetCode(SourceGeneratorContext ctx, BlockReturnType type) {
            switch (type) {
                case BlockReturnType.STRING:
                    return GetVariable(ctx).GetCodeString(ctx);
                case BlockReturnType.NUMBER:
                    return GetVariable(ctx).GetCodeNumber(ctx);
                case BlockReturnType.BOOLEAN:
                    throw new SystemException("Cannot convert from variable to boolean.");
                case BlockReturnType.ANY:
                    return GetVariable(ctx).GetCode(ctx);
            }
            throw new SystemException("");
        }
    }

    public class BroadcastBlockInput : ItmScratchBlockInput {
        public readonly string ID;

        public BroadcastBlockInput(BlockInputPrimitiveBroadcast input) {
            ID = input.ID;
        }

        public override string GetCode(SourceGeneratorContext ctx, BlockReturnType type) {
            throw new NotImplementedException();
        }
    }
}