using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Code
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance;

        private Unit _controlledUnit;

        public void Awake() => Instance = this;

        public void StartPlayerTurn(Unit unit)
        {
            _controlledUnit = unit;

            Debug.Log($"Игрок управляет: {unit.name}");
        }

        private void Update()
        {
            if (_controlledUnit == null || !_controlledUnit.IsPlayer)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clicked = hit.collider.gameObject;

                    if (clicked.name.StartsWith("Cell_") && !_controlledUnit.HasMoved)
                    {
                        var coords = clicked.name.Split('_');
                        int x = int.Parse(coords[1]);
                        int z = int.Parse(coords[2]);

                        var path = Pathfinding.Instance.FindPath(_controlledUnit.GridX, _controlledUnit.GridZ, x, z);

                        if (path.Count > 0)
                        {
                            _controlledUnit.OnActionComplete = () => TurnManager.Instance.EndCurrentTurn();
                            _controlledUnit.SetPath(path);
                        }
                    }

                    if (clicked.CompareTag("Unit") && !_controlledUnit.HasAttacked)
                    {
                        Unit target = clicked.GetComponent<Unit>();
                        if (target != null && target.IsEnemy)
                        {
                            _controlledUnit.OnActionComplete = () => TurnManager.Instance.EndCurrentTurn();
                            _controlledUnit.PerformAttackWithType(target, _controlledUnit.AttackType);
                        }
                    }
                }
            }
        }

        public Unit GetControlledUnit() => _controlledUnit;
    }
}
