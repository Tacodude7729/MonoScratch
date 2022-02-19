using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using ScratchSharp.Project;

namespace MonoScratch.Compiler {

    public static class Program {

        public static void Main() {

            string sb3 = "Project.sb3";

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

                File.WriteAllText("../Build/Project.csproj", 
@"<Project Sdk='Microsoft.NET.Sdk'>
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include='MonoGame.Framework.DesktopGL' Version='3.8.0.1641' />
  </ItemGroup>
</Project>
");
            } finally {
                Directory.Delete(sb3ExtractPath, true);
            }
        }
    }
}