using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnManager : MonoBehaviour
{
    private GameObject[] pawnObjects;
    private List<Pawn> pawns = new List<Pawn>();

    private GameManager gm;

    private Pawn lastSelectedPawn;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        pawnObjects = GameObject.FindGameObjectsWithTag("Pawn");
        foreach(GameObject obj in pawnObjects)
        {
            pawns.Add(obj.GetComponent<Pawn>());
        }
    }

    public void SetLastSelectedPawn(Pawn p)
    {
        lastSelectedPawn = p;
    }

    public Pawn GetLastSelectedPawn()
    {
        return lastSelectedPawn;
    }

    //public void RoundEnd()
    //{
    //    Debug.Log("Hit");
    //    gm.timer = gm.prepTime;
    //    gm.prepPhase = true;
    //    gm.fightPhase = false;
    //    foreach(Pawn p in pawns)
    //    {
    //        p.state = Pawn.dogState.PREP;
    //    }
    //    Debug.Log("Hit 2");
    //}
}
