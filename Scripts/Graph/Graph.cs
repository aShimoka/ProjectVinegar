using Godot;
using System;
using GMTK.Tile;
using Generic = System.Collections.Generic;

namespace GMTK.Graph
{
/*
 * 
 *
 * @author 
 */
public class Graph {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
		// -- Public Attributes --
			public Generic.List<Edge> Edges = new Generic.List<Edge>();

			public Generic.List<Vertex> Vertices = new Generic.List<Vertex>();

			/** List of points separated by room. */
			public Generic.List<Face> Faces = new Generic.List<Face>();
			
		// -- Protected Attributes --
		// -- Private Attributes --
			/** Map of all walls found. */
			private Wall[,] _map = new Wall[,] {};

			/** map width accessor. */
			private int _w { get => _map.GetLength(0); }

			/** map height accessor. */
			private int _h { get => _map.GetLength(1); }

	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
			/**
			 * Add a wall to the map.
			 */
			public void AddWall(Wall wall, int x, int y) {
				// Match the bounds of the map.
				MatchMap(x, y);

				// Set the wall.
				_map[x, y] = wall;
			}

			/**
			 * Transform world coordinates to map coordinates
			 */
			public static Vector2 WorldToMap(Vector2 position){
				return new Vector2((float)Math.Floor(position.x / 64f), (float)Math.Floor(position.y / 64f));
			}

			/**
			 * Add a vertex to the list.
			 */
			public void AddVertex(Vertex vtx) {
				// Avoid double elements.
				if (!Vertices.Contains(vtx))
					Vertices.Add(vtx);
			}

			/**
			 * Add a face to the graph.
			 */
			public void AddFace(Face face) {
				// Avoid double reference.
				if (!Faces.Contains(face))
					Faces.Add(face);
			}

			/**
			 * Add an edge to the graph.
			 */
			public void AddEdge(Edge edge) {
				// avoid double reference.
				if (!Edges.Contains(edge))
					Edges.Add(edge);
			}

			/**
			 * Remove a vertex from the graph.
			 */
			public void RemoveVertex(Vertex vtx) {
				if (Vertices.Contains(vtx))
					Vertices.Remove(vtx);
			}

			/**
			 * Remove an edge from the graph.
			 */
			public void RemoveEdge(Edge edge) {
				if (Edges.Contains(edge))
					Edges.Remove(edge);
			}

			/**
			 * Returns the wall found at the location given.
			 */
			public Wall GetWallAt(Vector2 pos) {
				// Check boundaries.

				if (pos.x < _w && pos.y < _h) {
					return _map[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)];
				} else {
					return null;
				}
			}

			/**
			 * Returns the vertex at the given position.
			 */
			public Vertex GetVertexAt(Vector2 pos) {
				// Loop through the vertices.
				foreach(Vertex vtx in Vertices) {
					// If the position matches.
					if (vtx.Position == pos) {
						// Return the vertex.
						return vtx;
					}
				}

				// return null.
				return null;	
			}

			/**
			 * Returns the face that contains this point, if any.
			 */
			public Face GetFaceFromPoint(Vector2 point) {
				// Loop through the faces.
				foreach(Face f in Faces) {
					// If the face contains the given point, return it.
					if (f.HasPoint(point)) {
						return f;
					}
				}

				return null;
			}

			/**
			 * Return the two faces that are next to a given wall.
			 */
			public Face[] GetFacesFromWall(Vector2 position) {
				// Check if the position is a wall.
				if (GetWallAt(position) != null) {
					// Create the output variable.
					Generic.List<Face> output = new Generic.List<Face>();

					// Look into each direction.
					Vector2[] directions = new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };
					foreach(Vector2 dir in directions) {
						// Check if the position is not wall.
						if (GetWallAt(position + dir) == null) {
							// Get the face from that point.
							Face face = GetFaceFromPoint(position + dir);
							if (face != null)
								output.Add(face);
						}
					}

					// Return the result.
					return output.ToArray();
				} else {
					throw new Exception("FATAL ERROR: " + position + " is not a wall.");
				}
			}

			/**
			 * String representation of the graph object.
			 */
			public override String ToString() {
				// Create the output string.
				String output = "";

				output += "Vtxs count: " + Vertices.Count + "\n";
				output += "Edge count: " + Edges.Count + "\n";

				int counter = 0;
				foreach (Wall w in _map)
					if (w != null) counter++;
				output += "Wall count: " + counter + "\n";
				output += "Face count: " + Faces.Count + "\n";

				foreach(Face f in Faces)
					output += f.ToString();

				return output;
			}

			/**
			 * Merges two faces together and returns the result.
			 */
			public Face MergeFaces(Face a, Face b) {
				// Check if both faces are correct.
				if (Faces.Contains(a) && Faces.Contains(b)) {
					// Merge b into a.
					a.Merge(b);

					// Remove b from the list.
					Faces.Remove(b);

					// Return a.
					return a;
				} else if (a != null || b != null) {
					// Return the face that is not null.
					return (a != null) ? a : b;
				} else {
					throw new Exception("Unhandled state");
				}
			}

		// -- Protected Methods --
		// -- Private Methods --
			/**
			 * Match the map bounds to the given position.
			 */
			private void MatchMap(int x, int y) {
				// Clamp the values of the bounds.
				int limX = (x >= _w) ? x + 1 : _w;
				int limY = (y >= _h) ? y + 1 : _h;

				// Check the limits.
				if (limX >= _w || limY >= _h) {
					// Resize the array.
					Wall[,] nMap = new Wall[limX, limY];

					// Copy the values over.
					for (int i = 0; i < _w; i++)
						for (int j = 0; j < _h; j++)
							nMap[i, j] = _map[i, j];
					
					// Change the array.
					_map = nMap;
				}
			}

	// --- /Methods ---

}
}