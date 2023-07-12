using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pawn : MonoBehaviour
{
    [Header("Pawn Variables")]
    [SerializeField] public GameObject structure;
    [SerializeField] private string name;
    [SerializeField] private int cost;
    [SerializeField] private Texture2D dogPic;
    [SerializeField] private bool enemy;

    [Header("Target Variables")]
    [SerializeField] float minDistanceToTarget = 4.0f;
    private GameObject target;

    [Header("Attacking Variables")]
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] private float attackDamage = 1.0f;
    [SerializeField] private AudioSource hitSound;
    private bool alreadyAttacking;
    private float attackTime;

    public NavMeshAgent navMeshAgent;
    public Vector3 returnPointPosition;
    public Quaternion returnPointRotation;

    [Header("Health Variables")]
    [SerializeField] public float maxHealth = 5.0f;
    private float health;

    public int rank;

    private PawnManager pm;
    private DeckManager dm;
    private GameManager gm;
    private Renderer renderer;

    private Vector3 mOffset;
    private float mZCoord;
    private bool selected;
    public bool dead;
    public bool inPlay;

    public BenchPoint curBenchPoint;

    private Vector3 beginClickPos;

    public enum dogState
    {
        PREP,
        SERACHING,
        ATTACKING
    };

    public dogState state;

    // Start is called before the first frame update
    private void Start()
    {
        // Initializing vairables
        renderer = structure.GetComponent<Renderer>();

        navMeshAgent = GetComponent<NavMeshAgent>();

        pm = GameObject.Find("PawnManager").GetComponent<PawnManager>();
        dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        hitSound = GetComponent<AudioSource>();

        minDistanceToTarget = 6f;

        state = dogState.PREP;

        dead = false;

        health = maxHealth;

        attackTime = timeBetweenAttacks;

        // Set the position of the gmaObject to the position of the child structure
        transform.position = structure.transform.position;
    }

    // Calls every frame
    private void Update()
    {
        // Keeps the position of the object at a certain height
        transform.position = new Vector3(transform.position.x, 2f, transform.position.z);

        if (health <= 0)
        {
           if (!gameObject.CompareTag("Enemy"))
           {
               dead = true;
               transform.position = new Vector3(transform.position.x, -10, transform.position.z);
               structure.gameObject.transform.position = transform.position;
           } else
           {
               Destroy(gameObject);
           }
        }

        if(gm.inPrepPhase())
        {
           navMeshAgent.radius = 0;
        } else
        {
           navMeshAgent.radius = 0.75f;
        }

        if (!dead)
        {
            // If no longer in the prep phase
            if (!gm.inPrepPhase())
            {
                // If the pawn is still selected then drop it in the current position
                if (selected)
                {
                    DropPawn();
                }
                if (transform.position.z > -17.0f)
                {
                    // Switches state off of prep if still on it
                    if (state == dogState.PREP)
                    {
                        state = dogState.SERACHING;
                    }

                    if (state == dogState.SERACHING)
                    {
                        // If no target has been identified then find a target
                        if (target == null)
                        {
                            FindTarget();
                        }

                        if (target != null && target.GetComponent<Pawn>() != null)
                        {
                            if (target.GetComponent<Pawn>().dead)
                            {
                                FindTarget();
                            }
                        }

                        if (state == dogState.SERACHING)
                        {
                            if (target != null || target.transform.position.z < -17)
                            {
                                if (Vector3.Magnitude(target.transform.position - transform.position) < minDistanceToTarget)
                                {
                                    navMeshAgent.isStopped = true;
                                    // Switch to Attack phase
                                    state = dogState.ATTACKING;
                                    attackTime = timeBetweenAttacks;
                                }
                                else
                                {
                                    navMeshAgent.SetDestination(target.transform.position);
                                }
                            } else
                            {
                                FindTarget();
                            }
                        }
                    }

                    if (state == dogState.ATTACKING)
                    {
                       // Increment attackTime
                       attackTime -= Time.deltaTime;

                        if (target != null)
                        {
                            // Attack target
                            if (attackTime <= 0)
                            {
                                hitSound.Play();
                                float remainingHealth = target.GetComponent<Pawn>().TakeDamage(attackDamage);
                                attackTime = timeBetweenAttacks;
                                if (remainingHealth <= 0)
                                {
                                    state = dogState.SERACHING;
                                    FindTarget();
                                }
                            }
                        
                            // If to far from the target go back to searching state
                            if (Vector3.Magnitude(target.transform.position - transform.position) > minDistanceToTarget + 1)
                            {
                                state = dogState.SERACHING;
                            }
                        
                            transform.forward = Vector3.Lerp(transform.forward, Vector3.Normalize(target.transform.position - transform.position), 0.75f);
                        } else
                        {
                            FindTarget();
                        }
                   }
                }
            }
            else
            {
                if (gameObject.CompareTag("Pawn"))
                {
                    transform.forward = Vector3.forward;
                }
            }
        }
    }

    public float TakeDamage(float damage)
    {
        health -= damage;
        return health;
    }

    public void ResetHealth()
    {
        health = maxHealth;
        dead = false;
    }

    public void SetReturnPoint()
    {
        returnPointPosition = transform.position;
        returnPointRotation = transform.rotation;
    }

    public void RankUp()
    {
       rank++;
       structure.transform.localScale = structure.transform.localScale * 1.3f;
       gameObject.GetComponent<BoxCollider>().size *= 1.3f;
       maxHealth *= 3;
       health = maxHealth;
       attackDamage *= 3;
       cost *= 3;
    }

    #region Mouse Functions

    /*
     * When you click down on the object then this function runs 
     */
    private void OnMouseDown()
    {
        // If still in the prep phase then continue
        if (gm.inPrepPhase() && !enemy)
        {
            // Set the layer to outlines to change the shader
            // structure.layer = LayerMask.NameToLayer("Outlined");

            // Gets the world z coordinate from the mouse position
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();

            selected = true;
            pm.SetLastSelectedPawn(this);
            beginClickPos = transform.position;

            // If the pawn is in the play area then reduce the map total
            if (transform.position.z > -17.5)
            {
                dm.ModifyMapTotal(-1);
            }
        }
    }

    /*
     * When when the click up the mouse on an object then this function runs
     */
    private void OnMouseUp()
    {
        // If still in prep phase
        if (gm.inPrepPhase() && !enemy)
        {
            // Drops the Pawn
            DropPawn();
        }
    }

    /*
     * While the Mouse is clicked and being dragged then this function runs
     */
    private void OnMouseDrag()
    {
        // If still in prep phase
        if (gm.inPrepPhase() && !enemy)
        {
            // Gets the position of the mouse in world cooridnates
            Vector3 currentPos = GetMouseWorldPos() + mOffset;
            currentPos = new Vector3(Mathf.Clamp(currentPos.x, -22.5f, 22.5f), currentPos.y, Mathf.Clamp(currentPos.z, -17.5f, -2.5f));

            // Sets the position of the pawn to the mouse world coordinates
            transform.position = currentPos;
        }
    }

    /*
     * Once the mouse enters the object this function plays
     */
    private void OnMouseEnter()
    {
        // structure.layer = LayerMask.NameToLayer("Outlined");
    }

    /*
     * Once the mouse exits the object this function plays
     */
    private void OnMouseExit()
    {
        if (!selected)
        {
            structure.layer = LayerMask.NameToLayer("Default");
        }
    }

    /*
    * Gets the world position of the mouse
    */
    private Vector3 GetMouseWorldPos()
    {
        // pixel coordinates (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate fo the gameObject on screen
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    #endregion

    #region FindTarget Function
    /*
     * Finds the closest target to the pawn
     */
    private void FindTarget()
    {
        navMeshAgent.isStopped = false;
        // Gets a list of all the targets
        GameObject[] targets;
        if (!enemy)
        {
            targets = GameObject.FindGameObjectsWithTag("Enemy");
        }
        else
        {
            targets = GameObject.FindGameObjectsWithTag("Pawn");
        }

        List<Pawn> pawnTargets = new List<Pawn>();
        foreach (GameObject go in targets)
        {
            Pawn p = go.GetComponent<Pawn>();
            if (p != null)
            {
                if (!p.dead && p.transform.position.z > -17)
                {
                    pawnTargets.Add(p);
                }
            }
        }

        if (pawnTargets.Count == 0)
        {
            // Set everyone to prep stage
           dm.RoundEnd();
           if(gameObject.CompareTag("Enemy"))
           {
               gm.StartCoroutine(gm.DisplayRoundLost());
           } else
           {
               gm.StartCoroutine(gm.DisplayRoundWon());
           }
           return;
        }

        // Initializes the minDistance and closestEnemy objects
        float minDistance = 100;
        GameObject closestEnemy = null;

        // Loops through each enemy in the targets list
        // foreach(GameObject enemy in targets)
        foreach (Pawn enemy in pawnTargets)
        {
            // Checks how far away the current enemy is from the pawn
            float distanceToEnemy = Vector3.Magnitude(enemy.transform.position - transform.position);

            // If it's less than minDistance then set the closest enemy to the enemy and change the minDistance value
            if (distanceToEnemy < minDistance)
            {
                // closestEnemy = enemy;
                closestEnemy = enemy.gameObject;
                minDistance = distanceToEnemy;
            }
        }
        // Set the target to the closest enemy
        target = closestEnemy;
    }
    #endregion

    #region DropPawn Function
       /*
        * Drops the pawn in its current position
        */
       private void DropPawn()
       {
           // Sets the position of the pawn
           transform.position = structure.transform.position;

           structure.transform.localPosition = Vector3.zero;

           // Makes the pawn not outlined
           structure.layer = LayerMask.NameToLayer("Default");

           // Modiifies the map total value
           if (transform.position.z > -17.5)
           {
               if (!dm.IsMapFull())
               {
                   dm.ModifyMapTotal(1);
               }
               else
               {
                   transform.position = GetBeginClickPos();
                   structure.transform.localPosition = Vector3.zero;
                   dm.StartCoroutine(dm.DisplayMapCount());
               }
           }

           // Deselects the pawn
           selected = false;
       }
       #endregion

    #region Getters
    public bool GetSelected()
    {
        return selected;
    }

    public Vector3 GetBeginClickPos()
    {
        return beginClickPos;
    }

    public PawnManager GetPawnManager()
    {
        return pm;
    }

    public string GetName()
    {
        return name;
    }

    public int GetCost()
    {
        return cost;
    }

    public Texture2D GetImage()
    {
        return dogPic;
    }

    public float GetHealth()
    {
        return health;
    }
    #endregion
}
