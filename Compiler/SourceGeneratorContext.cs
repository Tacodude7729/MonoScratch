using ScratchSharp.Project;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MonoScratch.Compiler {

    public class SourceGeneratorContext {

        public readonly SourceGenerator Source;
        public readonly ScratchProject Project;

        public ItmScratchTarget? CurrentTarget;
        public ProcedureBlocks.DefinitionBlock? CurrentProcedure;

        public readonly ItmScratchStage Stage;
        public readonly Dictionary<string, ItmScratchSprite> Sprites;

        public IEnumerable<ItmScratchTarget> Targets {
            get {
                yield return Stage;
                foreach (ItmScratchSprite sprite in Sprites.Values)
                    yield return sprite;
            }
        }

        public readonly Dictionary<string, int> SourceSymbols;

        public SourceGeneratorContext(ScratchProject project) {
            Source = new SourceGenerator();
            Project = project;

            SourceSymbols = new Dictionary<string, int>();

            Sprites = new Dictionary<string, ItmScratchSprite>();
            Stage = new ItmScratchStage(this, project.Stage);
            foreach (ScratchSprite sprite in Project.Sprites) {
                Sprites.Add(sprite.Name, new ItmScratchSprite(this, sprite));
            }
        }

        public void GenerateSources() {
            foreach (ItmScratchTarget target in Targets) {
                target.AppendSource(this);
            }
            InterfaceGenerator.AppendSource(this);
        }

        public void GenerateAssets(string extract, string path) {
            string assetsFolder = Path.Combine(path, "Assets");
            Directory.CreateDirectory(assetsFolder);

            foreach (ItmScratchTarget target in Targets) {
                string targetFolder = Path.Combine(assetsFolder, target.ClassName);
                Directory.CreateDirectory(targetFolder);
                foreach (ScratchCostume costume in target.Target.Costumes) {
                    File.Move(Path.Combine(extract, costume.MD5Ext), Path.Combine(targetFolder, costume.MD5Ext));
                }
            }
        }

        public ItmScratchVariable? GetVariable(string? ID) {
            if (ID == null)
                return null;
            ItmScratchVariable? var = Stage.GetVariable(ID);
            if (var == null) {
                var = CurrentTarget?.GetVariable(ID);
            }
            return var;
        }

        public ItmScratchVariable GetVariable(BlockField field) {
            return GetVariable(field.ID) ?? throw new SystemException($"Couldn't find variable {field.Name}.");
        }

        public string GetNextSymbol(string rawName, bool fixCapitols = true) {
            StringBuilder sb = new StringBuilder();
            bool whiteSpace = true;
            for (int i = 0; i < rawName.Length; i++) {
                char c = rawName[i];
                if (char.IsLetterOrDigit(c)) {
                    if (whiteSpace) {
                        if (fixCapitols)
                            c = char.ToUpper(c);
                        whiteSpace = false;
                    }
                    sb.Append(c);
                    if (char.IsDigit(c))
                        whiteSpace = true;
                } else if (char.IsWhiteSpace(c))
                    whiteSpace = true;
            }

            string name = sb.ToString();
            if (char.IsDigit(name[name.Length - 1])) name = name + "_";

            if (SourceSymbols.ContainsKey(name)) {
                int count = SourceSymbols[name] = ++SourceSymbols[name];
                return name + (count - 1);
            } else {
                SourceSymbols[name] = 0;
                return name;
            }
        }

        public void AppendBlocks(string id) {
            string? blockID = id;
            do {
                if (CurrentTarget?.Blocks.TryGetValue(blockID, out ItmScratchBlock? block) ?? false) {
                    block.AppendExecute(this);
                    blockID = block.Block.NextID;
                } else {
                    Console.WriteLine($"Couldn't find block {blockID}.");
                    break;
                }
            } while (blockID != null);
        }

    }
}