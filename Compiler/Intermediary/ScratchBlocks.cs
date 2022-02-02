using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class ScratchBlocks {

        public delegate ItmScratchBlock BlockType(SourceGeneratorContext ctx, ScratchBlock scratchBlock);

        private static Dictionary<string, BlockType> _blocks;

        static ScratchBlocks() {
            _blocks = new Dictionary<string, BlockType>();

            AddBlock("event_whenflagclicked", EventBlocks.CreateGreenFlagClicked);
            AddBlock("event_whenbroadcastreceived", EventBlocks.BroadcastReceivedBlock.Create);

            AddBlock("control_repeat", ControlBlocks.RepeatBlock.Create);

            AddBlock("operator_add", OperatorBlocks.AddBlock.Create);

            AddBlock("data_setvariableto", DataBlocks.SetVariableToBlock.Create);
            AddBlock("data_changevariableby", DataBlocks.ChangeVariableByBlock.Create);
            AddBlock("data_addtolist", DataBlocks.ListAddToBlock.Create);
            AddBlock("data_deleteoflist", DataBlocks.ListDeleteBlock.Create);
            AddBlock("data_deletealloflist", DataBlocks.ListDeleteAllBlock.Create);
            AddBlock("data_insertatlist", DataBlocks.ListInsertBlock.Create);
            AddBlock("data_replaceitemoflist", DataBlocks.ListReplaceItemBlock.Create);
            AddBlock("data_itemoflist", DataBlocks.ListItemOf.Create);
            AddBlock("data_itemnumoflist", DataBlocks.ListItemNumOf.Create);
            AddBlock("data_lengthoflist", DataBlocks.ListLength.Create);
            AddBlock("data_listcontainsitem", DataBlocks.ListContainsItem.Create);

            AddBlock("procedures_definition", ProcedureBlocks.DefinitionBlock.Create);
            AddBlock("procedures_call", ProcedureBlocks.CallBlock.Create);
            AddBlock("argument_reporter_string_number", ProcedureBlocks.ArgumentReporterStringNumber.Create);
            AddBlock("argument_reporter_boolean", ProcedureBlocks.ArgumentReporterBoolean.Create);

        }

        public static void AddBlock(string opcode, BlockType type) {
            _blocks.Add(opcode, type);
        }

        public static ItmScratchBlock? Create(SourceGeneratorContext ctx, ScratchBlock block) {
            if (_blocks.TryGetValue(block.Opcode, out BlockType? type))
                return type.Invoke(ctx, block);
            return null;
        }
    }

}