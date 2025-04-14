using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public string transitionedFromScene;
    public Vector2 platformingReSpawnPoint;

    public Vector2 respawnPoint;
    [SerializeField] Bench bench;

    public GameObject shade;
    public static GameManager Instance { get; private set; }

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;

    public bool gameIsPaused;
    private void Update()
    {
       if(Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;

        }
    }
   public void UnPauseGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        
        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    //private void Start()
    //{
    //    SaveData.Instance.LoadPlayerData();
    //}



    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        if(SaveData.Instance.benchSceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }
        if(SaveData.Instance.benchPos != null)
        {
            respawnPoint = SaveData.Instance.benchPos;
        }
        else
        {
            respawnPoint = platformingReSpawnPoint;
        }

        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeActivateDeathScreen());
        PlayerController.Instance.Respawned();
    }

    //public void SaveScene()
    //{
    //    string currentSceneName = SceneManager.GetActiveScene().name;
    //    SaveData.Instance.sceneNames.Add(currentSceneName);
    //}

}
