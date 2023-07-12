using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI Variables")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Pawn owningPawn;

    private float originalScale;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = owningPawn.maxHealth;
        originalScale = healthBar.gameObject.GetComponent<RectTransform>().localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the xp ui elements
        Vector3 curScale = healthBar.gameObject.GetComponent<RectTransform>().localScale;
        healthBar.gameObject.GetComponent<RectTransform>().localScale = new Vector3((owningPawn.GetHealth() / maxHealth) * originalScale, curScale.y, curScale.z);
    }
}
