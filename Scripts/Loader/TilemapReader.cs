using Godot;
using System;

using Generic = System.Collections.Generic;

namespace GMTK.Loader
{
/*
 * Uses the currently loaded Tilemaps and detect their contents.
 * Returns a Graph object containing data about the world.
 *
 * @author Caillaud Jean-Baptiste
 */
public static class TileMapReader {
	// ---  Attributes ---
		// -- Properties --
		// -- Public Attributes --
		// -- Private Attributes --
	// --- /Attributes ---

	// ---  Methods ---
		// -- Public Methods --
			/**
			 * Loads all data from the TileMaps below the given node.
			 * Returns a Graph object, loaded with all the required data.
			 */
			public static Graph.Graph LoadGraph(Node root) {
				// Prepare the Graph instance.
				Graph.Graph graph = new Graph.Graph();

				// Find all tilemap subnodes.
				Generic.List<TileMap> tilemaps = FindTilemapsUnder(root);

				// Get the limits of all the tilesets.
				Rect2 limits = GetTilemapsLimits(tilemaps);
				// Compute the limits of the loops.
				Vector2 start = limits.Position;
				Vector2 end   = limits.Position + limits.Size;

				// Loop through each tilemap.
				foreach(TileMap tilemap in tilemaps) {
					// Loop through the limits of the tilemap.
					for (int x = (int)start.x; x < end.x; x++) {
						for (int y = (int)start.y; y < end.y; y++) {
							// First, get the identifier of the cell.
							int tileID = tilemap.GetCell(x, y);

							// If the identifier is not -1.
							if (tileID != -1) {
								// Get the name of the tile.
								String tileName = tilemap.TileSet.TileGetName(tileID);

								// Check if the tile is a wall.
								ComputeWall(tileName, x, y, ref graph);

								// Check if the tile is a vertex.
								ComputeVertex(tileName, x, y, ref graph);

								// Compute the cost of the tile.
								ComputeCost(x, y, tilemap);
							}

							// If the tilemap handles zone splitting.
							if (tilemap.CollisionMask == 0b1000) {
								// Compute the face this tile belongs to.
								ComputeFace(x, y, ref graph);

								// If there is a tile.
								if (tileID != -1)
									// Compute the resources on that tile.
									ComputeResource(x, y, tilemap, ref graph);
							}
						}
					}
				}

				// Prepare the list of vertices to remove.
				Generic.List<Graph.Vertex> VerticesToRemove = new Generic.List<Graph.Vertex>();

				// Loop through each vertex.
				foreach (Graph.Vertex vtx in graph.Vertices) {
					// Check all four sides.
					Vector2[] directions = new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };
					foreach(Vector2 dir in directions) {
						// Check if a wall is present and has not been computed.
						Vector2 currentPosition = vtx.Position + dir;
						Tile.Wall dirWall = graph.GetWallAt(currentPosition);
						if (dirWall != null && dirWall.Edge == null) {

							// Get the rooms on each side of the wall.
							Graph.Face[] faces = graph.GetFacesFromWall(currentPosition);

							// Start computing a new edge.
							Graph.Edge edge = new Graph.Edge(faces);

							// Add the edges to the face.
							edge.RightFace = faces[0];
							edge.LeftFace  = faces[1];

							// Loop until a new vertex is found.
							while (graph.GetVertexAt(currentPosition) == null) {

								// If the wall exists.
								Tile.Wall wall = graph.GetWallAt(currentPosition);
								if (wall != null) {
									// Add the wall to the edge.
									edge.AddWall(wall);

									// Increment the position.
									currentPosition += dir;
								
								// If the wall stops abruptly, stop execution.
								} else {
									throw new Exception("FATAL ERROR: Stray wall found @ " + currentPosition + " ...");
								}
							}

							// Add the vertices to the edge.
							edge.StartVertex = vtx;
							edge.EndVertex = graph.GetVertexAt(currentPosition);

							// Add the edge to the graph.
							vtx.Edges.Add(edge);
							graph.AddEdge(edge);

							// Add the edge to the face.
							foreach(Graph.Face face in faces) face.AddEdge(edge);
						}
					}

					// After that is done, check if the vertex object is NOT a tower.
					if (!vtx.IsTower) {
						// If the vertex has two edges.
						if (vtx.Edges.Count == 2) {
							// Get the vertex's edges.
							Tuple<Graph.Edge, Graph.Edge> edges = new Tuple<Graph.Edge, Graph.Edge>(
								vtx.Edges[0], vtx.Edges[1]
							);

							// Merge the edges together.
							edges.Item1.Merge(edges.Item2);

							// Remove the second edge.
							graph.RemoveEdge(edges.Item2);
						
						// If the vertex has one single edge.
						} else if (vtx.Edges.Count == 1) {
							// Add the vertex's wall to the edge.
							vtx.Edges[0].AddWall(vtx.Wall);
						}


						// Add the vertex to removal.
						VerticesToRemove.Add(vtx);
					}
				}

