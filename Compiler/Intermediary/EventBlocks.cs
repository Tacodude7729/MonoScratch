
using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class EventBlocks {

        public static ItmScratchHatBlock CreateGreenFlagClicked(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
            return new ItmScratchHatBlock(ctx, scratchBlock, "GreenFlag");
        }

        public class ItmScratchBroadcast {
            public readonly string Name, ID;
            public readonly string CodeName;
            public readonly List<BroadcastReceivedBlock> Listeners;

            public string RunListenersName => "On" + CodeName;

            public ItmScratchBroadcast(SourceGeneratorContext ctx, string name, string id) {
                Name = name;
                ID = id;
                CodeName = ctx.GetNextSymbol("Broadcast " + Name);
                Listeners = new List<BroadcastReceivedBlock>();
            }
        }

        public class BroadcastReceivedBlock : ItmScratchHatBlock {
            public readonly ItmScratchBroadcast Broadcast;

            public override bool ReturnThreads => true;

            public BroadcastReceivedBlock(SourceGeneratorContext ctx, ScratchBlock block, ItmScratchBroadcast broadcast) : base(ctx, block, broadcast.CodeName) {
                Broadcast = broadcast;
            }

            public override void AppendListenerMethodHeader(SourceGeneratorContext ctx) {
                ctx.Source.AppendLine($"IEnumerable<MonoScratchThread> ProjectEvents.{ListenerMethodName}()");
            }

            public static BroadcastReceivedBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                ItmScratchBroadcast broadcast = ctx.GetOrCreateBroadcast(scratchBlock.Fields["BROADCAST_OPTION"]);
                return new BroadcastReceivedBlock(ctx, scratchBlock, broadcast);
            }
        }

        public class BroadcastBlock : ItmScratchBlock {
            public readonly ItmScratchBroadcast Broadcast;

            public BroadcastBlock(ScratchBlock block, ItmScratchBroadcast broadcast) : base(block) {
                Broadcast = broadcast;
            }

            public override void AppendExecute(SourceGeneratorContext ctx) {
                
            }

            public static BroadcastBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                ItmScratchBroadcast broadcast = ctx.GetOrCreateBroadcast(scratchBlock.Fields["BROADCAST_INPUT"]);
                return new BroadcastBlock(scratchBlock, broadcast);
            }
        }

    }
}