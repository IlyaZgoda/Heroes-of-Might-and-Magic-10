using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Code
{
    public class Pathfinding
    {
        public Grid _grid;
        public static Pathfinding Instance;
        
        public void Construct(Grid grid)
        {
            _grid = grid;
            Instance = this;
        }
            

        public List<Vector2Int> FindPath(int startX, int startZ, int targetX, int targetZ)
        {
            List<PathNode> openList = new();
            HashSet<Vector2Int> closedSet = new();
            PathNode start = new(startX, startZ);
            PathNode target = new(targetX, targetZ);

            start.GCost = 0;
            start.HCost = Heuristic(start, target);

            openList.Add(start);

            while (openList.Count > 0)
            {
                PathNode current = GetLowestFCost(openList);

                if (current.X == target.X && current.Z == target.Z)
                    return ReconstructPath(current);

                openList.Remove(current);
                closedSet.Add(current.Pos);

                foreach (Vector2Int dir in GetDirections())
                {
                    int nx = current.X + dir.x;
                    int nz = current.Z + dir.y;

                    Vector2Int nextPos = new(nx, nz);

                    if (closedSet.Contains(nextPos)) continue;
                    if (!_grid.IsCellWalkable(nx, nz)) continue;

                    int newGCost = current.GCost + 1;

                    PathNode neighbor = openList.Find(n => n.X == nx && n.Z == nz);

                    if (neighbor == null)
                    {
                        neighbor = new PathNode(nx, nz)
                        {
                            GCost = newGCost
                        };
                        neighbor.HCost = Heuristic(neighbor, target);
                        neighbor.Parent = current;
                        openList.Add(neighbor);
                    }

                    else if (newGCost < neighbor.GCost)
                    {
                        neighbor.GCost = newGCost;
                        neighbor.Parent = current;
                    }
                }
            }

            return new List<Vector2Int>(); 
        }

        private PathNode GetLowestFCost(List<PathNode> nodes)
        {
            PathNode best = nodes[0];

            foreach (var node in nodes)
            {
                if (node.FCost < best.FCost || (node.FCost == best.FCost && node.HCost < best.HCost))
                    best = node;
            }

            return best;
        }

        private int Heuristic(PathNode a, PathNode b)
        {
            return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Z - b.Z); 
        }

        private List<Vector2Int> ReconstructPath(PathNode endNode)
        {
            List<Vector2Int> path = new();
            PathNode current = endNode;

            while (current != null)
            {
                path.Add(new Vector2Int(current.X, current.Z));
                current = current.Parent;
            }

            path.Reverse();

            return path;
        }

        private Vector2Int[] GetDirections()
        {
            return new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
        }
    }
}
