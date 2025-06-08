using UnityEngine;

namespace Assets._Project.Code
{
    public class CompositionRoot : MonoBehaviour
    {
        private TurnManager _turnManager;

        private void Awake()
        {
            Grid grid = new();
            UnitSpawner spawner = new();
            Pathfinding pathfinding = new();
            UnitManager unitManager = new();

            grid.Construct();
            spawner.Construct(grid);
            pathfinding.Construct(grid);
            unitManager.Construct();

            grid.GenerateGrid();
            spawner.SpawnUnits();
        }
    }
}
