using System.Collections.Generic;

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
            foreach (ItmScratchSprite sprite in ctx.Sprites.Values) {
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

            ctx.Source.PopBlock();
            ctx.Source.AppendLine();

            // ProjectEventListener
            ctx.Source.AppendLine("public partial interface ProjectEvents");
            ctx.Source.PushBlock();
            ctx.Source.PopBlock();
        }
    }
}