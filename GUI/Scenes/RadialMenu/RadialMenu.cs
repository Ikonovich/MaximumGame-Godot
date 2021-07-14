using System;
using System.Collections.Generic;
using Godot;

namespace MaxGame {

    
    public class RadialMenu : Spatial {


        // <remarks>
        // Stores the radius of the menu. The first button will be placed directly above the unit at this
        // distance, while remaining buttons will be rotated off of it by 2PI/ButtonCount
        // </remarks>

        public float Radius = 4.0f;

        public List<RadialButton> ButtonList;

        public override void _Ready() {


        }

        public void ShowMenu() { 

            float offset = 2 * (float)Math.PI / ButtonList.Count;
            Vector3 circularVector = new Vector3(Radius, 0.0f, 0.0f);

            for (int i = 0; i > ButtonList.Count; i++) {


                RadialButton tempButton = ButtonList[i];

                Vector3 buttonPosition = GlobalTransform.origin + circularVector.Rotated(Vector3.Forward, offset);
                tempButton.Translation = buttonPosition;
                tempButton.ShowButton();


            }

        }

        public void HideMenu() {
            
             for (int i = 0; i > ButtonList.Count; i++) {

                RadialButton tempButton = ButtonList[i];
                tempButton.HideButton();


             }
        }



        public void PopulateMenu(List<RadialButton> buttonList) {

            ButtonList = buttonList;

        }

    }



}