using Godot;
using System;

using Generic = System.Collections.Generic;

namespace GMTK.Pathfinder
{
/*
 * AStar algorithm implementation.
 *
 * @author Caillaud Jean-Baptiste
 */
public static class AStar {
	// ---  Subclasses ---
		// -- Classes --
			private class AStarComparer : Generic.IComparer<Vector2> {
				public static AStarComparer Instance = new AStarComparer();

				public int Compare(Vector2 a, Vector2 b) {
					// Get the values.
					float aVal = _map[(int)a.x, (int)a.y];
					float bVal = _map[(int)b.x, (int)b.y];

					if (aVal < bVal)
						return -1;
					else if (aVal > bVal)
						return 1;
					else
						return 0;
				}
			}
	// --- /Subclasses ---
	
	// ---  Attributes ---
		// -- Exported --
		// -- Properties --
		// -- Public Attributes --
		// -- Private Attributes --
			/** Map of all the cells and their weight. */
			private static float[,] _map = new float[,] {};

			/** Width accessor. */
			private static int _w {
				get => _map.GetLength(0);
			}

			/** Height accessor. */
			private static int _h {
				get => _map.GetLength(1);
			}

			/** List of explored paths. */
			private static Generic.List<Vector2> _closed;

			/** List of directions to check next. */
			private static Generic.List<Vector2> _opened;

			/** Lists the nodes costs from the starting point. */
			private static float[,] _startCosts;

			/** Lists the nodes costs to the destination point. */
			private static float[,] _endCosts;

			/** List of the origin vectors of each point. */
			private static Vector2[,] _origins;
	// --- /Attributes ---

	// ---  Methods ---
		// -- Constructor --
		// -- Overrides --
		// -- Operators --
		// -- Public Methods --
			/**
			 * Add the given cost to the map.
			 */
			public static void AddCost(int x, int y, float weight) {
				// Resize the array.
				Resize(x, y);

				// Add the value to the array.
				_map[x, y] += weight;
			}

			/**
			 * Compute a path to the requested point.
			 */
			public static Tuple<Vector2[], float> ComputePath(Vector2 from, Vector2 to) {
				// Check boundaries.
				Rect2 bounds = new Rect2(0, 0, _w, _h);
				if (bounds.HasPoint(from) && bounds.HasPoint(to)) {
					// Create the lists.
					_closed = new Generic.List<Vector2>();
					_opened = new Generic.List<Vector2>();
					_startCosts = new float[_w, _h]; for(int i = 0; i < _w; i++) for (int j = 0; j < _h; j++) _startCosts[i, j] = float.PositiveInfinity;
					_endCosts = new float[_w, _h]; for(int i = 0; i < _w; i++) for (int j = 0; j < _h; j++) _endCosts[i, j] = float.PositiveInfinity;
					_origins = new Vector2[_w, _h];

					_opened.Add(from);
					_startCosts[(int)from.x, (int)from.y] = 0;
					_endCosts[(int)from.x, (int)from.y] = (to - from).Length();

					// Compute the path.
					if (_FindPath(to)) {
						return new Tuple<Vector2[], float>(_BuildPath(from, to), _startCosts[(int)to.x, (int)to.y]);
					} else {
						return null;
					}
				} else {
					return null;
				}
			}
		// -- Private Methods --
			/**
			 * Resize the array to contain the given point.
			 */
			private static void Resize(int x, int y) {
				// Get the limits of the new array.
				int limX = (x >= _w) ? x + 1 : _w;
				int limY = (y >= _h) ? y + 1 : _h;

				// If the limits require a resize.
				if (limX > _w || limY > _h) {
					// Resize the array.
					float[,] newArr = new float[limX,limY];

					// Copy the values over.
					for (int i = 0; i < _w; i++)
						for (int j = 0; j < _h; j++)
							newArr[i, j] = _map[i, j];
					
					// Change the reference.
					_map = newArr;
				}
			}

			/**
			 * Computes the next step in the a* algorithm.
			 */
			private static bool _FindPath(Vector2 destination) {
				// Prepare the return value.
				Generic.List<Vector2> nextSteps = new Generic.List<Vector2>();

				// Loop until the open map is filled.
				while (_opened.Count > 0) {
					// Sort the array.
					_opened.Sort(AStarComparer.Instance);

					// Get the next element in the list.
					Vector2 current = _opened[0];

					// If the current node is the finish line.
					if (current == destination)
						// Stop the method.
						return true;

					// Add it to the closed list.
					_opened.Remove(current);
					_closed.Add(current);

					// Look around the cell.
					Vector2[] directions = new Vector2[] {
						Vector2.Up, Vector2.Right, Vector2.Down, Vector2.Left
					};
					foreach(Vector2 dir in directions) {
						Vector2 pos = current + dir;

						// Check the boundaries.
						Rect2 bounds = new Rect2(0, 0, _w, _h);
						if (bounds.HasPoint(pos)) {
							// If the cell wasn't already checked.
							if (!_closed.Contains(pos)) {
								// Compute the distance to the start.
								float cost = _startCosts[(int)current.x, (int)current.y] + _map[(int)pos.x, (int)pos.y];

								// If the node is not in the opened set.
								if (!_opened.Contains(pos)) {
									// Add it.
									_opened.Add(pos);
								}
								// If the cost is less that the start cost.
								if (cost < _startCosts[(int)pos.x, (int)pos.y]) {
									// Record the path.
									_origins[(int)pos.x, (int)pos.y] = current;
									_startCosts[(int)pos.x, (int)pos.y] = cost;
									_endCosts[(int)pos.x, (int)pos.y] = _startCosts[(int)current.x, (int)current.y] + (destination - pos).Length();
								}
							}
						}
					}
				}

				// No path was found.
				return false;
			}

			/**
			 * Builds the path from the current state of the AStar.
			 */
			private static Vector2[] _BuildPath(Vector2 from, Vector2 to) {
				// Create the list.
				Generic.List<Vector2> path = new Generic.List<Vector2>();

				// Follow all the origin vectors.
				Vector2 current = to;
				while (current != from) {
					// Add the vector to the list.
					path.Add(current);

					// Go to the origin.
					current = _origins[(int)current.x, (int)current.y];
				}

				// Reverse the path.
				path.Reverse();
				// Return the list.
				return path.ToArray();
			}
	// --- /Methods ---

}
}