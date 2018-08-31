using Godot;
using System;
using GMTK.Graph;

namespace GMTK.Tile
{
/*
 * 
 *
 * @author 
 */

public enum WallType{
	WOOD, STONE, OBSIDIAN
};

public enum Orientation{
	TOP, BOTTOM, LEFT, RIGHT, OTHER
}

public class Wall {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
			public int HP { get => hp; }

			public bool IsDoor { get => isDoor; }

			public bool IsDoorOpen { get => isDoor && isDoorOpen; }

			public WallType WType { get => type; }

			public Edge Edge;

			public Vector2[] Neighbours {
				get => new Vector2[] {
					Position + Vector2.Up,
					Position + Vector2.Down,
					Position + Vector2.Left,
					Position + Vector2.Right
				};
			}
		// -- Public Attributes --
			public Vector2 Position;
			
		// -- Protected Attributes --
		// -- Private Attributes --

			public static int[] maxHp = new int[]{1, 2, 3};
			private int hp;
			private WallType type;
			private bool isDoor;
			private bool isDoorOpen;
			private Orientation direction;
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
			/**
			 * Create a new wall but do not assign it an edge.
			 */
			public Wall(WallType type, bool bIsDoor, Orientation direction) {
				this.type = type;
				this.isDoor = bIsDoor;
				this.Edge = null;
				this.hp = maxHp[(int)type];
				this.direction = direction;
				if (bIsDoor) hp *= 3;
			}

			public Wall(WallType type, bool isDoor, Orientation direction, Edge e) : this(type, isDoor, direction) {
				this.Edge = e;
			}
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
		public void OpenDoor(){
			if(isDoor){
				isDoorOpen = true;
			}
		}

		public void CloseDoor(){
			if(isDoor){
				isDoorOpen = false;
			}
		}

		public void LoseHp(){
			if(hp > 0) {
				if (isDoorOpen) {
					hp = 0;
				} else {
					hp--;
				}

				GD.Print(this.direction.ToString());
				TileMap t = GMTK.World.Instance.GetNode("Test/Deco") as TileMap;
				string tilename = "Wall_Down";
				if(hp != 0){
					tilename = "Wall";
					switch(type){
						case WallType.WOOD: tilename += "W_"; break;
						case WallType.STONE: tilename += "S_"; break;
						case WallType.OBSIDIAN: tilename += "O_"; break;
					}

					switch(direction){
						case Orientation.TOP: tilename += "T"; break;
						case Orientation.BOTTOM: tilename += "B"; break;
						case Orientation.LEFT: tilename += "L"; break;
						case Orientation.RIGHT: tilename += "R"; break;
					}

					if(hp != maxHp[(int)type]){
						tilename += hp.ToString();
					} 

				}
				else{
					if(direction == Orientation.TOP || direction == Orientation.BOTTOM){
						if(Management.Manager.graph.GetWallAt(new Vector2(Position.x - 1, Position.y)) != null){
							if(Management.Manager.graph.GetWallAt(new Vector2(Position.x - 1, Position.y)).HP != 0){
								switch(type){
									case WallType.STONE: tilename = "WallS_Down_Edge_L"; break;
									case WallType.OBSIDIAN: tilename = "WallO_Down_Edge_L"; break;
								}
							}
						}
						else if(Edge.EndVertex.Position == new Vector2(Position.x - 1, Position.y) || Edge.StartVertex.Position == new Vector2(Position.x - 1, Position.y)){
							switch(type){
								case WallType.WOOD: tilename = "Wall_Down"; break;
								case WallType.STONE: tilename = "WallS_Down_Edge_L"; break;
								case WallType.OBSIDIAN: tilename = "WallO_Down_Edge_L"; break;
							}
						}
					}
				}
				t.SetCell((int)Math.Round(Position.x), (int)Math.Round(Position.y), t.TileSet.FindTileByName(tilename));
				
			}
		}

		public void GainHp(){
			if(hp < maxHp[(int)this.type] && !Edge.HasFallen()){
				hp ++;
			}
		}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}