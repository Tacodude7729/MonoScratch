using ScratchSharp.Project;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.CSharp;

using System.CodeDom.Compiler; // TODO Use this to compile output automagically
using static MonoScratch.Compiler.EventBlocks;

namespace MonoScratch.Compiler {

    public class SourceGeneratorContext {

        public readonly SourceGenerator Source;
        public readonly ScratchProject Project;

        public ItmScratchTarget? CurrentTarget;
        public ProcedureBlocks.DefinitionBlock? CurrentProcedure;

        public bool ScreenRefresh => CurrentProcedure?.ScreenRefresh ?? true;
        public bool IsInSprite => CurrentTarget is ItmScratchSprite;
        public bool IsInStage => CurrentTarget is ItmScratchStage;
        public bool CanYield => CurrentProcedure?.YieldsThread ?? true;

        public readonly ItmScratchStage Stage;
        public readonly Dictionary<string, ItmScratchSprite> Sprites;
        public readonly Dictionary<string, ItmScratchBroadcast> Broadcasts;

        private readonly CSharpCodeProvider _codeProvider;

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

            _codeProvider = new CSharpCodeProvider();

            SourceSymbols = new Dictionary<string, int>();

            Sprites = new Dictionary<string, ItmScratchSprite>();
            Broadcasts = new Dictionary<string, ItmScratchBroadcast>();

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

        public ItmScratchList? GetList(string? ID) {
            if (ID == null)
                return null;
            ItmScratchList? var = Stage.GetList(ID);
            if (var == null) {
                var = CurrentTarget?.GetList(ID);
            }
            return var;
        }

        public ItmScratchList GetList(BlockField field) {
            return GetList(field.ID) ?? throw new SystemException($"Couldn't find list {field.Name}.");
        }

        public ItmScratchBroadcast GetOrCreateBroadcast(BlockField broadcastField) {
            if (broadcastField.ID == null) throw new SystemException("No ID on broadcast field!");
            if (Broadcasts.TryGetValue(broadcastField.ID, out ItmScratchBroadcast? value))
                return value;
            return Broadcasts[broadcastField.ID] = new ItmScratchBroadcast(this, broadcastField.Name, broadcastField.ID);
        }


        public ItmScratchBroadcast GetOrCreateBroadcast(BlockInput broadcastInput) {
            return GetOrCreateBroadcast((broadcastInput.Block as BlockInputPrimitiveBroadcast)
                ?? throw new SystemException("Expected broadcast input, got "+broadcastInput));
        }

        public ItmScratchBroadcast GetOrCreateBroadcast(BlockInputPrimitiveBroadcast broadcastInput) {
            if (Broadcasts.TryGetValue(broadcastInput.ID, out ItmScratchBroadcast? value))
                return value;
            return Broadcasts[broadcastInput.ID] = new ItmScratchBroadcast(this, broadcastInput.Name, broadcastInput.ID);
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

                // Prevent keyword names
                if (!_codeProvider.IsValidIdentifier(name)) {
                    return "@" + name;
                }

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

        public void AppendSoftYield() {
            if (ScreenRefresh)
                Source.AppendLine("yield return YieldReason.YIELD;");
        }
    }
}