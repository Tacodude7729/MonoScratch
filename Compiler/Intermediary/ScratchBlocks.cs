using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class ScratchBlocks {

        public delegate ItmScratchBlock BlockType(SourceGeneratorContext ctx, ScratchBlock scratchBlock);

        private static Dictionary<string, BlockType> _blocks;

        static ScratchBlocks() {
            _blocks = new Dictionary<string, BlockType>();

            AddBlock("motion_gotoxy", MotionBlocks.GotoXYBlock.Create);
            AddBlock("motion_changexby", MotionBlocks.ChangeXByBlock.Create);
            AddBlock("motion_setx", MotionBlocks.GotoXBlock.Create);
            AddBlock("motion_changeyby", MotionBlocks.ChangeYByBlock.Create);
            AddBlock("motion_sety", MotionBlocks.GotoYBlock.Create);

            AddBlock("looks_hide", LooksBlocks.CreateHideBlock);
            AddBlock("looks_show", LooksBlocks.CreateShowBlock);

            AddBlock("event_whenflagclicked", EventBlocks.CreateGreenFlagClicked);
            AddBlock("event_whenbroadcastreceived", EventBlocks.BroadcastReceivedBlock.Create);
            AddBlock("event_broadcast", EventBlocks.BroadcastBlock.Create);

            AddBlock("control_repeat", ControlBlocks.RepeatBlock.Create);
            AddBlock("control_if", ControlBlocks.IfBlock.Create);
            AddBlock("control_if_else", ControlBlocks.IfElseBlock.Create);
            AddBlock("control_repeat_until", ControlBlocks.RepeatUntilBlock.Create);
            AddBlock("control_forever", ControlBlocks.ForeverBlock.Create);
            AddBlock("control_stop", ControlBlocks.StopBlock.Create);

            AddBlock("sensing_resettimer", SensingBlocks.ResetTimerBlock.Create);
            AddBlock("sensing_timer", SensingBlocks.TimerBlock.Create);

            AddBlock("operator_add", OperatorBlocks.CreateAddBlock);
            AddBlock("operator_subtract", OperatorBlocks.CreateSubtractBlock);
            AddBlock("operator_multiply", OperatorBlocks.CreateMultiplyBlock);
            AddBlock("operator_divide", OperatorBlocks.CreateDevideBlock);
            AddBlock("operator_gt", OperatorBlocks.CreateGraterThanBlock);
            AddBlock("operator_lt", OperatorBlocks.CreateLessThanBlock);
            AddBlock("operator_equals", OperatorBlocks.CreateEqualsBlock);
            AddBlock("operator_and", OperatorBlocks.CreateAndBlock);
            AddBlock("operator_or", OperatorBlocks.CreateOrBlock);
            AddBlock("operator_not", OperatorBlocks.NotBlock.Create);
            AddBlock("operator_join", OperatorBlocks.JoinBlock.Create);
            AddBlock("operator_mod", OperatorBlocks.CreateModBlock);
            AddBlock("operator_round", OperatorBlocks.RoundBlock.Create);
            AddBlock("operator_random", OperatorBlocks.RandomBlock.Create);
            AddBlock("operator_mathop", OperatorBlocks.MathOperationBlock.Create);

            AddBlock("data_setvariableto", DataBlocks.SetVariableToBlock.Create);
            AddBlock("data_changevariableby", DataBlocks.ChangeVariableByBlock.Create);
            AddBlock("data_showvariable", DummyBlock.Create);
            AddBlock("data_hidevariable", DummyBlock.Create);
            AddBlock("data_addtolist", DataBlocks.ListAddToBlock.Create);
            AddBlock("data_deleteoflist", DataBlocks.ListDeleteBlock.Create);
            AddBlock("data_deletealloflist", DataBlocks.ListDeleteAllBlock.Create);
            AddBlock("data_insertatlist", DataBlocks.ListInsertBlock.Create);
            AddBlock("data_replaceitemoflist", DataBlocks.ListReplaceItemBlock.Create);
            AddBlock("data_itemoflist", DataBlocks.ListItemOf.Create);
            AddBlock("data_itemnumoflist", DataBlocks.ListItemNumOf.Create);
            AddBlock("data_lengthoflist", DataBlocks.ListLength.Create);
            AddBlock("data_listcontainsitem", DataBlocks.ListContainsItem.Create);
            AddBlock("data_showlist", DummyBlock.Create);
            AddBlock("data_hidelist", DummyBlock.Create);

            AddBlock("procedures_definition", ProcedureBlocks.DefinitionBlock.Create);
            AddBlock("procedures_call", ProcedureBlocks.CallBlock.Create);
            AddBlock("argument_reporter_string_number", ProcedureBlocks.ArgumentReporterStringNumber.Create);
            AddBlock("argument_reporter_boolean", ProcedureBlocks.ArgumentReporterBoolean.Create);
            AddBlock("procedures_prototype", DummyBlock.Create); // This block is handled in ProcedureBlocks.DefinitionBlock.Create

            AddBlock("pen_clear", PenBlocks.ClearBlock.Create);
            AddBlock("pen_penDown", PenBlocks.CreatePenDownBlock);
            AddBlock("pen_penUp", PenBlocks.CreatePenUpBlock);
            AddBlock("pen_setPenColorToColor", PenBlocks.SetPenColorBlock.Create);
            AddBlock("pen_changePenSizeBy", PenBlocks.CreateChangePenSizeByBlock);
            AddBlock("pen_setPenSizeTo", PenBlocks.CreateSetPenSizeBlock);

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