using System;
using IO = System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace MaxGame { 
	

	public static class JsonHandler {

		// Stores a dictionary containing the IDs and packed scenes of all objects implementing
		// IBuildable.


		static Dictionary<int, PackedScene> IBuildableDict;

		// This stores a list of buildings that a playable unit can construct through the mouse interface.


		static JsonHandler() {

			IBuildableDict = new Dictionary<int, PackedScene>();
			// Buildings start at key 1

			IBuildableDict.Add(1, (PackedScene)ResourceLoader.Load("res://Actors/Buildings/CentralCommand.tscn"));
			IBuildableDict.Add(2, (PackedScene)ResourceLoader.Load("res://Actors/Buildings/ConstructorBuilding.tscn"));
			IBuildableDict.Add(3, (PackedScene)ResourceLoader.Load("res://Actors/Buildings/HangarLarge.tscn"));
			IBuildableDict.Add(4, (PackedScene)ResourceLoader.Load("res://Actors/Buildings/GeneratorLarge.tscn"));
			IBuildableDict.Add(5, (PackedScene)ResourceLoader.Load("res://Actors/Buildings/MissileLauncher.tscn"));

			// Units start at key 101

			IBuildableDict.Add(101, (PackedScene)ResourceLoader.Load("res://Actors/Units/LightningRifleMech.tscn"));



			//string output = JsonSerializer.Serialize(CentralCommand);
			//string path = @"C:\Users\ikono\Documents\Maximum Game\Actors\BuildMenuOptions\CentralCommandBuildables.json";

			//IO.File.WriteAllText(path, output);

		}


		public static List<int> GetBuildableList(string path) {

			string input = IO.File.ReadAllText(path);

			List<int> output = (List<int>)JsonSerializer.Deserialize(input, typeof(List<int>));

			Console.WriteLine("Returning available buildable IDs from Json Handler");



			//List<int> CentralCommand = new List<int>();
			//CentralCommand.Add(1);
			//CentralCommand.Add(3);
			//CentralCommand.Add(101);

			return output;

		}

		public static void ExportToJson(Dictionary<string, string> dict, string path) {


			string output = JsonSerializer.Serialize(dict);

			IO.File.WriteAllText(path, output);
			
		}

		public static Dictionary<int, PackedScene> GetIBuildables() {
			
			
			Console.WriteLine("Returing all buildable IDs from Json Handler");


			return IBuildableDict;

		}

		//*****************
		// Experiment with modifying the properties in real time
		//*****************

	// public override void _PhysicsProcess(float delta) {

	// 		if ((_Get("IntProperty") != null) && (IsGotten == false)) {

	// 			Console.WriteLine(_Get("IntProperty").ToString());
	// 			Console.WriteLine("Should be writing IntProperty");

	// 			IsGotten = true;

				
	// 			Dictionary<string, object> dictOne = new Dictionary<string, object>();
	// 			Dictionary<string, object> dictTwo = new Dictionary<string, object>();

				
	// 			dictOne.Add("name", "Dynamic Vector3 Property");
	// 			dictOne.Add("type", 7);
	// 			dictTwo.Add("name", "Dynamic Object Property");
	// 			dictTwo.Add("type", 17);

	// 			DictArray.Add(dictOne);
	// 			DictArray.Add(dictTwo);
	// 		}
	// 	}

		// public override object _Get(String name) {


		// 	Console.WriteLine("Setting property");
		// 	if (name == "IntProperty") {
		// 		return StoredInt;
				
				
		// 		return true;
		// 	}
		// 	else {
		// 		Console.WriteLine("Failed to get property");
		// 		return false;
		// 	}
		// }



		// public override bool _Set(String name, object value) {


		// 	Console.WriteLine("Setting property");
		// 	if (name == "IntProperty") {
		// 		int newInt = (int)value;
				
				
		// 		return true;
		// 	}
		// 	else {
		// 		Console.WriteLine("Failed to set property");
		// 		return false;
		// 	}
		// }

		// public override Godot.Collections.Array _GetPropertyList() {

		// 	Console.WriteLine("Returning property list");


		// 	return DictArray;
		// }

	}
}
