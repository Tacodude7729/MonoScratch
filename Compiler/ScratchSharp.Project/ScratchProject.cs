using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;

namespace ScratchSharp.Project {

    public class ScratchProject {
        private static readonly Random _random = new Random();

        public string MetaSemver;
        public string MetaVm;
        public string MetaUserAgent;

        public dynamic JsonMonitors; // TODO Parse this

        public readonly List<string> Extensions;

        public readonly List<ScratchSprite> Sprites;
        public ScratchStage Stage;

        public IEnumerable<ScratchTarget> GetTargets() {
            yield return Stage;
            foreach (ScratchSprite sprite in Sprites) yield return sprite;
        }

        public ScratchProject() {
            MetaSemver = "3.0.0";
            MetaVm = "0.2.0";
            MetaUserAgent = "ScratchSharp/1.0";
            JsonMonitors = new JArray();
            Extensions = new List<string>();
            Sprites = new List<ScratchSprite>();
            
            Stage = new ScratchStage(this);
        }

        public ScratchProject(dynamic json) {
            MetaSemver = json.meta.semver;
            MetaVm = json.meta.vm;
            MetaUserAgent = json.meta.agent;
            Extensions = new List<string>();
            foreach (dynamic ext in json.extensions) Extensions.Add((string)ext);
            JsonMonitors = json.monitors;
            Sprites = new List<ScratchSprite>();
            foreach (dynamic targetJson in json.targets) {
                ScratchTarget target = ScratchTarget.Parse(targetJson, this);
                if (target is ScratchStage stage) {
                    if (Stage != null) throw new SystemException("Project contains two stages!");
                    Stage = stage;
                } else {
                    Sprites.Add((ScratchSprite)target);
                }
            }
            if (Stage == null) throw new SystemException("Project dosn't contain a stage!");
        }

        public delegate Stream ScratchAssetLoader(ScratchTarget target, ScratchAsset asset);

        public void WriteSb3(string outLocation, ScratchAssetLoader loader) {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "scratchsharp" + _random.Next());
            try {
                Directory.CreateDirectory(tempDirectory);

                File.WriteAllText(Path.Combine(tempDirectory, "project.json"), SerializeJson());
                WriteTargetAssets(tempDirectory, Stage, loader);
                foreach (ScratchSprite sprite in Sprites) WriteTargetAssets(tempDirectory, sprite, loader);

                ZipFile.CreateFromDirectory(tempDirectory, outLocation);
            } finally {
                if (Directory.Exists(tempDirectory)) Directory.Delete(tempDirectory, true);
            }
        }

        private void WriteTargetAssets(string path, ScratchTarget target, ScratchAssetLoader loader) {
            foreach (ScratchCostume costume in target.Costumes) WriteAsset(path, target, costume, loader);
            foreach (ScratchSound sound in target.Sounds) WriteAsset(path, target, sound, loader);
        }

        private void WriteAsset(string path, ScratchTarget target, ScratchAsset asset, ScratchAssetLoader loader) {
            string assetPath = Path.Combine(path, asset.MD5Ext);
            if (!File.Exists(assetPath)) {
                FileStream file = new FileStream(assetPath, FileMode.CreateNew, FileAccess.Write);
                Stream assetStream = loader(target, asset);
                assetStream.CopyTo(file);
                file.Close();
                assetStream.Close();
            }
        }

        public string SerializeJson() {
            return JsonConvert.SerializeObject(Serialize());
        }

        public dynamic Serialize() {
            dynamic meta = new JObject();

            meta.semver = MetaSemver;
            meta.vm = MetaVm;
            meta.agent = MetaUserAgent;

            JArray targets = new JArray();

            targets.Add(Stage.Serialize());
            foreach (ScratchSprite sprite in Sprites)
                targets.Add(sprite.Serialize());

            dynamic project = new JObject();

            project.meta = meta;
            project.monitors = JsonMonitors;
            project.exentsions = new JArray(Extensions);
            project.targets = targets;

            return project;
        }
    }
}