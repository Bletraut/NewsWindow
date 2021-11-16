using UnityEngine;

namespace Bletraut.Graphics.UI
{
    public class PagedItem : MonoBehaviour
    {
        public bool IsVisible { get; private set; } = true;

        private Vector2 savedScale = Vector2.zero;

        public void Show()
        {
            if (IsVisible) return;

            IsVisible = true;
            transform.localScale = savedScale;
        }

        public void Hide()
        {
            if (!IsVisible) return;

            savedScale = transform.localScale;
            IsVisible = false;
            transform.localScale = Vector2.zero;
        }
    }
}