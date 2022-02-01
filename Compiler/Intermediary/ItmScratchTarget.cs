using System.Collections.Generic;
using ScratchSharp.Project;
using System;
using System.IO;
using System.Text;

namespace MonoScratch.Compiler {

    public abstract class ItmScratchTarget {

        public readonly ScratchTarget Target;
        public readonly string ClassName;

        public readonly Dictionary<string, ItmScratchVariable> Variables;
        public readonly Dictionary<string, ItmScratchList> Lists;

        public readonly Dictionary<string, ItmScratchBlock> Blocks;
        public readonly Dictionary<string, List<ItmScratchHatBlock>> HatBlockMap;
        public readonly Dictionary<string, ProcedureBlocks.DefinitionBlock> Procedures;

        public ItmScratchTarget(SourceGeneratorContext ctx, ScratchTarget target, string className) {
            Target = target;
            ClassName = className;

            Variables = new Dictionary<string, ItmScratchVariable>();
            foreach (KeyValuePair<string, ScratchVariable> variable in target.Variables) {
                Variables.Add(variable.Key, new ItmScratchVariable(this, ctx, variable.Value));
            }

            Lists = new Dictionary<string, ItmScratchList>();
            foreach (KeyValuePair<string, ScratchList> list in target.Lists) {
                Lists.Add(list.Key, new ItmScratchList(this, ctx, list.Value));
            }

            Blocks = new Dictionary<string, ItmScratchBlock>();
            HatBlockMap = new Dictionary<string, List<ItmScratchHatBlock>>();
            Procedures = new Dictionary<string, ProcedureBlocks.DefinitionBlock>();

            ctx.CurrentTarget = this;
            foreach (ScratchBlock block in Target.Blocks.Values) {
                ItmScratchBlock? itmBlock = ScratchBlocks.Create(ctx, block);
                if (itmBlock == null) {
                    Console.WriteLine($"Unknown block {block.Opcode}. Skipping...");
                    continue;
                }
                Blocks.Add(block.ID, itmBlock);
                if (itmBlock is ItmScratchHatBlock hat) {
                    HatBlockMap.TryGetValue(hat.ListenerMethodName, out List<ItmScratchHatBlock>? hats);
                    if (hats == null) {
                        hats = new List<ItmScratchHatBlock>();
                        HatBlockMap.Add(hat.ListenerMethodName, hats);
                    }
                    hats.Add(hat);
                } else if (itmBlock is ProcedureBlocks.DefinitionBlock procDef) {
                    Procedures[procDef.Proccode] = procDef;
                }
            }
            ctx.CurrentTarget = null;
        }

