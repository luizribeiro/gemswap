all: build test

build:
	dotnet build gemswap

test:
	dotnet test tests

run:
	dotnet run --project gemswap/gemswap.csproj
