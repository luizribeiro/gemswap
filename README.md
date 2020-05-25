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

### TODOs

* **High-pri**
  * Stablize Y indices to kill bugs when new rows are added
  * Clean up multiple matrices used for the board (maybe into a single one?)
  * Fix bug where 5 piece L shaped combos are not scoring
* **Mid-pri**
  * Fix falling animation that looks wonky for whatever reason
* **Low-pri**
  * Center game on the screen
