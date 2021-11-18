using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

using Bletraut.Graphics.UI;

public class NewsWindow : MonoBehaviour
{
    [SerializeField]
    private PagedScrollRect pagedScroll;

    private List<GoodsData> goodsData = new List<GoodsData>();

    void Start()
    {
        GenerateGoodsData();
        pagedScroll.ItemsCount = goodsData.Count;
    }

    public void PageItemsLoaded(ReadOnlyCollection<PagedItem> items, int pageIndex)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].TryGetComponent(out Goods goods))
            {
                goods.SetData(goodsData[pagedScroll.ItemsPerPage * pageIndex + i]);
            }
        }
    }

    // --- Utils methods ---
    private void GenerateGoodsData()
    {
        for (int i = 0; i < 28; i++)
        {
            var goods = GoodsData.Random;
            goods.GoodsCount = i;
            goodsData.Add(goods);
        }
    }
}
