using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoScratch.Runtime {

    public interface IMonoScratchStage : IMonoScratchTarget {
    }

    public abstract class MonoScratchStage<T> : MonoScratchTarget<T>, IMonoScratchStage where T : MonoScratchStage<T>, IMonoScratchStage, new() {

        public override int RenderX => 0;
        public override int RenderY => 0;
        public override int RenderRotation => 90;
        public override float RenderScale => 1;

    }
}