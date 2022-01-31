using MonoScratch.Runtime;
using System.Collections.Generic;
using System;

namespace MonoScratch.Project {

    public static partial class Interface {

        public static partial List<IMonoScratchSprite> GetSprites();
        public static partial IMonoScratchStage GetStage();
    }


    public partial interface ProjectEvents {

        public void OnGreenFlag() { }   
        
    }
}