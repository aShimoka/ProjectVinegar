using Godot;
using System;
using GMTK.Tile;
using Generic = System.Collections.Generic;

namespace GMTK.Graph
{
/*
 * Contains all information about an edge
 *
 * @author Timothee Marczyk
 */
public class Edge {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
			/** Edge's start vertex. */
			public Vertex StartVertex {
				get => _vertices.Item1;
				set { _vertices = new Tuple<Vertex, Vertex>(value, _vertices.Item2); }
			}

			/** Edge's end vertex. */
			public Vertex EndVertex {
				get => _vertices.Item2;
				set { _vertices = new Tuple<Vertex, Vertex>(_vertices.Item1, value); }
			}

			/** Get the vertices as a list. */
			public Vertex[] Vertices {
				get => new Vertex[] { _vertices.Item1, _vertices.Item2 };
			}

			/** Gets the left side face. */
			public Face LeftFace {
				get => _faces.Item1;
				set { _faces = new Tuple<Face, Face>(value, _faces.Item2); } 
			}

			/** Gets the right side face. */
			public Face RightFace {
				get => _faces.Item2;
				set { _faces = new Tuple<Face, Face>(_faces.Item1, value); } 
			}

			/** Get the faces as a list. */
			public Face[] Faces {
				get => new Face[] { _faces.Item1, _faces.Item2 };
			}

			/** Get the list of walls. */
			public Wall[] Walls {
				get => _walls.ToArray();
			}
		// -- Public Attributes --
			/** Is true if the edge has been explored. */
			public bool IsExplored;

		// -- Protected Attributes --
		// -- Private Attributes --
			/** Couple of vertices marking the ends of the edge. */
			private Tuple<Vertex, Vertex> _vertices = new Tuple<Vertex, Vertex>(null, null);
			
			/** Couple of faces on both sides of the edge. */
			private Tuple<Face, Face> _faces = new Tuple<Face, Face>(null, null);

			/** List of walls along the edge. */
			private Generic.List<Wall> _walls = new Generic.List<Wall>();

	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --			
			/**
			 * Create a new edge object and specify its faces.
			 */
			public Edge (Tuple<Face, Face> faces) {
				_faces = new Tuple<Face, Face>(faces.Item1, faces.Item2);
			}
			
			public Edge (Face[] faces) {
				// Check the size of the array.
				if (faces.Length == 2) {
					// Set the items of the tuple.
					_faces = new Tuple<Face, Face>(faces[0], faces[1]);
				} else {
					throw new Exception("FATAL ERROR: Tried to instantiate edge with too many or too few faces: (" + faces.Length + ")");
				}
			}

			/**
			 * Create a new edge by specifying all its components.
			 */
			public Edge(Vertex start, Vertex end, Face sideA, Face sideB){
				// Set the vertex tuple.
				_vertices = new Tuple<Vertex, Vertex>(start, end);
				// Set the face tuple.
				_faces = new Tuple<Face, Face>(sideA, sideB);
			}

		// -- Public Methods --
			/**
			 * Add a new wall to the edge object.
			 */
			public void AddWall(Wall w){
				_walls.Add(w);
				w.Edge = this;
			}

			/**
			 * Set the edge's faces.
			 */
			
			/**
			 * Checks wether or not the edge is destroyed.
			 */
			public bool HasFallen() {
				// Prepare the flags.
				bool allDown = true;
				bool doorTaken = false;

				// Loop through the walls.
				foreach(Wall w in _walls) {
					// Check the health points of the wall.
					if(w.HP > 0) {
						allDown = false;
					} else if(w.IsDoor) {
						doorTaken = true;
					}
				}
				return allDown || doorTaken;
			}

			/**
			 * Merges a given edge into ourselves.
			 */
			public void Merge(Edge e) {
				// Check which vertex is common to both edges.
				Vertex common = GetCommonVertexWith(e);
				if (common != null) {
					// Check if the common vertex is a tower.
					if (!common.IsTower) {
						// Get the wall of the vertex.
						Tile.Wall vtxWall = common.Wall;

						// Add the wall to the list.
						AddWall(vtxWall);

						// Add the other walls to the list.
						foreach(Wall w in e.Walls){
							AddWall(w);
						}
					} else {
						throw new Exception("FATAL ERROR: Tried to merge two edges linked by a tower.");
					}
				} else {
					throw new Exception("FATAL ERROR: Tried to merge two unrelated edges.");
				}
			}

			/**
			 * Returns the vertex in common with the given edge.
			 */
			public Vertex GetCommonVertexWith(Edge other) {
				// Loop through the edges.
				foreach (Vertex selfVtx in Vertices)
					foreach (Vertex otherVtx in other.Vertices) 
						// If the vertices are the same.
						if (selfVtx == otherVtx)
							return selfVtx;
				
				// Return a null if nothing was found.
				return null;
			}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}