using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SceneFader sceneFader;

    public static UIManager Instance;

    [SerializeField] GameObject deathScreen;

    [SerializeField] GameObject breakMana, fullMana;
    public enum ManaState
    {
        FullMana,
        BreakMana
    }

    public ManaState manaState;

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

        sceneFader = GetComponentInChildren<SceneFader>();
    }

    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));

        yield return new WaitForSeconds(0.8f);
        deathScreen.SetActive(true);
        Debug.Log("DeathScene");
    }
    public IEnumerator DeActivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    public void SwitchMana(ManaState _state)
    {
        switch (_state)
        {
            case ManaState.FullMana:
                breakMana.SetActive(false);
                fullMana.SetActive(true);
                break;
            case ManaState.BreakMana:
                breakMana.SetActive(true);
                fullMana.SetActive(false);
                break;

        }
        manaState = _state;
    }
}
