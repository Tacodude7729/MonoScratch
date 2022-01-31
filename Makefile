all: build run

run:
	cd Build && dotnet run

build:
	rm -rf Build
	mkdir Build
	cd Compiler && dotnet run
	cp -r Runtime/src Build/MonoScratch
	cp Runtime/Runtime.csproj Build/Project.csproj