Hello! Thank you for the purchase. Hope the assets work well.
If they don't, please contact me slsovest@gmail.com.
If you'll need a .unitypackage, just mail me.




Assembling the robots:


The pack comes as a set of parts to assemble multiple variations of robots 
(and only a few of assembled robots in the "Big_scene" file). Only the robots' legs (and some other parts, like weapons) are animated, 
all the other parts you'll have to animate yourself or control programmaticaly.
Legs, chassis, shoulders and cockpits contain containers for mounting other parts, their names start with "Mount_".
(In some cases they may be deep in bone hierarchy).
Drop the appropriate part inside, and and make sure the part's pivot is at the "Mount" pivot.

- Start with legs. 
- The first container is in Legs->ROOT->Pelvis->Top->Mount_top. 
- Put the shoulders or the cockpit into "Mount_top".
- Find other containers inside shoulders and cockpit.

All weapons contain locators at their barrel ends (named "Barrel_end", or "Barrel_end_[number]" in case there are multiple barrels).


The buggies:

The assembly process is the same as for the robots. Start with the "Mount_Cockpit" container inside the chassis.
- Buggies' wheels can be detached or changed (just drop the wheels into the "Mount_Wheel" container).
- There are no snapping points for the spoiler parts, you'll have to eyeball it.
You can make a simple suspension with the skinned chassis. Just move the joints named "Wheel_".
Not sure if the suspension is organized in the best way, just tried to make it simple.
If you have any recommendations, please write, will try to improve it.


__________________________________________________________________________________________________________________



Animations:


The animations are 30fps.
You'll have to animate the wheels and tank threads yourself (by translating the texture or the thread UV's).

If you are familiar with Maya, and want to tweak the animations or create the new ones,
you can find the rigged legs and some animation clips in the "Spider_Maya_Rigs.rar".
The animations come as multiple separate files with animated skeleton only (no model), or the animated models (in the "Big_scene" file).


Here are the frames for the animated models in the "Big_scene" file:


Spider and transformer legs animations (frames):
Deactivate 	(1-19) 
Activate 	(30-58)
Death 		(70-84) (70-87 for light legs)
Idle 		(90-139)
Strafe_L 	(150-175)
Strafe_R 	(180-205)
Turn_45_L 	(210-239)
Turn_45_R 	(250-279)
Turn_20_L 	(290-319)
Turn_20_R 	(330-359)
Walk 		(370-395)
Walk_Back 	(400-425)
Walk_Turn_L 	(430-455)
Walk_Turn_R 	(460-485)

Transformer legs only:
Roller_DeActivate 	(500-559)
Roller_Death 		(570-587)
Roller_Idle 		(590-639)
Roller_Roll 		(650-689)
Roll_Turn_L 		(700-739)
Roll_Turn_R 		(750-789)
Transform_to_Roller 	(800-849)
Transform_to_Spider 	(860-909) (860-905 for light legs)


Legs_Tracks:
Roll 	(1-40)
Idle 	(50-99)
Death 	(110-128)


Backpack_Gun:
Fold 		(1-36)
Unfold 		(50-85)
Fold_Halfway 	(100-125)
Unfold_Halfway 	(130-155)


Halfshoulder_Extender:
Fold 	(1-12)
Unfold 	(15-26)


Weapons:

Sniper: 
Shoot (1-13)


Grenade_Launcher:
Fold 	(1-21)
Unfold 	(25-43)
Shoot 	(45-60)


Double_Gun:
Shoot (1-16)
Round (20-39)


__________________________________________________________________________________________________________________


Textures:


Hand-painted:
The source .PSD can be found in the "Materials" folder.
For a quick repaint, adjust the layers in the "COLOR" folder. You can drop your decals and textures (camouflage, for example) in the folder as well. Just be careful with texture seams.
You may want to turn off the "FX_Rust" and "FX_Chipped_paint" layers for more cartoony look.
The baked occlusion and contours may be found in the "BKP" folder.


PBR:
A set of PBR maps (Specular), including simple normal map, can be found in in the "Materials/PBR_Maps" folder
Decided not to include the PBR source PSD's directly in the package - they weigh a lot and not sure if they're needed by many people.
Here's the link:
https://drive.google.com/file/d/0B2mY9IjHMQLbNjZOU0FBdlB5Uzg/view?usp=sharing
The .rar contains DDo 2.0 project, which consists of several PSD files that you can edit manually as well.
To create new mech colors, edit the Albedo one (mostly the layers under Body_Paint group).