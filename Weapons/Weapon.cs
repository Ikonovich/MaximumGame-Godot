using System;
using Godot;
using MagicSmoke;

namespace MaxGame {

    public class Weapon : Spatial {

        [Export]
        public float Range { get; set; }

        [Export]
        public float Damage { get; set; }

        //Determines how long the weapon has to wait between shots.

        [Export]
        public float FireDelay = 1.0f;

        // Keeps track of how long it has been since the weapon fired.
        [Export]
        public float DelayCountdown = 0;


        // Stores the GUI icon for this weapon.
        [Export]
        public Texture Icon { get; set; }

        protected GameController GameController;

        protected DebugRenderer DebugRenderer;

        protected Unit Target;


        // Used to store the owner. Must be set from the parent for each weapon.
        public PlayableUnit Owner;

        // Used to store the team ID. Must be set from the parent for each weapon.

        public int TeamID;


        // Raycast used by the weapon, if necessary

        protected RayCast Ray;



        public override void _Ready() {


        }

        public virtual void Shoot() {

            Console.WriteLine("Shoot in Weapon");

        }


        // Returns the icon. 

        public virtual Texture GetIcon() {

            Texture failOverTexture = (Texture)ResourceLoader.Load("res://Game Assets/GUI/Icons/ErrIcon.png");

            if (Icon != null) {
                return Icon;
            }
            else {

                Console.WriteLine("Looks like you forgot to set an icon for this weapon.");
                return failOverTexture;
            }


        }
    }
}