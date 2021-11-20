using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using TMPro;

public class Goods : MonoBehaviour
{
    [SerializeField]
    private TMP_Text goodsName;
    [SerializeField]
    private TMP_Text goodsCount;
    [SerializeField]
    private TMP_Text goodsPrice;
    [SerializeField]
    private Image goodsImage;

    [SerializeField]
    private TMP_Text userName;
    [SerializeField]
    private TMP_Text userLevel;
    [SerializeField]
    private Image userImage;
    [SerializeField]
    private Sprite defaultUserSprite;

    public void Hide()
    {
        transform.localScale = Vector3.zero;
    }
    public void Show()
    {
        transform.localScale = Vector3.one;
    }

    public void SetData(GoodsData goodsData)
    {
        goodsName.text = goodsData.GoodsName;
        goodsCount.text = $"x{goodsData.GoodsCount}";
        goodsPrice.text = $"{goodsData.GoodsPrice}";
        Addressables.LoadAssetAsync<Sprite>(goodsData.GoodsImage).Completed += GoodsImage_Loaded;

        userName.text = goodsData.UserName;
        userLevel.text = $"{goodsData.UserLevel}";
        StartCoroutine(LoadUserImage(goodsData.UserImage));
        //UserImage.sprite = ???;
    }

    private void GoodsImage_Loaded(AsyncOperationHandle<Sprite> obj)
    {
        goodsImage.sprite = obj.Result;
    }
    private IEnumerator LoadUserImage(string imageLink)
    {
        userImage.sprite = defaultUserSprite;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(imageLink))
        {
            webRequest.downloadHandler = new DownloadHandlerTexture(true);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                userImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }
}

[System.Serializable]
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
}