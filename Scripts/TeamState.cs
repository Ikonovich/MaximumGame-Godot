using System;
using System.Collections.Generic;
using Godot;


namespace MaxGame {
    
// This class is used by the game controller to keep track of teams. 
// It stores the team ID, current resources available to the team, and a list of all units on the team.

    public class TeamState {



        public int TeamID { get; set; }

        // Stores the amount of each resource the team has, referenced by the resourcetype.
        // Used by a variety of classes to get the amount of resources available.

        public Dictionary<ResourceType, int> ResourceDict;

        // Stores a list of buildings that this team can construct.
        // Accessed and used by the HUD.

        public List<int> AvailableBuildings;


        public void CreateTeam(int teamID) {

            ResourceDict = new Dictionary<ResourceType, int>();

            ResourceDict.Add(ResourceType.Energy, 1500);
            ResourceDict.Add(ResourceType.Metal, 22075);
            ResourceDict.Add(ResourceType.Crystal, 901);

            TeamID = teamID;

            // Sets the default available build items

            AvailableBuildings = new List<int>();
            
            AvailableBuildings.Add(1);
            AvailableBuildings.Add(2);
            AvailableBuildings.Add(3);
            AvailableBuildings.Add(4);
            AvailableBuildings.Add(5);



            

        }

        // Takes a resource dictionary and adds it to the current resource dictionary.
        // Useful for taking out multiple types of resources at once when I.E. building units.
        // Use negative values to remove resources.

        public void UpdateResourceDict(Dictionary<ResourceType, int> modifier) {

            foreach (KeyValuePair<ResourceType, int> pair in modifier) {

                ResourceDict[pair.Key] += pair.Value;
            }
        }

        // Used to update a single resource, ideal when mining.
        public void UpdateResource(ResourceType resource, int modifier) {

            Console.WriteLine("Resource updated: Added " + modifier.ToString() + " of " + resource.ToString());
            
            ResourceDict[resource] += modifier;
        }
    }
}