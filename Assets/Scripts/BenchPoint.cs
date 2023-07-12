using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchPoint : MonoBehaviour
{
    public bool filled = false;
    private GameObject obj;

    // If an object collides with the trigger collider and stays there then this function plays
    private void OnTriggerStay(Collider other)
    {
        // If the object has the tag Pawn
        if (other.gameObject.CompareTag("Pawn"))
        {
            // If the bench spot is not already filled then fill the spot with the pawn
            if (!filled)
            {
                filled = true;
                obj = other.gameObject;
                other.gameObject.GetComponent<CheckCollision>().GetOwningPawn().inPlay = false;
                other.gameObject.GetComponent<CheckCollision>().GetOwningPawn().curBenchPoint = this;
            }
            // If the pawn is not the saved object that is filling the spot then return to last position
            else if (other.gameObject != obj)
            {
                if (other.gameObject.GetComponent<Pawn>() != null)
                {
                    if (!other.gameObject.GetComponent<Pawn>().GetSelected())
                    {
                        if (other.gameObject.GetComponent<Pawn>().GetBeginClickPos() != null)
                        {
                            other.gameObject.transform.position = other.gameObject.GetComponent<Pawn>().GetBeginClickPos();
                        }
                    }
                }
            }
        }
    }

    // Once the object leaves the collider then no longer filled
    private void OnTriggerExit(Collider other)
    {
        filled = false;
        if (obj == null)
        {
            obj.gameObject.GetComponent<CheckCollision>().GetOwningPawn().inPlay = true;
        }
        obj = null;
    }

    public bool GetFilled()
    {
        return filled;
    }
}
