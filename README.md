# AutonomousMovement2D-Unity
A 2D autonomous movement library for Unity 3D using Steering Behaviours

The project is still in early infancy so there are bugs to sort out before this is usable.

## Known Issues

- Every instance accessing World2D.Instance throws a NullReferenceException when the player is
  stopped.  
- Prioritized Dithering force calculation results in erratic behaviour.

## TODO

- Time Slicing force calculation.
- Improve performance.

## Assets used in the project

- 2D Toolkit (not included)
- MoreLinq (not included)
- MiscUtil (not included)
- Unity Test Tools
- Kenney Public Domain Sprites