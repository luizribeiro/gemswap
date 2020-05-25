# luizribeiro/gemswap

This is my gemswap game.

### Building

```
$ msbuild
$ dotnet run
```

### Packaging

For Windows:

```
$ dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
```
