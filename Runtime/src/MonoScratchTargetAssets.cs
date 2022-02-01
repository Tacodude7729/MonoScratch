using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System;

namespace MonoScratch.Runtime {

    public class MonoScratchTargetAssets {
        public readonly MonoScratchCostume[] Costumes;

        public MonoScratchTargetAssets(MonoScratchCostume[] costumes) {
            Costumes = costumes;
        }

        public void Load() {
            foreach (MonoScratchCostume costume in Costumes)
                costume.Load();
        }
    }

    public class MonoScratchCostume {
        public Texture2D? Texture;
        public Vector2 RotationCenter;
        public int BitmapResolution;


        public readonly string FilePath;

        public MonoScratchCostume(string path, int rx, int ry, int res) {
            FilePath = Path.Combine("Assets", path);
            RotationCenter = new Vector2(rx, ry);
            BitmapResolution = res;
        }

        public void Load() {
            Texture = Texture2D.FromFile(Program.Runtime.GraphicsDevice, FilePath);
        }
    }
}