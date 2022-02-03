using MonoScratch.Runtime;
using System.Collections.Generic;
using System;

namespace MonoScratch.Project {

    public record ProjectSettings(bool TurboMode, int FPS);

    public static partial class Interface {

        public static partial List<IMonoScratchSprite> GetSprites();
        public static partial IMonoScratchStage GetStage();
        public static partial ProjectSettings GetSettings();
        
    }


    public partial interface ProjectEvents {

        public void OnGreenFlag() { }   
        
    }
}