using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Code
{
    public class UnitManager 
    {
        public static UnitManager Instance { get; private set; }

        public List<Unit> PlayerUnits { get; private set; } = new List<Unit>();
        public List<Unit> EnemyUnits { get; private set; } = new List<Unit>();


        public void Construct()
        {
            if (Instance != null)
            {
                return;
            }
            Instance = this;
        }
        public void Register(Unit unit)
        {
            Debug.Log(PlayerUnits.Concat(EnemyUnits).ToList().Count);
            if (unit.IsPlayer && !PlayerUnits.Contains(unit))
                PlayerUnits.Add(unit);
            else if (unit.IsEnemy && !EnemyUnits.Contains(unit))
                EnemyUnits.Add(unit);
        }

        public void Unregister(Unit unit)
        {
            if (unit.IsPlayer)
                PlayerUnits.Remove(unit);
            else if (unit.IsEnemy)
                EnemyUnits.Remove(unit);
        }

        public List<Unit> GetAllUnits()
        {
            Debug.Log("GetAllUnits called");
            Debug.Log(PlayerUnits.Concat(EnemyUnits).ToList().Count);
            return PlayerUnits.Concat(EnemyUnits).ToList();
        }

        public Unit FindClosestPlayerUnit(Unit fromUnit)
        {
            Unit closest = null;
            int minDist = int.MaxValue;

            foreach (var player in PlayerUnits)
            {
                if (!player.IsAlive)
                    continue;

                int dist = Mathf.Abs(fromUnit.GridX - player.GridX) + Mathf.Abs(fromUnit.GridZ - player.GridZ);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = player;
                }
            }

            return closest;
        }

        public bool IsCellOccupied(int x, int z)
        {
            foreach (var unit in PlayerUnits)
            {
                if (unit.IsAlive && unit.GridX == x && unit.GridZ == z)
                    return true;
            }
            foreach (var unit in EnemyUnits)
            {
                if (unit.IsAlive && unit.GridX == x && unit.GridZ == z)
                    return true;
            }
            return false;
        }

        public string GetWinner()
        {
            var player = PlayerUnits.Sum(x => x.Health);
            var enemy = EnemyUnits.Sum(x => x.Health);

            if (player == enemy)
            {
                return "ничья";
            }

            return player > enemy ? "победили синие" : "победили красные";
        }
    }
}
