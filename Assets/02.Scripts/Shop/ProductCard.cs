using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image productImage;

    public int productId;
    public int price;
    public bool isSold = false;

    public void CardRefresh(ProductData productData) 
    {
        if (productData == null) 
        {
            return; 
        }

        nameText.text = productData.productName;
        descriptionText.text = productData.description;
        price = productData.price;
        priceText.text = price.ToString();
        productImage.sprite = productData.sprite;
        productId = productData.id;
    }

    public void Purchase() 
    {
        if (isSold) { return; }
        GameManager.Instance.PurchaseCharacter(this);
    }

    public void SoldOut() 
    {
        priceText.text = "SoldOut";
        isSold = true;
    }
}
