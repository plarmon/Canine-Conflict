using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    private ShopManager sm;
    private DeckManager dm;

    private void Start()
    {
        sm = GameObject.Find("Shop").GetComponent<ShopManager>();
        dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
    }

    // If an object collides with the trigger collider and stays there then this function plays
    private void OnTriggerStay(Collider other)
    {
        // If the object has the tag Pawn then continue
        if (other.gameObject.CompareTag("Pawn"))
        {
            // Gets the owning pawn from the check collision script
            Pawn pawn = other.gameObject.GetComponent<CheckCollision>().GetOwningPawn();

            // If the pawn is not selected then trash the pawn
            if (!pawn.GetSelected())
            {
                sm.bones += pawn.GetCost();
                dm.RemovePawn(pawn);
                Destroy(pawn.gameObject);
            }
        }
    }
}
