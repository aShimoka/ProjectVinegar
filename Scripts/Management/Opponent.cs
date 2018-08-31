using Godot;
using System;

namespace GMTK.Management
{
/*
 * Contains all variables and functions required to manage the opponent's morale and actions
 *
 * @author Timothee Marczyk
 */
public class Opponent {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --

		public static int maxMoraleLossPerStat = 10;
		public int Morale { get => morale; }
		public int Food { get => food; }
		public int Water { get => water; }
		public int Population { get => population; }
		public int Houses { get => houses; }

		// -- Public Attributes --
		// -- Protected Attributes --
		// -- Private Attributes --
		private int morale;
		private int food;
		private int water;
		private int population;
		private int houses;

	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		public Opponent(int startMorale, int startPopulation){
			morale = startMorale;
			population = startPopulation;
			food = startPopulation;
			water = startPopulation;
			houses = 0;

			foreach(Graph.Face f in Manager.graph.Faces){
				if(!f.zone.IsTakenOver){
					houses += f.zone.Houses;
				}
			}
			Harvest();
		}
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
		public void loseMorale(int amount){
			if(amount > 0){
				morale -= amount;
			}
		}

		public void Harvest(){
			foreach(Graph.Face f in Manager.graph.Faces){
				if(!f.zone.IsTakenOver){
					food += f.zone.Harvest();
					water += f.zone.DrawWater();
				}
			}
		}


		public void loseHouses(int amount){
			houses -= amount;
		}

		public void endTurn(){
			if(population > houses){
				morale -= ((houses - population) * maxMoraleLossPerStat) / population;
			}

			if(food >= population && population != 0){
				morale += food / population;
			}
			else if(food != 0){
				morale -= Math.Min(population / food, maxMoraleLossPerStat);
			}
			else{
				morale -= maxMoraleLossPerStat;
			}

			if(water >= population && population != 0){
				morale += water / population;
			}
			else if(water != 0){
				morale -= Math.Min(population / water, maxMoraleLossPerStat);
			}
			else{
				morale -= maxMoraleLossPerStat;
			}

			food -= population;
			water -= population;

			Harvest();
		}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}