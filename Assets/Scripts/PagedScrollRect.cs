using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Bletraut.Graphics.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class PagedScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private PagedItem _itemPrefab;
        public PagedItem ItemPrefab
        {
            get => _itemPrefab;
            set
            {
                if (_itemPrefab != value)
                {
                    _itemPrefab = value;
                    Init();
                }
            }
        }
        [SerializeField]
        [Range(1, 100)]
        private int _itemsPerPage;
        public int ItemsPerPage => _itemsPerPage;
        [SerializeField]
        private float _swipeSpeed = 2500f;
        public float SwipeSpeed { get => _swipeSpeed; set => _swipeSpeed = value; }

        public ListAndIntEvent OnPageLoaded;

        private int _itemsCount = 0;
        public int ItemsCount
        {
            get => _itemsCount;
            set
            {
                if (_itemsCount != value)
                {
                    _itemsCount = Mathf.Max(0, value);
                    Init();
                }
            }
        }
        public int CurrentPageIndex { get; private set; } = 0;
        public int MaxPageIndex { get; private set; } = 0;
        private int targetPageIdex = 0;

        private ScrollRect scrollRect;
        private float scrollRectElasticity;

        private List<PagedItem> currentPage = new List<PagedItem>();
        private List<PagedItem> bufferPage = new List<PagedItem>();

        private bool isDrag = false;
        private Vector2 startDragPos;
        private PointerEventData pointerEventData;

        private void Init()
        {
            if (scrollRect == null)
            {
                scrollRect = GetComponent<ScrollRect>();
                scrollRectElasticity = scrollRect.elasticity;
                scrollRect.onValueChanged.AddListener(ScrollRect_ValueChanged);
            }

            Reset();

            currentPage = AddItems(_itemsPerPage);
            MaxPageIndex = Mathf.CeilToInt((float)ItemsCount / _itemsPerPage);

            SetItemsVisibility(currentPage, CurrentPageIndex);
            OnPageLoaded?.Invoke(currentPage.Where(n => n.IsVisible).ToList().AsReadOnly(), CurrentPageIndex);
        }

        public void SwipeNextPage()
        {
            if (canLoadNextPage) SwipePage(Vector2.left);
        }
        public void SwipePrevPage()
        {
            if (canLoadPrevPage) SwipePage(Vector2.right);
        }
        private void SwipePage(Vector2 direction)
        {
            scrollRect.elasticity = scrollRectElasticity;
            scrollRect.velocity = SwipeSpeed * direction;
        }

        public void OnPointerDown(PointerEventData eventData) => OnBeginDrag(eventData);
        public void OnPointerUp(PointerEventData eventData) => OnEndDrag(eventData);
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
            pointerEventData = eventData;
            startDragPos = eventData.position;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;

            var scrollValue = (float)System.Math.Round(scrollRect.horizontalNormalizedPosition, 3);
            var isMoved = scrollValue != 0 && scrollValue != 1;
            if (isMoved)
            {
                var dragValue = eventData.position - startDragPos;
                SwipePage(new Vector2(Mathf.Sign(dragValue.x), 0));
            }
        }

        // ScrollRect methods
        private void ScrollRect_ValueChanged(Vector2 scrollValue)
        {
            var prevPageIndex = CurrentPageIndex;
            var savedScrollVelocity = scrollRect.velocity;
            scrollValue.x = (float)System.Math.Round(scrollValue.x, 3);
            scrollValue.y = (float)System.Math.Round(scrollValue.y, 3);
            
            if (scrollValue.x >= 1) LoadNextPage();
            else if (scrollValue.x <= 0) LoadPrevPage();

            var isNewPageLoaded = scrollValue.x >= 1 || scrollValue.x <= 0;
            if (isNewPageLoaded && prevPageIndex == CurrentPageIndex)
            {
                scrollRect.velocity = savedScrollVelocity;
                if (scrollRect.velocity == Vector2.zero) scrollRect.elasticity = 0;
            }
        }
        private void SetScrollHorisontalPosition(float position)
        {
            if (isDrag) scrollRect.OnEndDrag(pointerEventData);

            scrollRect.horizontalNormalizedPosition = position;

            if (isDrag)
            {
                pointerEventData.position = Input.mousePosition;
                scrollRect.OnBeginDrag(pointerEventData);
            }
        }

        // Pages methods
        private bool canLoadNextPage => CurrentPageIndex < MaxPageIndex - 1;
        private bool canLoadPrevPage => CurrentPageIndex > 0;
        private void LoadNextPage()
        {
            if (targetPageIdex > CurrentPageIndex) CurrentPageIndex = Mathf.Min(MaxPageIndex - 1, targetPageIdex);

            if (canLoadNextPage)
            {
                if (bufferPage.Count == 0)
                {
                    bufferPage = AddItems(_itemsPerPage);
                }
                else
                {
                    SwapPageItems(ref currentPage, ref bufferPage);
                    bufferPage.ForEach(n => n.transform.SetAsLastSibling());
                    SetScrollHorisontalPosition(0f);
                }

                targetPageIdex = CurrentPageIndex + 1;
                SetItemsVisibility(bufferPage, targetPageIdex);
                OnPageLoaded?.Invoke(bufferPage.Where(n => n.IsVisible).ToList().AsReadOnly(), targetPageIdex);
            }
            else targetPageIdex = CurrentPageIndex - 1;
        }
        private void LoadPrevPage()
        {
            if (targetPageIdex < CurrentPageIndex) CurrentPageIndex = Mathf.Max(0, targetPageIdex);

            if (canLoadPrevPage)
            {
                SwapPageItems(ref currentPage, ref bufferPage);
                currentPage.ForEach(n =>
                {
                    n.Show();
                    n.transform.SetAsFirstSibling();
                });
                currentPage.Reverse();
                SetScrollHorisontalPosition(1f);

                targetPageIdex = CurrentPageIndex - 1;
                OnPageLoaded?.Invoke(currentPage.AsReadOnly(), targetPageIdex);
            }
            else targetPageIdex = CurrentPageIndex + 1;
        }
        private void SwapPageItems(ref List<PagedItem> from, ref List<PagedItem> to)
        {
            var temp = from;
            from = to;
            to = temp;
        }
        private void SetItemsVisibility(List<PagedItem> items, int pageIndex)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (targetPageIdex * ItemsPerPage + i >= ItemsCount) items[i].Hide();
                else items[i].Show();
            }
        }

        // Grid methods
        private List<PagedItem> AddItems(int count)
        {
            var items = new List<PagedItem>();
            for (int i = 0; i < count; i++) items.Add(Instantiate(_itemPrefab, scrollRect.content.transform));

            return items;
        }
        private void Reset()
        {
            CurrentPageIndex = targetPageIdex = 0;

            currentPage = new List<PagedItem>();
            bufferPage = new List<PagedItem>();

            var i = scrollRect.content.transform.childCount;
            while (i-- > 0) Destroy(scrollRect.content.transform.GetChild(i).gameObject);
        }
    }

    [System.Serializable]
    public class ListAndIntEvent : UnityEvent<ReadOnlyCollection<PagedItem>, int> { }
}