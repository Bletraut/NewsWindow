using UnityEngine;
using UnityEngine.UI;

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
        //GoodsImage.sprite = ???;

        userName.text = goodsData.UserName;
        userLevel.text = $"{goodsData.UserLevel}";
        //UserImage.sprite = ???;
    }
}