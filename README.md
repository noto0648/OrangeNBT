# OrangeNBT
Library for Named Binary Tag and Minecraft world access.

Requires:
* .Net Standard 2.0

__THIS LIBRARY IS UNDER DEVELOPING.__
There's a possibility that we do destructive change if any inconvenience appears.

## OrangeNBT (Core)
You can use various nbt tags, serialize json and File IO.
## OrangeNBT.Data
Data of Minecraft.
Now, you can access minecraft block datas.

Requires OrangeNBT(Core)

## OrangeNBT.World
You can access the minecraft world data.

Requires OrangeNBT(Core), OrangeNBT.Data

This packeage includes modified [DotNetZip(Ionic Zip Library)](https://archive.codeplex.com/?p=dotnetzip).
Thanks for great developpers.

```
using OrangeNBT.Data.Anvil;
using OrangeNBT.World;
using OrangeNBT.World.Anvil;
using OrangeNBT.Data.Anvil.Helper;

var w = AnvilWorld.Load(@"C:\World");

w.Dimensions[Dimension.Overworld].Blocks.SetBlock(0, 20, 60, "minecraft:stone");
w.Dimensions[Dimension.Overworld].Blocks.SetBlock(1, 20, 60, "minecraft:oak_log", "axis", "y");

```

## OrangeNBT.BE
Coming soon...
Requires OrangeNBT(Core), OrangeNBT.Data, OrangeNBT.World

