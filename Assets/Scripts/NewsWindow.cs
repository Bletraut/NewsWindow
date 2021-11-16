using System.Linq;
using System.Collections;
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

public struct GoodsData
{
    public string GoodsName;
    public int GoodsCount;
    public int GoodsPrice;
    public string GoodsImage;

    public string UserName;
    public int UserLevel;
    public string UserImage;

    public GoodsData(string goodsName, int goodsCount, int goodsPrice, string goodsImage, string userName, int userLevel, string userImage)
    {
        GoodsName = goodsName;
        GoodsCount = goodsCount;
        GoodsPrice = goodsPrice;
        GoodsImage = goodsImage;

        UserName = userName;
        UserLevel = userLevel;
        UserImage = userImage;
    }

    public override string ToString()
    {
        return $"{{ GoodsName={GoodsName}, GoodsName={GoodsName},"
                + $" GoodsPrice={GoodsPrice} GoodsImage={GoodsImage},"
                + $" UserName={UserName}, UserLevel = {UserLevel}, UserImage={UserImage} }}";
    }

    private const string symbols = "ABCDEFJHIJKLMNOPQRSTUVWXYZ0123456789";
    public static GoodsData Random => new GoodsData()
    {
        GoodsName = new string(symbols.OrderByDescending(n => UnityEngine.Random.value).Take(UnityEngine.Random.Range(1, 10)).ToArray()),
        GoodsCount = UnityEngine.Random.Range(1, 100),
        GoodsPrice = UnityEngine.Random.Range(1, 1000),
        GoodsImage = new string(symbols.OrderByDescending(n => UnityEngine.Random.value).ToArray()),

        UserName = new string(symbols.OrderByDescending(n => UnityEngine.Random.value).Take(UnityEngine.Random.Range(1, 10)).ToArray()),
        UserLevel = UnityEngine.Random.Range(1, 100),
        UserImage = new string(symbols.OrderByDescending(n => UnityEngine.Random.value).ToArray())
    };
}
