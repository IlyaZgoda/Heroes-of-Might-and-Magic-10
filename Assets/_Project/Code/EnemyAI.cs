using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Code
{
    public class EnemyAI : MonoBehaviour
    {
        public static EnemyAI Instance;
        public float Delay = 0.5f;

        private void Awake() => Instance = this;

        public void ExecuteTurn(Unit enemyUnit)
        {
            StartCoroutine(EnemyRoutine(enemyUnit, () => TurnManager.Instance.EndCurrentTurn()));
        }
        private IEnumerator EnemyRoutine(Unit unit, System.Action onComplete)
        {
            yield return new WaitForSeconds(Delay);

            Unit target = UnitManager.Instance.FindClosestPlayerUnit(unit);
            if (target == null)
            {
                onComplete?.Invoke();
                yield break;
            }

            int dist = Mathf.Abs(unit.GridX - target.GridX) + Mathf.Abs(unit.GridZ - target.GridZ);

            if (dist <= unit.AttackRange)
            {
                unit.OnActionComplete = onComplete;
                unit.PerformAttackWithType(target, AttackType.Melee);
            }

            else if (dist <= unit.ShootRange)
            {
                unit.OnActionComplete = onComplete;
                unit.PerformAttackWithType(target, AttackType.Ranged);
            }
            
            else
            {
                List<Vector2Int> possibleTargets = new List<Vector2Int>();
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (Mathf.Abs(dx) + Mathf.Abs(dz) == 1) 
                        {
                            int nx = target.GridX + dx;
                            int nz = target.GridZ + dz;

                            if (!UnitManager.Instance.IsCellOccupied(nx, nz))
                            {
                                possibleTargets.Add(new Vector2Int(nx, nz));
                            }
                        }
                    }
                }

                List<Vector2Int> path = null;

                foreach (var pos in possibleTargets)
                {
                    var testPath = Pathfinding.Instance.FindPath(unit.GridX, unit.GridZ, pos.x, pos.y);
                    if (testPath.Count > 0)
                    {
                        path = testPath;
                        break;
                    }
                }

                if (path == null || path.Count == 0)
                {
                    onComplete?.Invoke();
                    yield break;
                }

                unit.OnActionComplete = onComplete;
                unit.SetPath(path);
            }
        }
    }
}
