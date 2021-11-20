using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;
using UnityEngine.Networking;

using Bletraut.Graphics.UI;

public class NewsWindow : MonoBehaviour
{
    [SerializeField]
    private PagedScrollRect pagedScroll;

    private List<GoodsData> goodsData = new List<GoodsData>();
    private string goodsDataLink = "https://raw.githubusercontent.com/Bletraut/NewsWindow/main/Data/goods.json";

    void Start()
    {
        StartCoroutine(LoadGoodsData(goodsDataLink));

        //pagedScroll.ItemsCount = goodsData.Count;
    }

    private IEnumerator LoadGoodsData(string dataLink)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(dataLink))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var goodsList = JsonUtility.FromJson<DataArray<GoodsData>>(webRequest.downloadHandler.text);
                Debug.Log(goodsList);
            }
            else
            {
                Debug.LogError("Unabled to load goods data");
            }
        }
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
}

[System.Serializable]
public class DataArray<T>
{
    public T[] Goods;
}
