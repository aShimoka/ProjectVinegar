using Godot;
using System;
using Generic = System.Collections.Generic;

namespace GMTK.Graph
{
/*
 * 
 *
 * @author 
 */
public class Vertex {
	// ---  Attributes ---
		// -- Exported --
		// -- Public Attributes --
			/** Position accessor. */
			public Vector2 Position { get; private set; }

			/** Tower status accessor. */
			public bool IsTower { get => (Wall == null); }

			/** Wall accessor. */
			public Tile.Wall Wall { get; private set; }

			/** List of edges along this vertex. */
			public Generic.List<Edge> Edges { get; private set; } = new Generic.List<Edge>();
		// -- Protected Attributes --
		// -- Private Attributes --
	// --- /Attributes --

	// ---  Methods ---
		// -- Constructor --
		public Vertex(int x, int y, Tile.Wall wall = null) {
			Position = new Vector2(x, y);
			Wall = wall;
		}

		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}