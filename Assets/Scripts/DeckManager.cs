using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckManager : MonoBehaviour
{
    [SerializeField] public int maxPawnsInPlay;
    [SerializeField] private TextMeshProUGUI mapCountText;
    [SerializeField] private float alphaInc;

    private GameManager gm;
    private List<Pawn> playerPawns;
    private int total;
    private bool mapFull;

    private int mapTotal;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        playerPawns = new List<Pawn>();
        total = 0;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Sets the map count alpha to zero
        mapCountText.alpha = 0;
    }

    /*
     * Adds a pawn to the playerPawns list
     * 
     * @param p - The pawn to add to the list
     */
    public void AddPawn(Pawn p)
    {
        playerPawns.Add(p);
        CheckMerge(p);
        total++;
    }

    private void CheckMerge(Pawn p)
    {
        string nameCheck = p.name;
        int rankCheck = p.rank;
        int count = 0;
        List<Pawn> samePawns = new List<Pawn>();

        foreach (Pawn pawn in playerPawns)
        {
            if (pawn.name == nameCheck && pawn.rank == rankCheck)
            {
                count++;
                samePawns.Add(pawn);
            }
        }
        if (count == 3)
        {
            total -= 3;
            foreach (Pawn pawn in samePawns)
            {
                if (pawn == p)
                {
                    p.RankUp();
                    if (p.rank != 3)
                    {
                        CheckMerge(p);
                    }
                }
                else
                {
                    if (pawn.gameObject.transform.position.z > -17)
                    {
                        ModifyMapTotal(-1);
                    }
                    RemovePawn(pawn);
                    BenchPoint point = pawn.curBenchPoint;
                    Destroy(pawn.gameObject);
                    point.filled = false;
                }
            }
        }
    }

    /*
     * Removes a pawn from the playerPawns list
     * 
     * @param p - The pawn to be removed
     */
    public void RemovePawn(Pawn p)
    {
        playerPawns.Remove(p);
        total--;
    }

    public void RoundEnd()
    {
        foreach (Pawn p in playerPawns)
        {
            if (p.transform.position.z > -17)
            {
                p.state = Pawn.dogState.PREP;
                p.navMeshAgent.ResetPath();
                p.navMeshAgent.isStopped = true;
                p.navMeshAgent.avoidancePriority = 0;
                p.ResetHealth();
                p.gameObject.transform.position = p.returnPointPosition;
                p.gameObject.transform.rotation = p.returnPointRotation;
                p.structure.gameObject.transform.position = p.gameObject.transform.position;
                p.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 offset = new Vector3(0, 2, 0);
        foreach (GameObject obj in enemies)
        {
            Destroy(obj);
        }

        gm.ResetRound();
    }

    public void SetReturnPoints()
    {
        foreach(Pawn p in playerPawns)
        {
            p.navMeshAgent.avoidancePriority = 50;
            p.navMeshAgent.isStopped = false;
            p.SetReturnPoint();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemies)
        {
            Pawn p = obj.GetComponent<Pawn>();
            p.navMeshAgent.avoidancePriority = 50;
            p.navMeshAgent.isStopped = false;
            p.SetReturnPoint();
        }
    }

    /*
     * Modifies the number of pawns that are on the map
     * 
     * @param modifier - How much to modify the mpa total by
     */
    public void ModifyMapTotal(int modifier)
    {
        // if (gm.inPrepPhase())
        // {
            // modifies the mapTotal variable
            mapTotal += modifier;

            // Determines if the map is full
            mapFull = mapTotal >= maxPawnsInPlay;

            // Modifies the mapCountText text value
            mapCountText.text = mapTotal.ToString() + " / " + maxPawnsInPlay.ToString();

            // Makes the new text fade pop up and fade out
            StopAllCoroutines();
            StartCoroutine(DisplayMapCount());
        // }
    }

    /*
     * 
     */
    public bool IsMapFull()
    {
        return mapFull;
    }

    /*
     * Coroutine which fades out the map count text
     */
    public IEnumerator DisplayMapCount()
    {
        float alphaValue = 1;
        while (alphaValue > 0)
        {
            mapCountText.alpha = alphaValue;
            alphaValue -= alphaInc * Time.deltaTime;
            yield return null;
        }
        mapCountText.alpha = 0;
    }
}
