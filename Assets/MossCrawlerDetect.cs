using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossCrawlerDetect : MonoBehaviour
{
    public static MossCrawlerDetect Instance;


    public bool detected;

    private void Awake()
    {  
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detected = true;
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {

    //    }
    //}
}
