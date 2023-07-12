using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    public Pawn pawn;
    public GameObject target;
    public GameObject structure;
    Vector3 truePos;
    public float gridSize;
    public float offset;
    public float yPos = 1.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        // If the pawn is selected then move it to the grid position it's closest to
        if (pawn.GetSelected())
        {
            truePos.x = Mathf.Floor(target.transform.position.x / gridSize) * gridSize + offset;
            truePos.y = yPos;
            truePos.z = Mathf.Floor(target.transform.position.z / gridSize) * gridSize + offset;

            structure.transform.position = truePos;
        }
    }
}
