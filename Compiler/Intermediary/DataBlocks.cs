
using ScratchSharp.Project;
using System;

namespace MonoScratch.Compiler {

    public static class DataBlocks {

        public abstract class VariableOperationBlock : ItmScratchBlock {
            public readonly ItmScratchVariable Variable;
            public readonly ItmScratchBlockInput Value;

            public VariableOperationBlock(SourceGeneratorContext ctx, ScratchBlock block) : base(block) {
                Variable = ctx.GetVariable(block.Fields["VARIABLE"]);
                Value = ItmScratchBlockInput.From(block.Inputs["VALUE"]);
            }
        }

        public class SetVariableToBlock : VariableOperationBlock {
            public SetVariableToBlock(SourceGeneratorContext ctx, ScratchBlock block) : base(ctx, block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"{Variable.GetCode(ctx)}.Set({Value.GetCode(ctx, BlockReturnType.ANY)});");
            }

            public static SetVariableToBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                return new SetVariableToBlock(ctx, scratchBlock);
            }
        }

        public class ChangeVariableByBlock : VariableOperationBlock {
            public ChangeVariableByBlock(SourceGeneratorContext ctx, ScratchBlock block) : base(ctx, block) { }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"{Variable.GetCode(ctx)}.Set({Variable.GetCodeNumber(ctx)} + {Value.GetCode(ctx, BlockReturnType.NUMBER)});");
            }

            public static ChangeVariableByBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                return new ChangeVariableByBlock(ctx, scratchBlock);
            }
        }

        public abstract class ListBlock : ItmScratchBlock {
            public readonly BlockField ListField;

            public ListBlock(ScratchBlock block) : base(block) {
                ListField = block.Fields["LIST"];
            }
        }

        public class ListAddToBlock : ListBlock {
            public ItmScratchBlockInput Item;

            public ListAddToBlock(ScratchBlock block) : base(block) {
                Item = ItmScratchBlockInput.From(block.Inputs["ITEM"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ItmScratchList list = ctx.GetList(ListField);
                ctx.Source.AppendLine($"{list.GetCode(ctx)}.Add(new MonoScratchValue({Item.GetCode(ctx, BlockReturnType.ANY)}));");
            }

            public static ListAddToBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock)
                => new ListAddToBlock(scratchBlock);
        }

        public class ListDeleteBlock : ListBlock {
            public ItmScratchBlockInput Index;

            public ListDeleteBlock(ScratchBlock block) : base(block) {
                Index = ItmScratchBlockInput.From(block.Inputs["INDEX"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ItmScratchList list = ctx.GetList(ListField);
                ctx.Source.AppendLine($"{list.GetCode(ctx)}.Delete({Index.GetCode(ctx, BlockReturnType.NUMBER)});");
            }

            public static ListDeleteBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock)
                => new ListDeleteBlock(scratchBlock);
        }

        public class ListDeleteAllBlock : ListBlock {
            public ListDeleteAllBlock(ScratchBlock block) : base(block) {
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ItmScratchList list = ctx.GetList(ListField);
                ctx.Source.AppendLine($"{list.GetCode(ctx)}.DeleteAll();");
            }

            public static ListDeleteAllBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock)
                => new ListDeleteAllBlock(scratchBlock);
        }

        public class ListInsertBlock : ListBlock {
            public ItmScratchBlockInput Index, Item;

            public ListInsertBlock(ScratchBlock block) : base(block) {
                Index = ItmScratchBlockInput.From(block.Inputs["INDEX"]);
                Item = ItmScratchBlockInput.From(block.Inputs["ITEM"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ItmScratchList list = ctx.GetList(ListField);
                ctx.Source.AppendLine($"{list.GetCode(ctx)}.Insert({Index.GetCode(ctx, BlockReturnType.NUMBER)}, new MonoScratchValue({Item.GetCode(ctx, BlockReturnType.ANY)});");
            }

            public static ListInsertBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock)
                => new ListInsertBlock(scratchBlock);
        }

        public class ListReplaceItemBlock : ListBlock {
            public ItmScratchBlockInput Index, Item;

            public ListReplaceItemBlock(ScratchBlock block) : base(block) {
                Index = ItmScratchBlockInput.From(block.Inputs["INDEX"]);
                Item = ItmScratchBlockInput.From(block.Inputs["ITEM"]);
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                ItmScratchList list = ctx.GetList(ListField);
                ctx.Source.AppendLine($"{list.GetCode(ctx)}.Replace({Index.GetCode(ctx, BlockReturnType.NUMBER)}, new MonoScratchValue({Item.GetCode(ctx, BlockReturnType.ANY)});");
            }

            public static ListReplaceItemBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock)
                => new ListReplaceItemBlock(scratchBlock);
        }

        public class ListItemOf : ListBlock {
            public ItmScratchBlockInput Index;

            public ListItemOf(ScratchBlock block) : base(block) {
                Index = ItmScratchBlockInput.From(block.Inputs["INDEX"]);
            }

            // public override string GetValueCode(SourceGeneratorContext ctx, BlockReturnType requestedType) {
            //     return base.GetValueCode(ctx, requestedType);
            // }



            public static ListItemOf Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock)
                => new ListItemOf(scratchBlock);
        }

    }
}