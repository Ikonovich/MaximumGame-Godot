Thank you for the purchase! Hope the assets work well.
If they don't, please contact me: slsovest@gmail.com.



Importing into Unreal:

After import you'll need to:
-In skeletal meshes: add sockets to the bones named "Mount_..."
-In all meshes: all sockets: rotate X = -90
-for the sockets named "Mount_..._R": scale X = -1
 


Assembling the turrets:


Bases, shoulders and cockpits contain containers for mounting other parts, their names start with "Mount_".
Just drop the part in the corresponding container, and sero out its transformations.

- Start with Base. 
- The first container inside is "Mount_top".
- Put the "Base_Top_Mount" (or Walls, and then "Base_Top_Mount") inside it, find inside another "Mount_Top" container.
- Put the Shoulders or the Cockpit into "Mount_top".
- Find other containers inside shoulders and cockpit.

All weapons contain locators at their barrel ends (named "Barrel_end"). Rocket launchers contain multiple locators, for all of the rockets.



The texture PSD:


The source .PSD can be found in the "Materials" folder.
For a quick repaint, adjust the layers in the "COLOR" folder. You can drop your decals and textures (camouflage, for example) in the folder as well. Just be careful with texture seams.
You may want to turn off the "FX_Rust" and "FX_Chipped_paint" layers for more cartoony look.
Or make ambient occlusion stronger by increasing opacity of "SHADING/MORE_OCCLUSION" layer.