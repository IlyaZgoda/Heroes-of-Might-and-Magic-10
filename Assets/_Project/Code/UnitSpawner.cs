using UnityEngine;

namespace Assets._Project.Code
{
    public class UnitSpawner
    {
        private Grid _grid;

        public void Construct(Grid grid) =>
            _grid = grid;

        private void SpawnUnitAt(int x, int z, UnitFaction faction, GameObject prefab)
        {
            Vector3 spawnPos = new(x * _grid.CellSize, 0.5f, z * _grid.CellSize);
            GameObject unitGameObject = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
            unitGameObject.name = $"Unit_{x}_{z}";

            if (unitGameObject.TryGetComponent<Unit>(out var unitScript))
            {
                unitScript.Health = 100;
                unitScript.GridX = x;
                unitScript.GridZ = z;
                unitScript.Faction = faction;
                unitScript.AttackDamage = 15;
                unitScript.AttackRange = 1;
                unitScript.ShootRange = 3;
               
            }
            var unit = unitGameObject.GetComponent<Unit>();
            var materail = unit.GetComponent<Renderer>();
            if (unit.IsPlayer)
            {
                materail.material.color = Color.blue;
            }
            else
            {
                materail.material.color = Color.red;
            }

            UnitManager.Instance.Register(unit);
        }

        public void SpawnUnits()
        {
            Debug.Log("Unit");
            GameObject prefab = Resources.Load<GameObject>("Unit");

            SpawnUnitAt(0, 0, UnitFaction.Player, prefab);
            SpawnUnitAt(0, 1, UnitFaction.Player, prefab);
            SpawnUnitAt(1, 1, UnitFaction.Player, prefab);
            SpawnUnitAt(9, 9, UnitFaction.Enemy, prefab);
            SpawnUnitAt(9, 8, UnitFaction.Enemy, prefab);
            SpawnUnitAt(8, 8, UnitFaction.Enemy, prefab);
        }
    }
}
