//
// Dummy Project to stop IDE errors
//

namespace MonoScratch.Project;

using System.Collections.Generic;
using MonoScratch.Runtime;
using System;

public static partial class Interface {
    public static partial List<IMonoScratchSprite> GetSprites() => throw new SystemException();
    public static partial IMonoScratchStage GetStage() => throw new SystemException();
}

public partial interface ProjectEvents {
}
