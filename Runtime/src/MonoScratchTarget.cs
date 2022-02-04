
using MonoScratch.Project;

namespace MonoScratch.Runtime {

    public interface IMonoScratchTarget : ProjectEvents {
        public MonoScratchTargetAssets Assets { get; }

        public TargetLinkedList.Node? LayerNode { get; set; }

        public int CurrentCostumeIdx { get; set; }
        public MonoScratchCostume CurrentCostume { get; }

        public int RenderX { get; }
        public int RenderY { get; }
        public int RenderRotation { get; }
        public float RenderScale { get; }
        public bool RenderVisible { get; }
    }

    public abstract class MonoScratchTarget<T> : IMonoScratchTarget where T : MonoScratchTarget<T>, IMonoScratchTarget, new() {
        public static T Instance;

        public int CurrentCostumeIdx { get; set; }

        public abstract int RenderX { get; }
        public abstract int RenderY { get; }
        public abstract int RenderRotation { get; }
        public abstract float RenderScale { get; }

        static MonoScratchTarget() {
            Instance = new T();
        }

        public MonoScratchCostume CurrentCostume => Assets.Costumes[CurrentCostumeIdx];

        public abstract MonoScratchTargetAssets Assets { get; }
        public TargetLinkedList.Node? LayerNode { get; set; }
        public abstract bool RenderVisible { get; }
    }
}