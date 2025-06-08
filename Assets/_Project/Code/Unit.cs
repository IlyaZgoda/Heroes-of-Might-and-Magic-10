using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Project.Code
{
    public class Unit : MonoBehaviour
    {
        public int GridX { get; set; }
        public int GridZ { get; set; }

        private readonly float _cellSize = 1f;
        private readonly float _moveSpeed = 5f;

        private readonly Queue<Vector3> _pathQueue = new();
        private int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                _health = value;
                _hpText.text = _health.ToString();
            }
        }

        public bool IsMoving { get; private set; } = false;

        public UnitFaction Faction { get; set; }
        public bool IsPlayer => Faction == UnitFaction.Player;
        public bool IsEnemy => !IsPlayer;
        public bool IsAlive => _health > 0;

        public AttackType AttackType { get; set; } = AttackType.Melee;
        public int AttackDamage { get; set; }
        public int AttackRange { get; set; } = 1;
        public int ShootRange { get; set; } = 3;
        public int MagicRange { get; set; } = 5;

        public bool HasMoved { get; set; } = false;
        public bool HasAttacked { get; set; } = false;

        public int MaxMovementPoints { get; private set; } = 5;
        public int CurrentMovementPoints { get; private set; }

        public Action OnActionComplete;
        private TMP_Text _hpText;
        private GameObject _projectilePrefab;

        private void Awake()
        {
            _hpText = GetComponentInChildren<TMP_Text>();
            _projectilePrefab = Resources.Load<GameObject>("Projectile");

        }
        private void Update()
        {
            if (IsMoving && _pathQueue.Count > 0)
            {
                Vector3 target = _pathQueue.Peek();
                transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, target) < 0.01f)
                {
                    transform.position = target;
                    _pathQueue.Dequeue();

                    if (_pathQueue.Count == 0)
                    {
                        IsMoving = false;
                        GridX = Mathf.RoundToInt(target.x / _cellSize);
                        GridZ = Mathf.RoundToInt(target.z / _cellSize);
                        HasMoved = true;

                        OnActionComplete?.Invoke();
                        OnActionComplete = null;
                    }
                }
            }
        }

        public void SetPath(List<Vector2Int> fullPath)
        {
            _pathQueue.Clear();

            int steps = Mathf.Min(CurrentMovementPoints, fullPath.Count);

            for (int i = 0; i < steps; i++)
            {
                _pathQueue.Enqueue(GridToWorld(fullPath[i].x, fullPath[i].y));
            }

            if (_pathQueue.Count > 0)
            {
                IsMoving = true;
                CurrentMovementPoints -= steps;
                HasMoved = true;
            }
        }

        private Vector3 GridToWorld(int x, int z)
        {
            return new Vector3(x * _cellSize, 0.5f, z * _cellSize);
        }

        public void PerformAttackWithType(Unit target, AttackType type)
        {
            if (HasAttacked || !IsAlive || target == null || !target.IsAlive)
            {
                OnActionComplete?.Invoke();
                OnActionComplete = null;
                return;
            }

            int distance = Mathf.Abs(GridX - target.GridX) + Mathf.Abs(GridZ - target.GridZ);

            bool canAttack = type switch
            {
                AttackType.Melee => distance <= AttackRange,
                AttackType.Ranged => distance <= ShootRange,
                AttackType.Magic => distance <= MagicRange,
                _ => false
            };

            if (!canAttack)
            {
                Debug.Log("Цель вне досягаемости");
                Grid.Instance.HighlightReachableCells(this);
                return;
            }

            switch (type)
            {
                case AttackType.Melee:
                    PerformMeleeAttack(target);
                    break;
                case AttackType.Ranged:
                    PerformRangedAttack(target);
                    break;
                case AttackType.Magic:
                    PerformMagicAttack(target);
                    break;
            }

            HasAttacked = true;
            OnActionComplete?.Invoke();
            OnActionComplete = null;
        }

        private void PerformMeleeAttack(Unit target)
        {
            Debug.Log($"{name} strikes {target.name} with melee.");
            target.TakeDamage(AttackDamage);
        }

        private void PerformRangedAttack(Unit target)
        {
            Debug.Log($"{name} shoots at {target.name}.");
            GameObject projectileGO = Instantiate(_projectilePrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            projectile.Initialize(target, AttackDamage);        
        }

        private void PerformMagicAttack(Unit target)
        {
            Debug.Log($"{name} casts a spell on {target.name}.");
            target.TakeDamage(AttackDamage);
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            Debug.Log($"{name} took {amount} damage. Remaining: {_health}");

            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{name} has died.");
            UnitManager.Instance.Unregister(this);

            TurnManager.Instance?.FinishUnitDueToDeath(this); 

            OnActionComplete?.Invoke();  
            OnActionComplete = null;

            Destroy(gameObject);
        }

        public void ResetTurn()
        {
            HasMoved = false;
            HasAttacked = false;
            CurrentMovementPoints = MaxMovementPoints;
        }

        public bool CanMoveTo(int cost) => cost <= CurrentMovementPoints;
    }
}

