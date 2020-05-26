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

* **Gameplay**
  * Add game over
  * Add score

* **Advanced Gameplay**
  * Add support for combos

* **Aesthetics**
  * Add sound effects
  * Add music
  * Add animation to cursor movement

* **Dev Experience**
  * Clean up multiple matrices used for the board (maybe into a single one?)
  * Integrate with StyleCopAnalyzers
