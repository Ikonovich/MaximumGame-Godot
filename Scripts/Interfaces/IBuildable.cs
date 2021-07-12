using System;
using Godot;


public interface IBuildable {


    // The name of the object
    string Name { get; set; }
    // Who the object belongs to
    int TeamID { get; set; }
    // Stores the cost in resources of this object.
    int EnergyCost { get; set; }
    int MetalCost { get; set; }
    int CrystalCost { get ; set; }


    	// Stores the preview scene camera's offsets for this particular item.
	// Used to adjust the camera for an item in the inventory menus.
	
    float PreviewXoffset { get; set; }

	float PreviewYoffset { get; set; }

    float PreviewZoffset { get; set; }

    
    // Used to determine an offset over the terrain when this item is placed.
    // Usually used to drop units from the sky. 
    float TerrainYoffset { get; set; }

    bool IsPreviewScene { get; set; }
    
}