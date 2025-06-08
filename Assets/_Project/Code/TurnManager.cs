using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Project.Code
{
    public enum TurnPhase
    {
        PlayerTurn,
        EnemyTurn
    }

    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        private readonly Queue<Unit> _turnQueue = new();
        private List<Unit> _unitsInCurrentRound = new();
        private Unit _currentUnit;
        private int _currentRound = 0;
        public event Action<int> RoundStarted;
        public event Action<string> GameEnded;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public void Start()
        {
            InitializeTurnOrder();
            RoundStarted += FinishGame;
        }

        private void FinishGame(int round)
        {
            if (round == 10)
            {
                Debug.Log("Игра завершена");
                PlayerInput.Instance.enabled = false;
                EnemyAI.Instance.enabled = false;
                enabled = false;
                var winner = UnitManager.Instance.GetWinner();

                GameEnded?.Invoke(winner);
            }
        }

        private void InitializeTurnOrder()
        {
            _turnQueue.Clear();
            _unitsInCurrentRound.Clear();

            List<Unit> allUnits = UnitManager.Instance.GetAllUnits();

            foreach (var unit in allUnits)
            {
                if (unit.IsAlive)
                {
                    unit.ResetTurn();
                    _turnQueue.Enqueue(unit);
                    _unitsInCurrentRound.Add(unit);
                }
            }

            _currentRound++;
            Debug.Log($"Раунд {_currentRound} начинается");

            RoundStarted?.Invoke(_currentRound); 

            if (_turnQueue.Count == 0)
            {
                Debug.LogWarning("Нет живых юнитов — конец боя?");
                return;
            }

            StartNextTurn();
        }
        private void StartNextTurn()
        {
            while (_turnQueue.Count > 0)
            {
                _currentUnit = _turnQueue.Dequeue();

                if (_currentUnit != null && _currentUnit.IsAlive)
                {
                    break;
                }
            }

            if (_currentUnit == null || !_currentUnit.IsAlive)
            {
                Debug.LogWarning("Не удалось найти следующего активного юнита");
                return;
            }

            Debug.Log($"Начинается ход: {_currentUnit.name}");

            Indicator.Instance.Follow(_currentUnit.gameObject);
            Grid.Instance.HighlightReachableCells(_currentUnit);

            if (_currentUnit.IsPlayer)
            {
                PlayerInput.Instance.StartPlayerTurn(_currentUnit);
            }
            else
            {
                EnemyAI.Instance.ExecuteTurn(_currentUnit);
            }
        }

        public void EndCurrentTurn()
        {
            _unitsInCurrentRound.Remove(_currentUnit);

            if (_unitsInCurrentRound.Count == 0)
            {
                Debug.Log("Раунд завершён.");
                InitializeTurnOrder(); 
            }
            else
            {
                StartNextTurn(); 
            }
        }
        public void FinishUnitDueToDeath(Unit unit)
        {
            if (_currentUnit == unit)
            {
                Debug.Log($"{unit.name} умер во время своего хода.");
                EndCurrentTurn(); 
            }
            else if (_unitsInCurrentRound.Contains(unit))
            {
                Debug.Log($"{unit.name} умер до своего хода.");
                _unitsInCurrentRound.Remove(unit);
            }
        }
        public Unit GetCurrentUnit() => _currentUnit;
        public bool IsPlayerTurn() => _currentUnit != null && _currentUnit.IsPlayer;
        public Unit GetActivePlayerUnit() => _currentUnit.IsPlayer ? _currentUnit : null;
        public void FinishActiveUnitTurn() => EndCurrentTurn();
    }
}
