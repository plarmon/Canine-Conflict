using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Time Variables")]
    [SerializeField] public float prepTime = 20.0f;
    [SerializeField] TextMeshProUGUI timerDisplay;

    [Header("Environment Variables")]
    [SerializeField] GameObject wall;

    [Header("Enemies")]
    [SerializeField] GameObject[] enemyPrefabs;

    private DeckManager dm;
    private ShopManager sm;
    private XPManager xpm;

    [Header("Round Variables")]
    [SerializeField] TextMeshProUGUI roundDisplay;
    [SerializeField] TextMeshProUGUI roundWonDisplay;
    [SerializeField] TextMeshProUGUI roundLostDisplay;
    [SerializeField] private float alphaInc;
    private int round;

    public float timer;
    public bool prepPhase;
    public bool fightPhase;

    [Header("Life Variables")]
    [SerializeField] private Image[] heartImages;
    private int life;

    // Start is called before the first frame update
    void Start()
    {
        life = 3;

        dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        sm = GameObject.Find("Shop").GetComponent<ShopManager>();
        xpm = GameObject.Find("LevelMeter").GetComponent<XPManager>();

        roundWonDisplay.alpha = 0;
        roundLostDisplay.alpha = 0;

        timer = prepTime;
        prepPhase = true;
        fightPhase = false;

        round = 1;

        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        // If in the prep phase then increment the timer
        if (prepPhase)
        {
            // PrepPhase
            timer -= Time.deltaTime;
            timerDisplay.text = (Mathf.Round(timer)).ToString();

            roundDisplay.text = round.ToString();

            // If the timer is lower than 0 then move into the fight phase
            if(timer <= 0)
            {
                fightPhase = true;
                prepPhase = false;
                wall.SetActive(false);

                dm.SetReturnPoints();

            }
        } else if(fightPhase)
        {
            // Fight Phase
        }
    }

    // Returns if the player is in the prep phase
    public bool inPrepPhase()
    {
        return prepPhase;
    }

    public void ResetRound()
    {
        timer = prepTime;
        prepPhase = true;
        fightPhase = false;
        sm.RefreshShop();
        sm.bones += 5;
        xpm.AddToXP(2);
        wall.SetActive(true);
        round++;

        // Spawn new enemies
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for(int i = 0; i < round; i++)
        {
            int randomIndex;
            float randomXPos, randomZPos;

            // Spawn an enemy in a random position
            randomIndex = Random.Range(0, enemyPrefabs.Length);
            randomXPos = Mathf.Floor(Random.Range(-22.5f, 22.5f) / 5) * 5 + 2.5f;
            randomZPos = Mathf.Floor(Random.Range(2.5f, 12.5f) / 5) * 5 + 2.5f;

            GameObject enemy = Instantiate(enemyPrefabs[randomIndex], new Vector3(randomXPos, 2, randomZPos), Quaternion.Euler(Vector3.forward));
            enemy.transform.Rotate(new Vector3(0, 180, 0), Space.Self);

            if(Random.value > 0.5f && (i + 1) < round)
            {
                enemy.GetComponent<Pawn>().RankUp();
                i++;
                if(Random.value > 0.5f && (i + 1) < round)
                {
                    enemy.GetComponent<Pawn>().RankUp();
                }
            }
        }
    }

    public IEnumerator DisplayRoundWon()
    {
        float alphaValue = 1;
        while (alphaValue > 0)
        {
            roundWonDisplay.alpha = alphaValue;
            alphaValue -= alphaInc * Time.deltaTime;
            yield return null;
        }
        roundWonDisplay.alpha = 0;
    }

    public IEnumerator DisplayRoundLost()
    {
        life--;
        heartImages[life].color = Color.black;

        float alphaValue = 1;
        while (alphaValue > 0)
        {
            roundLostDisplay.alpha = alphaValue;
            alphaValue -= alphaInc * Time.deltaTime;
            yield return null;
        }
        roundLostDisplay.alpha = 0;

        if(life <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
