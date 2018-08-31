using Godot;
using System;
using GMTK.Tile;
using Generic = System.Collections.Generic;

namespace GMTK.Management
{
/*
 * 
 *
 * @author 
 */
public class Player {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --

		public int Morale { get => morale; }

		public int Food { get => food; }

		public int Wells { get => wells; }

		public int Objectives { get => objectives; }

		// -- Public Attributes --

		public Generic.List<Cow> cows = new Generic.List<Cow>();

		// -- Protected Attributes --
		// -- Private Attributes --
		private int morale;
		private int food;
		private int wells;
		private int objectives;

	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --

		// Constructor for the player class, should be called after the graph has been generated to retrieve the default food and water values
		public Player(int startMorale, int startFood){
			morale = startMorale;
			food = startFood;
			wells = 1;
			objectives = 0;
			foreach(Graph.Face f in Manager.graph.Faces){
				if(f.zone.IsTakenOver){
					cows.AddRange(f.zone.GetCows());
					if(f.zone.IsObjective){
						objectives++;
					}
				}
				wells += f.zone.Wells;
			}
		}
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --

		public void addWell(int amount){
			if(amount > 0){
				wells += amount;
			}
		}

		public void addFood(int amount){
			if(amount > 0){
				food += amount;
			}
		}

		public void addObjective(){
			objectives++;
		}

		public void loseMorale(int value){
			if(value > 0){
				morale -= value;
			}
		}

		// Makes the morale evolve depending on the number of cows controlled, the amount of food owned and the number of wells under your control.
		// Reduces the amount of food by the number of cows.
		public void endTurn(){
			
			// Reducing morale in case of insufficient water
			if(cows.Count > wells){
				morale -= (wells - cows.Count);
			}

			morale += (food / cows.Count);

			food -= cows.Count;

			foreach(Cow c in cows){
				c.EndTurn();
			}
		}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}