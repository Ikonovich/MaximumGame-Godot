using System;
using Godot;
using MagicSmoke;

namespace MaxGame {


	public enum ResourceType {
	 	Energy,
	 	Metal,
	 	Crystal
	}


	public class ResourceDeposit : StaticBody {
		

		// Provides an ID for this particular mining resource. Must be unique.
		[Export]
		public int ID { get; set; }

		// Used to set what type of resource this is. 0 = Energy, 1 = Metal, 2 = Crystal.

		[Export]
		public ResourceType ResourceType = ResourceType.Energy;


		// Used to determine what base amount is returned when this resource is mined.

		[Export] 
		public int HarvestAmount { get; set; } = 10;


		// Determines how much of the resource this object starts with.
		[Export]
		public int TotalAmount { get; set ; } = 10000;

		protected int CurrentAmount;


		public override void _Ready() {

			CurrentAmount = TotalAmount;

		}

		// When this resource is mined, determines how many resources should be acquired and
		// returns them.
		// Efficiency is the harvester's collection efficiency.
		// When the resource is empty, the node is deleted.

		public int Harvest(float Efficiency) {




			int harvestResult = (int)(Efficiency * HarvestAmount);

			Console.WriteLine("Harvest result: " + harvestResult.ToString());

			if (harvestResult > CurrentAmount) {

				QueueFree();
				return CurrentAmount;
			}
			else {

				CurrentAmount -= harvestResult;
				return harvestResult;

			}
		}
	}
}
