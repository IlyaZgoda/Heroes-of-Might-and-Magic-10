using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Code
{
    public class Projectile : MonoBehaviour
    {
        public float Speed = 10f;
        public int Damage;
        private Unit _target;

        public void Initialize(Unit target, int damage)
        {
            _target = target;
            Damage = damage;
        }

        void Update()
        {
            if (_target == null || !_target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.position += direction * Speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, _target.transform.position) < 0.3f)
            {
                _target.TakeDamage(Damage);
                Destroy(gameObject);
            }
        }
    }
}
