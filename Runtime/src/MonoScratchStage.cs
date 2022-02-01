using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoScratch.Runtime {

    public interface IMonoScratchStage : IMonoScratchTarget {
        public void DrawStage(MonoScratchRenderer renderer);
    }

    public abstract class MonoScratchStage<T> : MonoScratchTarget<T>, IMonoScratchStage where T : MonoScratchStage<T>, IMonoScratchStage, new() {

        public void DrawStage(MonoScratchRenderer renderer) {
            renderer.RenderCostume(CurrentCostume, 0, 0, 90, 100);
        }

    }
}