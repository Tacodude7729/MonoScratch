using System.Collections.Generic;
using System.Text;
using static MonoScratch.Compiler.EventBlocks;

namespace MonoScratch.Compiler {

    public static class InterfaceGenerator {

        public static void AppendSource(SourceGeneratorContext ctx) {

            // Dictionary<string, ItmScratchHatBlock> hats;

            // Interface
            ctx.Source.AppendLine("public static partial class Interface");
            ctx.Source.PushBlock();

            // GetSprites()
            ctx.Source.AppendLine("public static partial List<IMonoScratchSprite> GetSprites()");
            ctx.Source.PushBlock();
            ctx.Source.AppendLine("List<IMonoScratchSprite> sprites = new List<IMonoScratchSprite>();");
            
            List<ItmScratchSprite> sprites = new List<ItmScratchSprite>();
            sprites.AddRange(ctx.Sprites.Values);
            sprites.Sort((s1, s2) => s2.Target.LayerOrder - s1.Target.LayerOrder);

            foreach (ItmScratchSprite sprite in sprites) {
                ctx.Source.AppendLine($"sprites.Add({sprite.ClassName}.Instance);");
            }
            ctx.Source.AppendLine("return sprites;");
            ctx.Source.PopBlock();

            // GetStage()
            ctx.Source.AppendLine();
            ctx.Source.AppendLine("public static partial IMonoScratchStage GetStage()");
            ctx.Source.PushBlock();
            ctx.Source.AppendLine($"return {ctx.Stage.ClassName}.Instance;");
            ctx.Source.PopBlock();

            // GetSettings()
            ctx.Source.AppendLine();
            ctx.Source.AppendLine("public static partial ProjectSettings GetSettings()");
            ctx.Source.PushBlock();
            
            StringBuilder settings = new StringBuilder("return new ProjectSettings(");

            settings.Append("false, "); // TurboMode
            settings.Append("true, "); // CloseWhenDone
            settings.Append("30"); // FPS

            settings.Append(");");
            
            ctx.Source.AppendLine(settings.ToString());
            ctx.Source.PopBlock();

            ctx.Source.PopBlock();
            ctx.Source.AppendLine();

            // ProjectEventListener
            ctx.Source.AppendLine("public partial interface ProjectEvents");
            ctx.Source.PushBlock();

            foreach (ItmScratchBroadcast broadcast in ctx.Broadcasts.Values) {
                ctx.Source.AppendLine($"public MonoScratchThread[] {broadcast.RunListenersName}()");
                ctx.Source.PushBlock();
                ctx.Source.AppendLine("return new MonoScratchThread[0];");
                ctx.Source.PopBlock();
            }
            ctx.Source.PopBlock();
        }
    }
}