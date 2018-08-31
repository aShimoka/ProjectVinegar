using Godot;
using System;
using GMTK.Management;

namespace GMTK
{
/*
 * 
 *
 * @author Caillaud Jean-Baptiste
 */

enum ActionType{
	NONE,
	SELECT,
	MOVE,
	DISMOUNT
}

public class World : Node2D {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
		// -- Public Attributes --
			public static World Instance;
		// -- Protected Attributes --
		// -- Private Attributes --
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
			public override void _Ready() {
				Instance = this;
				
				Manager.graph = Loader.TileMapReader.LoadGraph(this);
				
				Manager.objectiveCount = 0;
				foreach(Graph.Face face in Manager.graph.Faces) {
					if (face.zone.IsObjective) {
						Manager.objectiveCount++;
					}
				}

				Manager.player = new Player(100, 100);
				
				Manager.opponent = new Opponent(100, 10);

				Manager.currentCow = Manager.player.cows[0];
			}

			public override void _Input(InputEvent ev){
				if(ev is InputEventKey){
					Manager.EndTurn();
				}
				if(ev is InputEventMouseButton){
					if(((InputEventMouseButton)ev).ButtonIndex == (int)ButtonList.Left && ((InputEventMouseButton)ev).Pressed){
						ActionType t = ActionType.NONE;
						Vector2 mousePos = Graph.Graph.WorldToMap(GetGlobalMousePosition());
						foreach(Tile.Cow c in Manager.player.cows){
							if(c.Position.x == mousePos.x && c.Position.y == mousePos.y){
								t = ActionType.SELECT;
								if (Manager.currentCow != null)
									(Manager.currentCow.GetNode("Light2D") as Light2D).Visible = false;
								Manager.currentCow = c;
								(Manager.currentCow.GetNode("Light2D") as Light2D).Visible = true;
								
							}
						}

						if(t == ActionType.NONE){
							GD.Print("Setting target to : (" + mousePos.x + ", " + mousePos.y + ")\n");
							// Check if pointed case is either a wall or walkable surface
							Manager.currentCow.setTarget(mousePos);
							if(!Manager.currentCow.acted){
								Manager.currentCow.Act();
							}
						}
					}
				} else if (ev is InputEventMouseMotion) {
					// Get mouse position.
					Vector2 mPos = GetGlobalMousePosition();

					// Get tilemaps.
					TileMap tm = GetNode("Test/Bg") as TileMap;
					TileMap tmOverlay = GetNode("Test/Overlay") as TileMap;

					if (tm != null && tmOverlay != null) {
						Vector2 mapPos = tm.WorldToMap(mPos);
						Graph.Face face = Manager.graph.GetFaceFromPoint(mapPos);
						Rect2 rect = tm.GetUsedRect();
						for(int i = (int)rect.Position.x; i < (rect.Position.x + rect.Size.x); i++)
							for (int j = (int)rect.Position.y; j < (rect.Position.y + rect.Size.y); j++)
								tmOverlay.SetCell(i, j, -1);
						
						if (face != null) {
							foreach (Vector2 p in face.Points) {
								tmOverlay.SetCell((int)p.x, (int)p.y, 0);
							}

							foreach (Graph.Edge e in face.Edges) {
								foreach (Tile.Wall w in e.Walls)
									tmOverlay.SetCell((int)w.Position.x, (int)w.Position.y, 1);
							}
						}
					}
				}
			}

			public override void _Process(float delta) {
			}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}