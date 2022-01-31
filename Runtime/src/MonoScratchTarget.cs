
using MonoScratch.Project;

namespace MonoScratch.Runtime {

    public interface IMonoScratchTarget : ProjectEvents {
        public MonoScratchTargetAssets Assets { get; }

        public int CurrentCostumeIdx { get; set; }
        public MonoScratchCostume CurrentCostume { get; }
    }

    public abstract class MonoScratchTarget<T> : IMonoScratchTarget where T : MonoScratchTarget<T>, IMonoScratchTarget, new() {
        public static T Instance;

        public int CurrentCostumeIdx { get; set; }

        static MonoScratchTarget() {
            Instance = new T();
        }

        public MonoScratchCostume CurrentCostume => Assets.Costumes[CurrentCostumeIdx];
        
        public abstract MonoScratchTargetAssets Assets { get; }
    }
}