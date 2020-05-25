# luizribeiro/gemswap

[![Build Status](https://travis-ci.com/luizribeiro/gemswap.svg?token=Y5WyECQyFrzmKkJLsCaK&branch=master)](https://travis-ci.com/luizribeiro/gemswap)

This is my gemswap game.

### Building

```
$ nuget restore
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
  * Add basic test coverage
  * Stablize Y indices to kill bugs when new rows are added
  * Clean up multiple matrices used for the board (maybe into a single one?)
  * Fix bug where 5 piece L shaped combos are not scoring
* **Mid-pri**
  * Fix falling animation that looks wonky for whatever reason
  * Add support for two or more players
  * Add support for keyboard
  * Integrate with StyleCopAnalyzers
  * Integrate with Travis or AppVeyor
* **Low-pri**
  * Center game on the screen
  * Add animation to cursor movement
  * Prevent cursor from going above all rows
  * Delete old, inactive timers
  * Different animation duration for each action (swapping, falling,
    disappearing, etc)
