# luizribeiro/gemswap

[![Build Status](https://travis-ci.com/luizribeiro/gemswap.svg?token=Y5WyECQyFrzmKkJLsCaK&branch=master)](https://travis-ci.com/luizribeiro/gemswap)
[![codecov](https://codecov.io/gh/luizribeiro/gemswap/branch/master/graph/badge.svg?token=1Y0L8M3PWE)](https://codecov.io/gh/luizribeiro/gemswap)

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

* **Advanced Gameplay**
  * Add support for combos
  * Add support for sending bricks to each other

* **Aesthetics**
  * Add animation to cursor movement
  * Add timer
  * Add effects to:
    * Eliminations
    * Game over

* **Competition**
  * Finish game for both players
  * Allow restart of the game
  * Number of victories per player

* **Simple Tweaks**
  * On gamepad, allow any button to be used for swapping

* **Dev Experience**
  * Clean up multiple matrices used for the board (maybe into a single one?)
