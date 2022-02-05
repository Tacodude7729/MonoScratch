using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class Program {

        public static void Main() {

            string sb3 = "Goals.sb3";

            string sb3ExtractPath = Path.Combine(Path.GetTempPath(), "monoscratch" + new Random().Next());

            try {
                Directory.CreateDirectory(sb3ExtractPath);
                ZipFile.ExtractToDirectory(sb3, sb3ExtractPath);

                string projectJsonPath = Path.Combine(sb3ExtractPath, "project.json");
                dynamic projectJson = JsonConvert.DeserializeObject(File.ReadAllText(projectJsonPath)) ?? throw new SystemException();

                ScratchProject project = new ScratchProject(projectJson);
                SourceGeneratorContext sourceCtx = new SourceGeneratorContext(project);

                sourceCtx.GenerateSources();
                File.WriteAllText("../Build/Project.cs", sourceCtx.Source.ToString());        

                sourceCtx.GenerateAssets(sb3ExtractPath, "../Build");
            } finally {
                Directory.Delete(sb3ExtractPath, true);
            }
        }
    }
}