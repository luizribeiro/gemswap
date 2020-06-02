all: build test

build:
	dotnet build gemswap

test:
	dotnet test tests

coverage:
	dotnet test tests /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

run:
	dotnet run --project gemswap/gemswap.csproj

clean:
	dotnet clean gemswap
	dotnet clean tests
