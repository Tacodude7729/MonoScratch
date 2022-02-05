
all: build run

include .env
export

run:
	cd Build && bin/Release/net6.0/Project

build:
	rm -rf Build
	mkdir Build
	cd Compiler && dotnet run
	cp -r Runtime/src Build/MonoScratch
	cp -r Runtime/shaders/bin Build/Shaders
	cp Runtime/Runtime.csproj Build/Project.csproj
	cd Build && dotnet build -c Release

shaders:
	rm -rf Runtime/shaders/bin
	mkdir Runtime/shaders/bin
	mgfxc Runtime/shaders/src/Sprite.hlsl Runtime/shaders/bin/Sprite.mgfx /Profile:OpenGL
	mgfxc Runtime/shaders/src/PenLine.hlsl Runtime/shaders/bin/PenLine.mgfx /Profile:OpenGL
	make