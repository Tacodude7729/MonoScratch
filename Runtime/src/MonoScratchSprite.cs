using System;
using System.Collections.Generic;
using MonoScratch.Project;

namespace MonoScratch.Runtime {

    public interface IMonoScratchSprite : IMonoScratchTarget {

        public double X { get; set; }
        public double Y { get; set; }

        public bool Visible { get; set; }

        public double Size { get; set; }
        public double Direction { get; set; }
        // public bool Draggable; TODO
        // public string RotationStyle; TODO

        public TargetLinkedList.Node? SpriteListNode { get; set; }
    }

    public abstract class MonoScratchSprite<T> : MonoScratchTarget<T>, IMonoScratchSprite where T : MonoScratchSprite<T>, IMonoScratchSprite, new() {
        public static List<T> Clones;

        static MonoScratchSprite() {
            Clones = new List<T>();
        }

        public bool Visible { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
        public double Direction { get; set; }

        public override int RenderX => (int) Math.Round(X);
        public override int RenderY => (int) Math.Round(Y);
        public override int RenderRotation => (int) Math.Round(Direction);
        public override float RenderScale => (float) (Size / 100);

        public TargetLinkedList.Node? SpriteListNode { get; set; }
    }
}