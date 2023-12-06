# Engine & Game using Monogame

## Projects in solution

`Adventure.Content.Pipeline` contains custom content importers and processors for the content pipeline
`Adventure.Content` contains the contracts for game content and game content readers
`Adventure` contains the game code
`Engine` contains the engine code

## Running for the first time

The game's `Content.mgcb` file has a dependency on the `Adventure.Content.Pipeline.dll`. The `Adventure.Content.Pipeline` needs to be built in `Release` mode before attempting to build the rest of the solution.

## External tools

The game's levels are built using **LDtk**. (https://ldtk.io/)
- The `world.ldtk` project file is located at `Adventure/Content/Levels/world.ldtk`
 
