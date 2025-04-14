using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDash : MonoBehaviour
{
    [SerializeField] GameObject canvasUI;
    bool used;
    
    void Start()
    {
        if (PlayerController.Instance.unlockDash)
        {
            Destroy(gameObject);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());

        }
    }

    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(0.5f);

        canvasUI.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        PlayerController.Instance.unlockDash = true;
        SaveData.Instance.SavePlayerData();
        canvasUI.SetActive(false);

        Destroy(gameObject);
    }
}