				// Remove the vertices.
				foreach (Graph.Vertex vtx in VerticesToRemove) graph.RemoveVertex(vtx);

				// Return the generated graph.
				GD.Print(graph.ToString());
				return graph;
			}
		// -- Private Methods --
			/**
			 * Returns an array of all the TileMaps object under a given node.
			 */
			private static Generic.List<TileMap> FindTilemapsUnder(Node root) {
				// Prepare the return list.
				Generic.List<TileMap> output = new Generic.List<TileMap>();

				// Loop through all the children nodes.
				foreach (Node child in root.GetChildren()) {
					// If the object is a TileMap.
					if (child is TileMap) {
						// Add it to the result list.
						output.Add(child as TileMap);
					}

					// If the object has any children.
					if (child.GetChildCount() > 0) {
						// Add its sub-tilemaps to the list.
						output.AddRange(FindTilemapsUnder(child));
					}
				}

				// Return the list.
				return output;
			}

			/**
			 * Returns a Rect2 containing all the tilemaps object given.
			 */
			private static Rect2 GetTilemapsLimits(Generic.List<TileMap> tilemaps) {
				// Prepare the return rect.
				Rect2 limits = new Rect2();

				// Loop through the tilemaps.
				foreach(TileMap tilemap in tilemaps) {
					// Merge the rect2 objects.
					limits = limits.Merge(tilemap.GetUsedRect());
				}

				// Return the limits.
				return limits;
			}

			/**
			 * Check if the given tile is a vertex.
			 * If it is, add it to the given graph reference.
			 */
			private static void ComputeVertex(String name, int x, int y, ref Graph.Graph graph) {
				// Prepare the regex matcher.
				RegEx rgx = new RegEx();
				rgx.Compile("^Wall._[TBLR]{2}$|^(Tower)");

				// Get and check the result of the regex.
				RegExMatch result = rgx.Search(name);
				if (result != null) {
					// Check wether a tower was selected.
					bool bIsTower = result.GetString(1).Equals("Tower");

					// Create a new Vertex instance.
					Graph.Vertex vtx = new Graph.Vertex(x, y, graph.GetWallAt(new Vector2(x, y)));

					// Add the instance to the graph.
					graph.AddVertex(vtx);
				}
			}

			/**
			 * Check if the given tile is a wall.
			 * If it is, add it to the given graph reference.
			 */
			private static void ComputeWall(String name, int x, int y, ref Graph.Graph graph) {
				// Prepare the regex matcher.
				RegEx rgx = new RegEx();
				rgx.Compile("^Wall([SOW])_([TLBR]{1})(Door)?.*$");

				// Get and check the result of the regex.
				RegExMatch result = rgx.Search(name);
				if (result != null) {
					// Get the type of wall.
					Tile.WallType type = Tile.WallType.WOOD;
					switch (result.GetString(1)) {
						case "S": type = Tile.WallType.STONE;    break;
						case "O": type = Tile.WallType.OBSIDIAN; break;
						case "W": type = Tile.WallType.WOOD;     break;
					}

					Tile.Orientation dir = Tile.Orientation.OTHER;
					switch (result.GetString(2)){
						case "T": dir = Tile.Orientation.TOP; break;
						case "B": dir = Tile.Orientation.BOTTOM; break;
						case "L": dir = Tile.Orientation.LEFT; break;
						case "R": dir = Tile.Orientation.RIGHT; break;
					}


	
					// Check if the given wall is a door.
					bool bIsDoor = result.GetString(3).Equals("Door");

					// Create a new Wall instance.
					Tile.Wall wall = new Tile.Wall(type, bIsDoor, dir);
					wall.Position = new Vector2(x, y);

					// Add it to the graph object.
					graph.AddWall(wall, x, y);
				}
			}

