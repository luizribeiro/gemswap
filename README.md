# luizribeiro/gemswap

[![Build Status](https://travis-ci.com/luizribeiro/gemswap.svg?token=Y5WyECQyFrzmKkJLsCaK&branch=master)](https://travis-ci.com/luizribeiro/gemswap)

This is my gemswap game.

### Building

```
$ cd gemswap
$ nuget restore
$ msbuild
$ dotnet run
```

### Testing

```
$ cd tests
$ dotnet test
```

### Packaging

For Windows:

```
$ dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
```

### TODOs

* **High-pri**
  * Clean up multiple matrices used for the board (maybe into a single one?)
  * Fix bug where pieces are getting eliminated while one of them is still falling
* **Mid-pri**
  * Fix falling animation that looks wonky for whatever reason
  * Add support for two or more players
  * Add support for keyboard
  * Integrate with StyleCopAnalyzers
* **Low-pri**
  * Center game on the screen
  * Add animation to cursor movement
  * Prevent cursor from going above all rows
  * Delete old, inactive timers
