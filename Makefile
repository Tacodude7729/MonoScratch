
all: build run

export MGFXC_WINE_PATH = /home/zach/.local/share/wineprefixes/monogame

run:
	cd Build && dotnet run

build:
	rm -rf Build
	mkdir Build
	cd Compiler && dotnet run
	cp -r Runtime/src Build/MonoScratch
	cp -r Runtime/shaders/bin Build/Shaders
	cp Runtime/Runtime.csproj Build/Project.csproj

shaders:
	mgfxc Runtime/shaders/src/Test.hlsl Runtime/shaders/bin/Test.mgfx /Profile:OpenGL