			/**
			 * Checks wether this tile belongs to a given face.
			 */
			private static void ComputeFace(int x, int y, ref Graph.Graph graph) {
				// Check wether this tile is a wall, a vertex, or an empty cell.
				Vector2 position = new Vector2(x, y);
				Tile.Wall cellWall = graph.GetWallAt(position);
				Graph.Vertex cellVtx = graph.GetVertexAt(position);
				if (cellWall == null && cellVtx == null) {
					// Get the top and left cells.
					Vector2 left = position + new Vector2(-1,  0);
					Vector2 top  = position + new Vector2( 0, -1);
					
					// Get the faces of the cells.
					Graph.Face leftFace = graph.GetFaceFromPoint(left);
					Graph.Face topFace  = graph.GetFaceFromPoint(top);

					Graph.Face pointFace;
					// If both faces are null.
					if (leftFace == null && topFace == null) {
						// Create a new face.
						pointFace = new Graph.Face();

						// Add the face to the graph.
						graph.AddFace(pointFace);

					// If both faces are different.
					} else if (leftFace != topFace) {
						// Merge both faces together.
						pointFace = graph.MergeFaces(leftFace, topFace);

					// If both faces are the same.
					} else {
						// Select it as the point's face.
						pointFace = leftFace;
					}

					// Add the point to the face.
					pointFace.AddPoint(position);
				}
			}

			/**
			 * Computes the resource on the given tile.
			 */
			private static void ComputeResource(int x, int y, TileMap map, ref Graph.Graph graph) {
				// Get the face of the given resource.
				Graph.Face face = graph.GetFaceFromPoint(new Vector2(x, y));
				if (face != null) {
					// Prepare the regex of the tile name.
					RegEx rgx = new RegEx();
					rgx.Compile(".*(House|Well|Grass|Cow|Start|Objective).*");

					// Check the result.
					String tileName = map.TileSet.TileGetName(map.GetCell(x, y));
					RegExMatch result = rgx.Search(tileName);
					if (result != null)
						switch(result.GetString(1)) {
							case "House": 
								face.zone.addHouse(); 
								break;

							case "Well" : 
								face.zone.AddWell();  
								break;

							case "Grass": 
								// Check the value of the grass.
								rgx.Compile("([0-9])");
								result = rgx.Search(tileName);
								if (result != null)
									face.zone.AddGrass(new Vector2(x, y), result.GetString(1).ToInt());
								break;

							case "Objective": 
								face.zone.IsObjective = true; 
								break;

							case "Cow"  : 
								// Add a cow to the zone.
								Tile.Cow cow = Tile.Cow.Instance();
								cow.Position = new Vector2(x, y);
								World.Instance.AddChild(cow);
								face.zone.AddCow(cow); 

								// Remove the cow.
								map.SetCell(x, y, -1); 
								break;

							case "Start": 
								// Take the zone.
								face.zone.TakeOver(); 

								// Remove the start point.
								map.SetCell(x, y, -1); 
								break;
						}
				}
			}

			/**
			 * Computes the cost of a given tile.
			 */
			private static void ComputeCost(int x, int y, TileMap map) {
				// Get the tile id.
				int tileID = map.GetCell(x, y);
				if (tileID > -1) {
					// Get the name of the tile.
					String tileName = map.TileSet.TileGetName(tileID);

					// Prepare the weight counter.
					float weight = 1;

					// Check the name of the tile.
					RegEx rgx = new RegEx();
					rgx.Compile("(Road|Wall|Tower|House|Well|Tree|Objective|Barrier)");
					RegExMatch result = rgx.Search(tileName);
					if (result != null) {
						switch(result.GetString(1)) {
							case "Wall":
							case "Tower":
							case "Well":
							case "Tree":
							case "House":
							case "Barrier":
							case "Objective":
								weight = float.PositiveInfinity;
								break;
						}
					}

					// Add the weight.
					Pathfinder.AStar.AddCost(x, y, weight);
				}
			}
	// --- /Methods ---

}
}