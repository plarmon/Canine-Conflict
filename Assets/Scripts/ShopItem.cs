using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour
{
    private ShopManager.ShopPawn pawn;
    private ShopManager sm;

    [SerializeField] private TextMeshProUGUI dogName;
    [SerializeField] private TextMeshProUGUI dogCost;
    [SerializeField] private Image dogImage;
    [SerializeField] public Button button;

    // Start is called before the first frame update
    void Start()
    {
        sm = GameObject.Find("Shop").GetComponent<ShopManager>();
    }

    /*
     * Instantiates a new shop item
     * 
     * @param name - The name of the shop item
     * @param cost - The cost of the shop item
     * @param image - The image of the shop item
     * @param pawnPrefab - The object prefab of the shop item
     */
    public void instantiate(string name, int cost, Texture2D image, GameObject pawnPrefab)
    {
        pawn.name = name;
        pawn.cost = cost;
        pawn.pawnPic = image;
        pawn.pawnPrefab = pawnPrefab;
        pawn.owningSlot = gameObject;

        dogName.text = name;
        dogCost.text = cost.ToString();
        dogImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
    }

    /*
     * Once the shop item is clicked on then buy the item though the shop manager
     */
    public void StartBuy()
    {
        sm.BuyItem(pawn);
        // Deselect the button
        EventSystem.current.SetSelectedGameObject(null);
    }
}
