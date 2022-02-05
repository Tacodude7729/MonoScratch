using ScratchSharp.Project;
using System;
using System.Collections.Generic;

namespace MonoScratch.Compiler {
    public abstract class ItmScratchBlockInput {

        public static readonly ItmScratchBlockInput EMPTY = new RawBlockInput("");

        public static ItmScratchBlockInput From(Dictionary<string, BlockInput> inputs, string input) {
            return From(inputs, input, EMPTY);
        }

        public static ItmScratchBlockInput From(Dictionary<string, BlockInput> inputs, string input, ItmScratchBlockInput alt) {
            if (inputs.ContainsKey(input)) return From(inputs[input]);
            return alt;
        }

        public static ItmScratchBlockInput From(BlockInput input) {
            return From(input.Block);
        }

        public static ItmScratchBlockInput From(BlockInputPrimitive input) {
            if (input is BlockInputPrimitiveID inputId) {
                return new BlockBlockInput(inputId);
            } else if (input is BlockInputPrimitiveVariableList varList) {
                return new VariableBlockInput(varList);
            } else if (input is BlockInputPrimitiveBroadcast broadcast) {
                throw new SystemException("Cannot create Scratch block input from broadcast!");
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

        public RawBlockInput(string value) {
            Value = value;
        }

        public override string GetCode(SourceGeneratorContext ctx, BlockReturnType type) {
            switch (type) {
                case BlockReturnType.STRING:
                    return SourceGenerator.StringValue(Value);
                case BlockReturnType.NUMBER:
                    return BlockUtils.StringToNum(Value).ToString() + "d";
                case BlockReturnType.ANY:
                    return BlockUtils.StringToNumString(Value);
                case BlockReturnType.VALUE:
                    return $"new MonoScratchValue({BlockUtils.StringToNumString(Value)})";
                case BlockReturnType.BOOLEAN:
                    return BlockUtils.StringToBool(Value);
            }
            throw new SystemException($"Cannot convert raw input to {type}.");
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
                            return code;
                        case BlockReturnType.ANY:
                        case BlockReturnType.NUMBER:
                        case BlockReturnType.VALUE:
                            return code + ".ToString()";
                        case BlockReturnType.BOOLEAN:
                            return $"(({code}) ? \"true\" : \"false\")";
                    }
                    break;
                case BlockReturnType.NUMBER:
                    switch (returnType) {
                        case BlockReturnType.NUMBER:
                            return code;
                        case BlockReturnType.STRING:
                            return $"Utils.StringToNumber({code})";
                        case BlockReturnType.BOOLEAN:
                            return $"(({code}) ? 1 : 0)";
                        case BlockReturnType.VALUE:
                            return code + ".AsNumber()";
                    }
                    break;
                case BlockReturnType.BOOLEAN:
                    if (returnType == BlockReturnType.BOOLEAN)
                        return code;
                    break;
                case BlockReturnType.ANY:
                    return code;
                case BlockReturnType.VALUE:
                    if (returnType == BlockReturnType.VALUE)
                        return code;
                    return $"new MonoScratchValue({code})";
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
                case BlockReturnType.VALUE:
                case BlockReturnType.ANY:
                    return $"new MonoScratchValue({GetVariable(ctx).GetCode(ctx)})";
            }
            throw new SystemException("");
        }
    }
}