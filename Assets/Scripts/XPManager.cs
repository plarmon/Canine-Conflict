using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class XPManager : MonoBehaviour
{
    [Header("UI Variables")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI XPCounterText;
    [SerializeField] private TextMeshProUGUI levelText;

    private ShopManager sm;
    private DeckManager dm;
    private int maxXPCount;
    private int xpCount;
    private int curLevel;

    private void Start()
    {
        sm = GameObject.Find("Shop").GetComponent<ShopManager>();
        dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();

        xpCount = 0;
        maxXPCount = 2;
        curLevel = 0;

        XPCounterText.text = xpCount.ToString() + " / " + maxXPCount.ToString();
    }

    /*
     * Once a button is clicked in the UI then buy xp to level up the player
     */
    public void BuyXP()
    {
        // Checks to see if the player has enough bones
        if(sm.bones - 2 >= 0)
        {
            sm.bones -= 2;
            AddToXP(2);
        } else
        {
            sm.StartCoroutine(sm.NoBones());
        }
        // Deselects the button
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void AddToXP(int amount)
    {
        xpCount += amount;

        // Checks if the player has gained enough xp to level up
        if (xpCount >= maxXPCount)
        {
            // Level up 
            curLevel++;
            levelText.text = curLevel.ToString();

            xpCount = 0;
            maxXPCount *= 2;

            dm.maxPawnsInPlay++;
            dm.ModifyMapTotal(0);
        }

        // Updates the xp ui elements
        Vector3 curScale = progressBar.gameObject.GetComponent<RectTransform>().localScale;
        progressBar.gameObject.GetComponent<RectTransform>().localScale = new Vector3((float)xpCount / (float)maxXPCount, curScale.y, curScale.z);
        XPCounterText.text = xpCount.ToString() + " / " + maxXPCount.ToString();
    }
}
