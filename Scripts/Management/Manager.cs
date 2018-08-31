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
public static class Manager {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --

		// -- Public Attributes --

		public static Graph.Graph graph;
		public static Random rand = new Random();
		public static Player player;
		public static Opponent opponent;
		public static Cow currentCow;
		public static int objectiveCount;

		// -- Protected Attributes --
		// -- Private Attributes --
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --

		public static bool RollPlayer(){
			return rand.Next(100) < player.Morale;
		}

		public static bool RollOpponent(){
			return rand.Next(100) < opponent.Morale;
		}
		public static void EndTurn(){
			player.endTurn();
			opponent.endTurn();
		}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}