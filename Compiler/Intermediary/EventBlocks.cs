
using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class EventBlocks {

        public static ItmScratchHatBlock CreateGreenFlagClicked(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
            return new ItmScratchSimpleHatBlock(ctx, scratchBlock, "GreenFlag");
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
            public override string RunnerMethodName { get; }

            public BroadcastReceivedBlock(SourceGeneratorContext ctx, ScratchBlock block, ItmScratchBroadcast broadcast) : base(ctx, block, broadcast.CodeName + "Listener") {
                Broadcast = broadcast;
                RunnerMethodName = broadcast.RunListenersName;
            }

            public override void AppendRunnerMethod(SourceGeneratorContext ctx, List<ItmScratchHatBlock> hats) {
                ctx.Source.AppendLine($"MonoScratchThread[] ProjectEvents.{RunnerMethodName}()");
                ctx.Source.PushBlock();
                ctx.Source.AppendLine("return new MonoScratchThread[] {");
                ctx.Source.PushIndent();
                for (int i = 0; i < hats.Count; i++) {
                    ctx.Source.AppendLine($"Program.Runtime.StartThread({hats[i].ListenerMethodName}, true){(i == hats.Count - 1 ? "" : ",")}");
                }
                ctx.Source.PopIndent();
                ctx.Source.AppendLine("};");
                ctx.Source.PopBlock();
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
                string target = ctx.GetNextSymbol("target", false);
                ctx.Source.AppendLine($"foreach (IMonoScratchTarget {target} in Program.Runtime.Targets.Forward()) {target}.{Broadcast.RunListenersName}();");
            }

            public static BroadcastBlock Create(SourceGeneratorContext ctx, ScratchBlock scratchBlock) {
                ItmScratchBroadcast broadcast = ctx.GetOrCreateBroadcast(scratchBlock.Inputs["BROADCAST_INPUT"]);
                return new BroadcastBlock(scratchBlock, broadcast);
            }
        }

    }
}


/*
        MonoScratchThread[][] receivers = new MonoScratchThread[Program.Runtime.Targets.Count][];
        int index = 0;
        foreach (IMonoScratchTarget target in Program.Runtime.Targets.Forward()) {
            receivers[index++] = target.OnBroadcastMessage1_();
        }
        for (index = 0; index < receivers.Length; index++) {
            MonoScratchThread[] receiverThreads = receivers[index];
            for (int j = 0; j < receiverThreads.Length; j++) {
                while (receiverThreads[j].Status != ThreadStatus.DONE)
                    yield return YieldReason.HARD_YIELD; // hard yield?
            }
        }

 */