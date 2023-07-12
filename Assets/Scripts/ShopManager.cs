using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] pawnPrefabObjs;
    private List<Pawn> pawns;
    [SerializeField] private GameObject shopItemPrefab;
    private GameObject[] shopSlots;

    [Header("Text UI Elements")]
    [SerializeField] private TextMeshProUGUI boneCountText;
    [SerializeField] private TextMeshProUGUI noBonesText;
    [SerializeField] private TextMeshProUGUI benchFullText;
    [SerializeField] private float alphaInc = 5.0f;

    public int bones;
    private GameObject[] benchPointObjs;
    private List<BenchPoint> benchPoints;

    private DeckManager dm;
    private GameManager gm;

    /*
     * Struct which contains all the data for an item in the shop
     */
    public struct ShopPawn
    {
        public string name;
        public int cost;
        public Texture2D pawnPic;
        public GameObject pawnPrefab;
        public GameObject owningSlot;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets the alpha value of the bench and bones text to 0
        benchFullText.alpha = 0;
        noBonesText.alpha = 0;

        dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        benchPoints = new List<BenchPoint>();

        // Gets a list of all the game objects with the tag BenchPoint
        benchPointObjs = GameObject.FindGameObjectsWithTag("BenchPoint");

        // Creates a list of BenchPoints from the list of gameOBjects with the tag bench point
        foreach(GameObject obj in benchPointObjs)
        {
            benchPoints.Add(obj.GetComponent<BenchPoint>());
        }

        // Creates a list of Pawns from the list of gameObjects in the array pawnPrefabObjs
        pawns = new List<Pawn>();
        foreach(GameObject obj in pawnPrefabObjs)
        {
            pawns.Add(obj.GetComponent<Pawn>());
        }

        // Gets a list of all the game objects with the tag ShopSlot
        shopSlots = GameObject.FindGameObjectsWithTag("ShopSlot");

        // Fils the shop with objects
        RefreshShop();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the bones text
        boneCountText.text = bones.ToString();
    }

    /*
     * Buys a certain item from the shop
     * 
     * @param pawn - The ShopPawn to buy from the shop
     */
    public void BuyItem(ShopPawn pawn)
    {
        if (gm.inPrepPhase())
        {
            // If you have enough bones to buy the object then continue
            if (pawn.cost <= bones)
            {
                // Trys to find a bechPoint where you can place the new item
                bool foundSpot = false;
                foreach (BenchPoint bp in benchPoints)
                {
                    if (!bp.GetFilled())
                    {
                        GameObject newPawn = Instantiate(pawn.pawnPrefab, bp.gameObject.transform.position, Quaternion.Euler(Vector3.forward));
                        bones -= pawn.cost;
                        Destroy(pawn.owningSlot);
                        foundSpot = true;
                        dm.AddPawn(newPawn.GetComponent<Pawn>());
                        break;
                    }
                }
                if (!foundSpot)
                {
                    // Not Enough Room
                    StartCoroutine(BenchFull());
                }
            }
            else
            {
                // Broke
                StartCoroutine(NoBones());
            }
        }
    }

    // Once the refresh button is clicked then refresh the shop and remove 1 bone
    public void PayForRefresh()
    {
        bones -= 1;
        RefreshShop();
    }

    /*
     * Refreshes the shop with new items
     */
    public void RefreshShop()
    {
        // Goes through each shop slot and fills it with a new shop item
        foreach(GameObject obj in shopSlots)
        {
            if (obj.transform.childCount >= 1)
            {
                Destroy(obj.transform.GetChild(0).gameObject);
            }
            GameObject shopItem = Instantiate(shopItemPrefab, obj.transform);
            Pawn pawnPick = pawns[Random.Range(0, pawns.Count)];
            shopItem.GetComponent<ShopItem>().instantiate(pawnPick.GetName(), pawnPick.GetCost(), pawnPick.GetImage(), pawnPick.gameObject);
        }
    }

    /*
     * Coroutine which fades out the Bench full text
     */
    private IEnumerator BenchFull()
    {
        float alphaValue = 1;
        while(alphaValue > 0)
        {
            benchFullText.alpha = alphaValue;
            alphaValue -= alphaInc * Time.deltaTime;
            yield return null;
        }
        benchFullText.alpha = 0;
    }

    /*
     * Coroutine which fades out the no bones text
     */
    public IEnumerator NoBones()
    {
        float alphaValue = 1;
        while (alphaValue > 0)
        {
            noBonesText.alpha = alphaValue;
            alphaValue -= alphaInc * Time.deltaTime;
            yield return null;
        }
        noBonesText.alpha = 0;
    }
}
