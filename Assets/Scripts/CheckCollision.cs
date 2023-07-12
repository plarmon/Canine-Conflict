using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    [SerializeField] private Pawn owningPawn;
    private DeckManager dm;
    private GameManager gm;

    private void Start()
    {
        dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // If an object collides with the trigger collider and stays there then this function plays
    private void OnCollisionStay(Collision collision)
    {
        // If the collider ha sthe tag pawn and is not selected and was not just selected then continue
        if (collision.collider.gameObject.CompareTag("Pawn") && !owningPawn.GetSelected() && owningPawn.GetPawnManager().GetLastSelectedPawn() == owningPawn && gm.inPrepPhase())
        {
            // Gets the original position of the gameObject
            Vector3 originalPosition = owningPawn.gameObject.transform.position;

            // Sets the position of the owningPawn to the owning pawn's begin click position
            owningPawn.gameObject.transform.position = owningPawn.GetBeginClickPos();

            // If the pawn is in the deck then remove 1 from the map total
            if(owningPawn.gameObject.transform.position.z < -17f)
            {
                dm.ModifyMapTotal(-1);
                if(collision.collider.gameObject.transform.position.z < -17f)
                {
                    dm.ModifyMapTotal(1);
                }
            } else
            {
                // if the map is not full
                if (!dm.IsMapFull())
                {
                    // Add 1 to the map total
                    dm.ModifyMapTotal(1);
                    
                    // If the begin click position was on the map and the original position was in the map then remove 1 from the map total
                    if (owningPawn.GetBeginClickPos().z > -17 && originalPosition.z > -17)
                    {
                        dm.ModifyMapTotal(-1);
                    }
                }
            }
        }
    }

    // Returns the owning pawn of this script
    public Pawn GetOwningPawn()
    {
        return owningPawn;
    }
}
