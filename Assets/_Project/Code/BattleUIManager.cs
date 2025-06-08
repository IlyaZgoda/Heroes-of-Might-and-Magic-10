using TMPro;
using UnityEngine;

namespace Assets._Project.Code
{
    public class BattleUIManager : MonoBehaviour
    {
        public TMP_Text RoundText;
        public TMP_Text WinnerText;

        private void Awake()
        {
            TurnManager.Instance.RoundStarted += SwitchRound;
            TurnManager.Instance.GameEnded += SetWinnerText;
        }
        public void SetMeleeAttack()
        {
            var unit = PlayerInput.Instance.GetControlledUnit();
            if (unit != null)
                unit.AttackType = AttackType.Melee;
        }

        public void SetRangedAttack()
        {
            var unit = PlayerInput.Instance.GetControlledUnit();
            if (unit != null)
            {
                unit.AttackType = AttackType.Ranged;
                Grid.Instance.HighlightAttackableCells(unit);
            }
                
        }

        public void SetMagicAttack()
        {
            var unit = PlayerInput.Instance.GetControlledUnit();
            if (unit != null)
                unit.AttackType = AttackType.Magic;
        }

        private void SwitchRound(int round)       
        {
            RoundText.text = "Ход " + round;
        }

        private void SetWinnerText(string winner)
        {
            WinnerText.text = winner;
        }
    }
}