        public void AppendSource(SourceGeneratorContext ctx) {
            ctx.CurrentTarget = this;
            AppendSourceClassHeader(ctx);
            ctx.Source.PushBlock();

            // Variable Declarations
            foreach (ItmScratchVariable variable in Variables.Values) {
                ctx.Source.AppendLine($"// {variable.Name}");
                ctx.Source.AppendLine($"public MonoScratchValue {variable.CodeName};");
            }
            ctx.Source.AppendLine();

            foreach (ItmScratchList list in Lists.Values) {
                ctx.Source.AppendLine($"// {list.Name}");
                ctx.Source.AppendLine($"public MonoScratchList {list.CodeName};");
            }
            ctx.Source.AppendLine();

            // Constructor
            ctx.Source.AppendLine($"public {ClassName}() : base()");
            ctx.Source.PushBlock();
            ctx.Source.AppendLine($"CurrentCostumeIdx = {Target.CurrentCostume};");
            ctx.Source.AppendLine();
            foreach (ItmScratchVariable variable in Variables.Values) {
                ctx.Source.AppendLine($"{variable.GetCode(ctx)} = new MonoScratchValue({BlockUtils.StringToNumString(variable.Value)});");
            }
            foreach (ItmScratchList list in Lists.Values) {
                ctx.Source.AppendLine();
                string listCode = list.GetCode(ctx);
                ctx.Source.AppendLine($"{listCode} = new MonoScratchList();");
                foreach (string value in list.Values) {
                    ctx.Source.AppendLine($"{listCode}.Add(new MonoScratchValue({BlockUtils.StringToNumString(value)}));");
                }
            }
            ctx.Source.AppendLine();
            AppendToDefaultConstructor(ctx);
            ctx.Source.PopBlock();

            ctx.Source.AppendLine();

            // Assets
            ctx.Source.AppendLine($"private static MonoScratchTargetAssets _assets = new MonoScratchTargetAssets(");
            ctx.Source.PushIndent();
            ctx.Source.AppendLine("new MonoScratchCostume[] {");
            ctx.Source.PushIndent();
            foreach (ScratchCostume costume in Target.Costumes) {
                ctx.Source.AppendLine($"new MonoScratchCostume(\"{Path.Combine(ClassName, costume.MD5Ext)}\", {costume.RotationCenterX}, {costume.RotationCenterY}, {costume.BitmapResolution ?? 1}),");
            }
            ctx.Source.PopIndent();
            ctx.Source.AppendLine("}");
            ctx.Source.PopIndent();
            ctx.Source.AppendLine(");");
            ctx.Source.AppendLine($"public override MonoScratchTargetAssets Assets => _assets;");
            ctx.Source.AppendLine();

            // Event Hat Blocks
            foreach (KeyValuePair<string, List<ItmScratchHatBlock>> hats in HatBlockMap) {
                ItmScratchHatBlock firstHat = hats.Value[0];

                firstHat.AppendListenerMethodHeader(ctx);
                ctx.Source.PushBlock();
                foreach (ItmScratchHatBlock hat in hats.Value) { // TODO Order
                    ctx.Source.AppendLine($"Utils.StartThread({hat.MethodName});");
                }
                ctx.Source.PopBlock();
                ctx.Source.AppendLine();

                foreach (ItmScratchHatBlock hat in hats.Value) {
                    if (hat.Block.NextID != null) {
                        hat.AppendMethodHeader(ctx);
                        ctx.Source.PushBlock();
                        ctx.AppendBlocks(hat.Block.NextID);
                        ctx.Source.AppendLine("yield break;");
                        ctx.Source.PopBlock();
                        ctx.Source.AppendLine();
                    }
                }
            }
            ctx.Source.AppendLine();

            // Custom Blocks
            foreach (ProcedureBlocks.DefinitionBlock procedure in Procedures.Values) {
                StringBuilder line = new StringBuilder($"public IEnumerable<YieldReason> {procedure.MethodName}(");
                int i = 0;
                foreach (ProcedureBlocks.ProcedureArgument argument in procedure.ArgumentIdMap.Values) {
                    if (argument.ArgType == ProcedureBlocks.ProcedureArgumentType.VALUE) {
                        line.Append("MonoScratchValue ");
                    } else {
                        line.Append("bool ");
                    }
                    line.Append(argument.CodeName);

                    if (i != procedure.ArgumentIdMap.Count - 1) {
                        line.Append(", ");
                    }
                    ++i;
                }
                line.Append(")");

                ctx.Source.AppendLine(line.ToString());
                ctx.Source.PushBlock();
                ctx.CurrentProcedure = procedure;
                if (procedure.Block.NextID != null)
                    ctx.AppendBlocks(procedure.Block.NextID);
                ctx.CurrentProcedure = null;
                ctx.Source.AppendLine("yield break;");
                ctx.Source.PopBlock();
                ctx.Source.AppendLine();
            }

            ctx.Source.PopBlock();
            ctx.Source.AppendLine();
            ctx.CurrentTarget = null;
        }

        protected abstract void AppendSourceClassHeader(SourceGeneratorContext ctx);

        protected virtual void AppendToDefaultConstructor(SourceGeneratorContext ctx) { }

        public ItmScratchVariable? GetVariable(string ID) {
            Variables.TryGetValue(ID, out ItmScratchVariable? var);
            return var;
        }

        public ItmScratchList? GetList(string ID) {
            Lists.TryGetValue(ID, out ItmScratchList? list);
            return list;
        }
    }

    public class ItmScratchStage : ItmScratchTarget {

        public readonly ScratchStage Stage;

        public ItmScratchStage(SourceGeneratorContext ctx, ScratchStage stage) : base(ctx, stage, ctx.GetNextSymbol("Stage")) {
            Stage = stage;
        }

        protected override void AppendSourceClassHeader(SourceGeneratorContext ctx) {
            ctx.Source.AppendLine($"// The Stage");
            ctx.Source.AppendLine($"public class {ClassName} : MonoScratchStage<{ClassName}>, ProjectEvents");
        }
    }

    public class ItmScratchSprite : ItmScratchTarget {

        public readonly ScratchSprite Sprite;

        public ItmScratchSprite(SourceGeneratorContext ctx, ScratchSprite sprite) : base(ctx, sprite, ctx.GetNextSymbol(sprite.Name)) {
            Sprite = sprite;
        }

        protected override void AppendSourceClassHeader(SourceGeneratorContext ctx) {
            ctx.Source.AppendLine($"// {Sprite.Name}");
            ctx.Source.AppendLine($"public class {ClassName} : MonoScratchSprite<{ClassName}>, ProjectEvents");
        }

        protected override void AppendToDefaultConstructor(SourceGeneratorContext ctx) {
            ctx.Source.AppendLine($"X = {Sprite.X};");
            ctx.Source.AppendLine($"Y = {Sprite.Y};");
            ctx.Source.AppendLine($"Visible = {(Sprite.Visible ? "true" : "false")};");
            ctx.Source.AppendLine($"Size = {Sprite.Size};");
            ctx.Source.AppendLine($"Direction = {Sprite.Direction};");
        }
    }
}