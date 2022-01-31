using System.Collections.Generic;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public class ItmScratchVariable {
        public readonly string ID;
        public readonly string Name;
        public readonly string Value;

        public readonly ItmScratchTarget Target;
        public readonly string CodeName;

        public ItmScratchVariable(ItmScratchTarget target, SourceGeneratorContext ctx, ScratchVariable variable) {
            Target = target;
            ID = variable.ID;
            Name = variable.Name;
            Value = variable.Value;
            CodeName = ctx.GetNextSymbol(Name);
        }

        public string GetCode(SourceGeneratorContext ctx) {
            if (ctx.CurrentTarget == Target) {
                return CodeName;
            } else {
                return Target.ClassName + ".Instance." + CodeName;
            }
        }

        public string GetCodeNumber(SourceGeneratorContext ctx) {
            return GetCode(ctx) + ".AsNumber()";
        }

        public string GetCodeString(SourceGeneratorContext ctx) {
            return GetCode(ctx) + ".AsString()";
        }
    }

    public class ItmScratchList {
        public readonly string ID;
        public readonly string Name;
        public readonly List<string> Values;

        public readonly ItmScratchTarget Target;
        public readonly string CodeName;

        public ItmScratchList(ItmScratchTarget target, SourceGeneratorContext ctx, ScratchList variable) {
            Target = target;
            ID = variable.ID;
            Name = variable.Name;
            Values = variable.Values;
            CodeName = ctx.GetNextSymbol(Name);
        }

        public string GetCode(SourceGeneratorContext ctx) {
            if (ctx.CurrentTarget == Target) {
                return CodeName;
            } else {
                return Target.ClassName + ".Instance." + CodeName;
            }
        }
    }
}