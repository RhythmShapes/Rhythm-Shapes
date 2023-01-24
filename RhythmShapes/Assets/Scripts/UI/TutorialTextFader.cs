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

    [SerializeField] private float fadeTime;
    

    void Start()
    {
        text0 = regionName0.GetComponent<TMP_Text>();
        text1 = regionName1.GetComponent<TMP_Text>();
        text2 = regionName2.GetComponent<TMP_Text>();
        text3 = regionName3.GetComponent<TMP_Text>();
        text4 = regionName4.GetComponent<TMP_Text>();
        text5 = regionName5.GetComponent<TMP_Text>();

        text0.CrossFadeAlpha(0, 18f, false);
        text1.CrossFadeAlpha(0, 0.0f, false);
        text2.CrossFadeAlpha(0, 0.0f, false);
        text3.CrossFadeAlpha(0, 0.0f, false);
        text4.CrossFadeAlpha(0, 0.0f, false);
        text5.CrossFadeAlpha(0, 0.0f, false);

        StartCoroutine(FadeIn(text1, timeIn1, timeOut1));
        StartCoroutine(FadeIn(text2, timeIn2, timeOut2));
        StartCoroutine(FadeIn(text3, timeIn3, timeOut3));
        StartCoroutine(FadeIn(text4, timeIn4, timeOut4));
        StartCoroutine(FadeIn(text5, timeIn5, timeOut5));

        StartCoroutine(EndTutorial(timingEnd));
    }

    private IEnumerator FadeIn(TMP_Text textObject, float waitTime, float disappearTime)
    {
        yield return new WaitForSeconds(waitTime);
        textObject.CrossFadeAlpha(1, fadeTime, false);
        yield return new WaitForSeconds(disappearTime);
        textObject.CrossFadeAlpha(0, fadeTime, false);
    }

    private IEnumerator EndTutorial(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        tutorialTextCanvas.SetActive(false);
    }
}
