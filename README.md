# Hex Grid System

This is a library (& demo) for smoothed block tiles on a 3D hexagonal grid.  Like Minecraft, except hexagonal prims and smoothed blocks.

This library is used in my released game [Aquifer Ascent](https://github.com/uncannyforest/Aquifer-Ascent).

## Features 

- Set surface elevation level or manually set solid/air blocks at any point above or below surface
- Several different grids with different materials/shapes can be used simultaneously (set the block to a given material)
  - 3 different sets of 3D tiles are included (one is more blocky for artificial-type structures) and custom ones can be added
- Custom shader which detects face orientation to set upward-facing, side-facing, and downward-facing materials separately
- Optional first-person terraformer prefab (cursor in the middle of the screen like Minecraft)

## Implementation

Uses a [dual-grid system](https://x.com/OskSta/status/1448248658865049605) to make the surface look smooth.

## Unity version

This project is in 2020.3.48f1, but assets should be copyable to other Unity versions with minimal effort.

## Usage

Copy this into your project.  Terraforming directory is optional (it's the first-person terraformer), you can write custom terraforming code without it.

The demo scene generates terrain live using a random walk algorithm.

Exactly one game object should contain the WorldGrid component, which uses a Singleton pattern.  Set block types by

1. Creating a GridMod object: `GridMod mod = new GridMod(GridPos pos, Block materialType, int height = 1)`

- `GridPos` constructor is `new GridPos(int elevation, int x, int y)` — note that `x` and `y` axes are 120 degrees from each other.
- `Block` is an enum including AIR.  New non-AIR block types can be added by creating new MaterialShape ScriptableObjects
- `height` is used to set a column of blocks at once.

2. Commit the change with `mod.Commit()`
