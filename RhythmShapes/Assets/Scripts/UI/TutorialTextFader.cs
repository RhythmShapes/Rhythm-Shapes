using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextFader : MonoBehaviour
{
    [SerializeField] private GameObject regionName0;
    [SerializeField] private GameObject regionName1;
    [SerializeField] private GameObject regionName2;
    [SerializeField] private GameObject regionName3;
    [SerializeField] private GameObject regionName4;
    [SerializeField] private GameObject regionName5;
    [SerializeField] private GameObject tutorialTextCanvas;

    [SerializeField] private float timeIn0;
    [SerializeField] private float timeOut0;
    [SerializeField] private float timeIn1;
    [SerializeField] private float timeOut1;
    [SerializeField] private float timeIn2;
    [SerializeField] private float timeOut2;
    [SerializeField] private float timeIn3;
    [SerializeField] private float timeOut3;
    [SerializeField] private float timeIn4;
    [SerializeField] private float timeOut4;
    [SerializeField] private float timeIn5;
    [SerializeField] private float timeOut5;

    [SerializeField] private float timingEnd;

    private TMP_Text text0;
    private TMP_Text text1;
    private TMP_Text text2;
    private TMP_Text text3;
    private TMP_Text text4;
    private TMP_Text text5;

    private AudioSource _audioSource;

    [SerializeField] private float fadeTime;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        text0 = regionName0.GetComponent<TMP_Text>();
        text1 = regionName1.GetComponent<TMP_Text>();
        text2 = regionName2.GetComponent<TMP_Text>();
        text3 = regionName3.GetComponent<TMP_Text>();
        text4 = regionName4.GetComponent<TMP_Text>();
        text5 = regionName5.GetComponent<TMP_Text>();

        ShowTexts();
    }

    void ShowTexts() 
    {
        tutorialTextCanvas.SetActive(true);
        StartCoroutine(FadeIn(regionName0, text0, timeIn0, timeOut0));
        StartCoroutine(FadeIn(regionName1, text1, timeIn1, timeOut1));
        StartCoroutine(FadeIn(regionName2, text2, timeIn2, timeOut2));
        StartCoroutine(FadeIn(regionName3, text3, timeIn3, timeOut3));
        StartCoroutine(FadeIn(regionName4, text4, timeIn4, timeOut4));
        StartCoroutine(FadeIn(regionName5, text5, timeIn5, timeOut5));

        StartCoroutine(EndTutorial(timingEnd));
    }

    private IEnumerator FadeIn(GameObject regionObject, TMP_Text textObject, float waitTime, float disappearTime)
    {
        yield return new WaitUntil(() => _audioSource.time >= waitTime);
        textObject.CrossFadeAlpha(0, 0f, false);
        regionObject.SetActive(true);
        textObject.CrossFadeAlpha(1, fadeTime, false);
        yield return new WaitUntil(() => _audioSource.time >= disappearTime);
        textObject.CrossFadeAlpha(0, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);
        regionObject.SetActive(false);
    }

    private IEnumerator EndTutorial(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        tutorialTextCanvas.SetActive(false);
    }

    public void OnGameRestarted()
    {
        regionName0.SetActive(false);
        regionName1.SetActive(false);
        regionName2.SetActive(false);
        regionName3.SetActive(false);
        regionName4.SetActive(false);
        regionName5.SetActive(false);
        ShowTexts();
    }
}
