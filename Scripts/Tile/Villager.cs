using Godot;
using System;
using GMTK.Management;

namespace GMTK.Tile
{
/*
 * 
 *
 * @author 
 */
public class Villager : RigidBody2D {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
		// -- Public Attributes --
		// -- Protected Attributes --
		// -- Private Attributes --
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
		public void repair(Wall w){
			if(Manager.RollOpponent()){
				w.GainHp();
				Manager.opponent.loseMorale(Wall.maxHp[(int)w.WType] - w.HP);
			}
		}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}