using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class spike : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerController.Instance.pState.cutScene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1);
        PlayerController.Instance.transform.position = GameManager.Instance.platformingReSpawnPoint;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
        PlayerController.Instance.pState.cutScene = false;
        PlayerController.Instance.pState.invincible = false;
        Time.timeScale = 1;

    }
}
