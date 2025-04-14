using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    
    private void Start()
    {
        

        if (GameManager.Instance.transitionedFromScene == transitionTo)
        {
            PlayerController.Instance.transform.position = startPoint.position; //
            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            PlayerController.Instance.pState.cutScene = true;
            //PlayerController.instance.pState.invincible = true;
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
            //SceneManager.LoadScene(transitionTo);
        }
    }

    void CheckShadeData()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (enemyObjects[i].GetComponent<Shade>() != null)
            {
                SaveData.Instance.SaveShadeData();
            }
        }
    }
}