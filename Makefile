
all: build run

include .env
export

run:
	cd Build && dotnet build -c Release
	cd Build && bin/Release/net6.0/Project

build:
	rm -rf Build
	mkdir Build Build/MonoScratch
	cd Compiler && dotnet run
	cp -r Runtime/src Build/MonoScratch/Runtime
	cp -r Share/src Build/MonoScratch/Share
	cp -r Shaders/bin Build/Shaders

shaders:
	rm -rf Runtime/shaders/bin
	mkdir Runtime/shaders/bin
	mgfxc Runtime/shaders/src/Sprite.hlsl Runtime/shaders/bin/Sprite.mgfx /Profile:OpenGL
	mgfxc Runtime/shaders/src/PenLine.hlsl Runtime/shaders/bin/PenLine.mgfx /Profile:OpenGL
	make