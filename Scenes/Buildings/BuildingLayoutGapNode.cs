using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace ProjectPlantris.Scenes.Buildings;


[Tool, GlobalClass]
public partial class BuildingLayoutGapNode : Resource
{
    [Export] public int X { get; set; }
    [Export] public int Y { get; set; }

    public override bool Equals(object? obj)
    {
        return false;
    }
    
    public bool Equals(BuildingLayoutGapNode? other) => other != null && X == other.X && Y == other.Y;
    public bool Equals(Plot other) => X == other.X && Y == other.Y;

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}