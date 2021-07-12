using System;
using System.Collections.Generic;
using Godot;

// This interface is used for objects that can construct IBuildable objects.
// However, at this time, PlayableUnits use a different building implementation.

public interface IConstructor {


    // Stores a list of the units that this structure can build.
    Dictionary<int, PackedScene> BuildableItems { get; set; }

    // Stores the builder's team.
    int TeamID { get; set; }


    // Takes an IBuildable as a PackedScene and adds it's it to the item's construction queue
    void Build(PackedScene item);




}