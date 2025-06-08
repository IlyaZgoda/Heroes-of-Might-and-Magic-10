using UnityEngine;

namespace Assets._Project.Code
{
    public class PathNode
    {
        public int X { get; private set; }
        public int Z { get; private set; }
        public int GCost { get; set; } 
        public int HCost { get; set; } 
        public int FCost => GCost + HCost;

        public PathNode Parent { get; set; }

        public PathNode(int x, int z)
        {
            X = x;
            Z = z;
        }

        public Vector2Int Pos => new(X, Z);
    }
}
