using Godot;
using System;

using Generic = System.Collections.Generic;

namespace GMTK.Graph
{
/*
 * Computed face of the entire map.
 *
 * @author Caillaud Jean-Baptiste
 */
public class Face {
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
		// -- Public Attributes --
			/** List of points inside the room. */
			public Generic.List<Vector2> Points = new Generic.List<Vector2>();

			/** List of edges along the room. */
			public Generic.List<Edge> Edges = new Generic.List<Edge>();

			public Tile.Zone zone = new Tile.Zone();
		// -- Protected Attributes --
		// -- Private Attributes --
			/** Static face counter. */
			private static int Counter = 0;

			/** Unique face id. */
			public int id;
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
			public Face() {
				id = Counter++;
			}
		// -- Overrides --
			public override String ToString() {
				// Print the number of points.
				String output = " - Room #" + id + "\n";

				output += zone.ToString();

				return output;
			}
		// -- Operators --
		// -- Public Methods --
			/**
			 * Merge a face into this one.
			 */
			public void Merge(Face other) {

				// Add its points.
				Points.AddRange(other.Points);

				// Add its edges.
				foreach (Edge edge in other.Edges)
					AddEdge(edge);

				zone.Merge(other.zone);
			}

			public void AddPoint(Vector2 point) {
				if (!Points.Contains(point))
					Points.Add(point);
			}

			public void AddEdge(Edge edge) {
				if (!Edges.Contains(edge))
					Edges.Add(edge);
			}

			public bool HasPoint(Vector2 point) {
				foreach (Vector2 p in Points) {
					if (p.DistanceTo(point) < 1) return true;
				}
				
				return false;
			}
		// -- Protected Methods --
		// -- Private Methods --
	// --- /Methods ---

}
}