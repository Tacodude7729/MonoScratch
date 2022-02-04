using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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

        public bool PenDown { get; set; }
        public double PenSize { get; set; }
        public Color PenColor { get; set; }
    }

    public abstract class MonoScratchSprite<T> : MonoScratchTarget<T>, IMonoScratchSprite where T : MonoScratchSprite<T>, IMonoScratchSprite, new() {
        public static List<T> Clones;

        static MonoScratchSprite() {
            Clones = new List<T>();
        }

        public bool Visible { get; set; }

        private double _x, _y;
        public double Size { get; set; }
        public double Direction { get; set; }

        public TargetLinkedList.Node? SpriteListNode { get; set; }

        private bool _penDown;
        private double _penSize;
        public Color PenColor { get; set; }

        private void OnMoved(double x, double y) {
            if (_x != x || _y != y) { // TODO Fencing
                if (Visible) Program.Runtime.RedrawRequested = true;
                if (PenDown) {
                    Program.Runtime.Renderer.PenDrawLine(_x, _y, x, y, _penSize, PenColor);
                }
                _x = x;
                _y = y;
            }
        }

        public void SetXY(double x, double y) {
            OnMoved(x, y);
        }

        public double X {
            get => _x; set {
                OnMoved(value, _y);
            }
        }
        public double Y {
            get => _y; set {
                OnMoved(_x, value);
            }
        }

        public override int RenderX => (int)Math.Round(_x);
        public override int RenderY => (int)Math.Round(_y);
        public override int RenderRotation => (int)Math.Round(Direction);
        public override float RenderScale => (float)(Size / 100);
        public override bool RenderVisible => Visible;

        public bool PenDown {
            get => _penDown; set {
                if (value) { // Pen Down
                    Program.Runtime.Renderer.PenDrawLine(_x, _y, _x, _y, _penSize, PenColor);
                    _penDown = true;
                } else { // Pen Up
                    _penDown = true;
                }
            }
        }
        public double PenSize { get => _penSize; set { _penSize = value; } }
    }
}