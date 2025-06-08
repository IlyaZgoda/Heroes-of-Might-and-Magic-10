using UnityEngine;

namespace Assets._Project.Code
{
    public class GridCellData
    {
        private readonly int _x;
        private readonly int _z;

        public bool IsOccupied { get; private set; }
        public GameObject CellObject { get; set; }

        public GridCellData(int x, int z, GameObject obj)
        {
            _x = x;
            _z = z;
            CellObject = obj;
            IsOccupied = false;
        }
    }
}
