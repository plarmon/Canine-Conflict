using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Texture2D[] imageArray;
    [SerializeField] private GameObject slideshowObj;
    [SerializeField] private Image slide;
    [SerializeField] private AudioSource backgroundMusic;

    private int currentImage = 0;

    private void Start()
    {
        currentImage = 0;

        backgroundMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            EndCutscene();
        }

        if(Input.GetMouseButtonDown(0))
        {
            currentImage++;
            if (!(currentImage >= imageArray.Length))
            {
                slide.sprite = Sprite.Create(imageArray[currentImage], new Rect(0, 0, imageArray[currentImage].width, imageArray[currentImage].height), new Vector2(0.5f, 0.5f));
            } else
            {
                EndCutscene();
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void EndCutscene()
    {
        slideshowObj.SetActive(false);
    }
}
