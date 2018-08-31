using System;
using GMTK.Management;

namespace GMTK.Tile
{
/*
 * 
 *
 * @author 
 */
public class Cow : Sprite {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
		public new Vector2 Position { get {
			return Graph.Graph.WorldToMap(base.Position);
		} set { base.Position = 64 * value; } }
		// -- Public Attributes --
		// -- Protected Attributes --
		// -- Private Attributes --
		public bool controled = false;

		public bool acted = false;

		public bool hasTarget = false;
		private bool targetIsWall = false;
		public Vector2 targetPos;
		private Wall targetWall;
		private int counter = 0;
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --

		public static Cow Instance() {
			PackedScene scene = GD.Load("res://Scenes/Tile/Cow.tscn") as PackedScene;
			return scene.Instance() as Cow;
		}
		public void setTarget(Vector2 pos){
			// If the face belongs to the user.
			if (Manager.graph.GetWallAt(pos) != null) {

				GD.Print("Is wall");

				// If any of the wall's zones belong to the user.
				if (
					Manager.graph.GetWallAt(pos).Edge.LeftFace.zone.IsTakenOver ||
					Manager.graph.GetWallAt(pos).Edge.RightFace.zone.IsTakenOver
				) {

					GD.Print("Is taken over");
					// Set the target as a wall.
					targetWall = Manager.graph.GetWallAt(pos);
					targetPos = pos;
					targetIsWall = true;
					hasTarget = true;
					counter = 0;
				}
			}
			else if (Manager.graph.GetFaceFromPoint(pos).zone.IsTakenOver) {
				// Set the target.
				targetPos = pos;
				hasTarget = true;
				counter = 0;
			
			// If the target is a wall.
			}
		}

		
		public void Move() {
			// If a target was defined.
			if (hasTarget) {

				if (targetIsWall) {
					
					// Get the point in the occupied zone.
					foreach(Vector2 v in targetWall.Neighbours)
						if(Manager.graph.GetWallAt(v) == null)
							if (Manager.graph.GetFaceFromPoint(v).zone.IsTakenOver)
								targetPos = v;
					}
				 // Find a path to the point.
				Vector2[] path = Pathfinder.AStar.ComputePath(Position, targetPos).Item1;

				// Move along the path.
				int counter = 1;
				if (counter > path.Length) {
					counter = path.Length;
					hasTarget = false;
				}

				for(int i = 0; i < counter; i++){
					Position = path[i];
				}
				
			}
			
			if(Position == targetPos) {
				hasTarget = false;
			}
		}

		public void Attack(Wall w){
			if(Manager.RollPlayer()){
				w.LoseHp();
				Manager.player.loseMorale(w.HP);
				if(w.HP == 0){
					if(w.Edge.HasFallen()){
						if(w.Edge.LeftFace.zone.IsTakenOver){
							Raid(w.Edge.RightFace.zone);
						}
						else if(w.Edge.RightFace.zone.IsTakenOver){
							Raid(w.Edge.LeftFace.zone);
						}
						targetIsWall = false;
						hasTarget = false;
					}
				}
			}
		}

		public void Raid(Zone z){
			Manager.player.addWell(z.Wells);
			Manager.player.addFood(z.RaidValue);
			foreach(Cow c in z.GetCows()){
				c.controled = true;
				Manager.player.cows.Add(c);
			}
			if(z.IsObjective){
				Manager.player.addObjective();
			}
			z.TakeOver();
		}

		public void Act(){
			if(targetIsWall && Math.Abs(targetPos.x - Position.x) + Math.Abs(targetPos.y - Position.y) == 1){
				Attack(targetWall);
				GD.Print("Attacking\n");
			}
			else{
				Move();
				GD.Print("Moving\n");
			}
			acted = true;
		}

		public void EndTurn(){
			if(!acted){
				Act();
			}
			acted = false;
		}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}