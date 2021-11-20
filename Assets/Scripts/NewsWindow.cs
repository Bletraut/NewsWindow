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

    private GoodsData[] goodsData;
    private string goodsDataLink = "https://raw.githubusercontent.com/Bletraut/NewsWindow/main/Data/goods.json";

    void Start()
    {
        StartCoroutine(LoadGoodsData(goodsDataLink));
    }

    private IEnumerator LoadGoodsData(string dataLink)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(dataLink))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                goodsData = JsonUtility.FromJson<GoodsArray<GoodsData>>(webRequest.downloadHandler.text).Goods;
                pagedScroll.ItemsCount = goodsData.Length;
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

    public void Close()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

[System.Serializable]
public class GoodsArray<T>
{
    public T[] Goods;
}
