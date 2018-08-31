using Godot;
using System;
using GMTK.Graph;
using Generic = System.Collections.Generic;

namespace GMTK.Tile
{
/*
 * Contains all information relative to one zone of the map
 *
 * @author Timothee Marczyk
 */
public class Zone {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
			/** Taken over status accessor. */
			public bool IsTakenOver { get => takenOver; }

			public int RaidValue { get => raidValue; }

			public int Wells { get => wellAmount; }
			public int Houses {get => houseAmount; }

			public bool IsObjective;
		// -- Public Attributes --
		// -- Protected Attributes --
		// -- Private Attributes --

		private static int grassFoodValue = 3;
		private static int cowHarvestValue = 10;
		private static int wellDrawValue = 10;
		private Generic.List<Edge> edges = new Generic.List<Edge>();
		private Generic.List<Cow> cows = new Generic.List<Cow>();
		private Generic.List<Vector2> grass = new Generic.List<Vector2>();
		private int raidValue;
		private int wellAmount;
		private bool takenOver;
		private int houseAmount;

	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		public Zone() {
			takenOver = false;
			raidValue = 0;
			wellAmount = 0;
		}
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
		public void AddEdge(Edge e) {
			edges.Add(e);
		}

		public void AddGrass(Vector2 pos, int amount){
			raidValue += amount * grassFoodValue;
			grass.Add(pos);
		}

		public void AddWell(){
			wellAmount += 1;
		}

		public void addHouse(){
			houseAmount += 1;
		}

		public void AddCow(Cow c){
			cows.Add(c);
		}

		public int Harvest(){
			return cows.Count * cowHarvestValue;
		}

		public int DrawWater(){
			return wellAmount * wellDrawValue;
		}

		public void TakeOver(){
			takenOver = true;
			/** TODO: Take grass away visually */
		}

		public Generic.List<Cow> GetCows(){
			return cows;
		}

		public void Merge(Zone z){
			this.cows.AddRange(z.cows);
			this.grass.AddRange(z.grass);
			this.raidValue += z.raidValue;
			this.houseAmount += z.houseAmount;
			this.wellAmount += z.wellAmount;
			foreach(Edge e in z.edges){
				if(!edges.Contains(e)){
					edges.Add(e);
				}
			}

		}

		public override String ToString() {
			String output = "";

			output += " => Cows : " + cows.Count + "\n";
			output += " => Houses : " + houseAmount + "\n";
			output += " => Grass : " + grass.Count + "\n";
			output += " => Well : " + wellAmount + "\n";
			output += " => Taken Over : " + IsTakenOver + "\n";
			output += " => IsObjective : " + IsObjective + "\n";

			return output;
		}

		// -- Protected Methods --
		// -- Private Methods --
		// --- /Methods ---

}
}