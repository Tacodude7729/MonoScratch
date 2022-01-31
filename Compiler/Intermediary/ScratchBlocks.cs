using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class ScratchBlocks {

        public delegate ItmScratchBlock BlockType(SourceGeneratorContext ctx, ScratchBlock scratchBlock);

        private static Dictionary<string, BlockType> _blocks;

        static ScratchBlocks() {
            _blocks = new Dictionary<string, BlockType>();

            AddBlock("event_whenflagclicked", EventBlocks.GreenFlagClicked);

            AddBlock("control_repeat", ControlBlocks.RepeatBlock.Create);

            AddBlock("data_setvariableto", DataBlocks.SetVariableToBlock.Create);
            AddBlock("data_changevariableby", DataBlocks.ChangeVariableByBlock.Create);

            AddBlock("procedures_definition", ProcedureBlocks.DefinitionBlock.Create);

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