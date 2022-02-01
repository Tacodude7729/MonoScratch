using System;
using System.Collections.Generic;
using MonoScratch.Project;

namespace MonoScratch.Runtime {

    public interface IMonoScratchSprite : IMonoScratchTarget {

        public int X { get; set; }
        public int Y { get; set; }

        public bool Visible { get; set; }

        public int Size { get; set; }
        public int Direction { get; set; }
        // public bool Draggable; TODO
        // public string RotationStyle; TODO

        public void DrawSprite(MonoScratchRenderer renderer);
    }

    public abstract class MonoScratchSprite<T> : MonoScratchTarget<T>, IMonoScratchSprite where T : MonoScratchSprite<T>, IMonoScratchSprite, new() {
        public static List<T> Clones;

        static MonoScratchSprite() {
            Clones = new List<T>();
        }
        
        public int X { get; set; }
        public int Y { get; set; }

        public bool Visible { get; set; }

        public int Size { get; set; }
        public int Direction { get; set; }

        public void DrawSprite(MonoScratchRenderer renderer) {
            renderer.RenderCostume(CurrentCostume, 0, 0, 90, 100);
        }
    }
}