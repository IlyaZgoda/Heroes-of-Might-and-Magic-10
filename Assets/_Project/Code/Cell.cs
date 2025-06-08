using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Project.Code
{
    public class Cell : MonoBehaviour
    {
        private Renderer rend;
        private Color originalOutlineColor;
        private Color currentHighlightColor;

        public Color hoverColor = Color.yellow;
        public Color moveHighlightColor = Color.cyan;
        public Color attackHighlightColor = Color.red;

        private bool isHighlighted = false;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            originalOutlineColor = rend.material.GetColor("_OutColor");
        }

        private void OnMouseEnter()
        {
            if (TurnManager.Instance.IsPlayerTurn())
            {
                rend.material.SetColor("_OutColor", hoverColor);
            }
        }

        private void OnMouseExit()
        {
            if (TurnManager.Instance.IsPlayerTurn())
            {
                if (isHighlighted) 
                    rend.material.SetColor("_OutColor", currentHighlightColor);
                else
                    rend.material.SetColor("_OutColor", originalOutlineColor);
            }
        }

        public void HighlightMove()
        {
            rend.material.SetColor("_OutColor", moveHighlightColor);
            isHighlighted = true;
            currentHighlightColor = moveHighlightColor;
        }

        public void HighlightShootAttack()
        {
            rend.material.SetColor("_OutColor", attackHighlightColor);
            isHighlighted = true;
            currentHighlightColor = attackHighlightColor;
        }

        public void ClearHighlight()
        {
            rend.material.SetColor("_OutColor", originalOutlineColor);
            isHighlighted = false;
        }
    }
}
