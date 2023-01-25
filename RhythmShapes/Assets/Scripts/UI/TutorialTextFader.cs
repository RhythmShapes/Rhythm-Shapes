using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTextFader : MonoBehaviour
{
    [SerializeField] private GameObject regionName1;
    [SerializeField] private GameObject regionName2;
    [SerializeField] private GameObject regionName3;
    [SerializeField] private GameObject regionName4;
    [SerializeField] private GameObject regionName5;
    [SerializeField] private GameObject tutorialTextCanvas;
    [SerializeField] private float timing1;
    [SerializeField] private float timing2;
    [SerializeField] private float timing3;
    [SerializeField] private float timing4;
    [SerializeField] private float timing5;
    [SerializeField] private float timingEnd;

    private TMP_Text text1;
    private TMP_Text text2;
    private TMP_Text text3;
    private TMP_Text text4;
    private TMP_Text text5;

    [SerializeField] private float fadeTime;
    

    void Start()
    {
        text1 = regionName1.GetComponent<TMP_Text>();
        text2 = regionName2.GetComponent<TMP_Text>();
        text3 = regionName3.GetComponent<TMP_Text>();
        text4 = regionName4.GetComponent<TMP_Text>();
        text5 = regionName5.GetComponent<TMP_Text>();

        text1.CrossFadeAlpha(0, 0.0f, false);
        text2.CrossFadeAlpha(0, 0.0f, false);
        text3.CrossFadeAlpha(0, 0.0f, false);
        text4.CrossFadeAlpha(0, 0.0f, false);
        text5.CrossFadeAlpha(0, 0.0f, false);

        StartCoroutine(FadeIn(text1, timing1));
        StartCoroutine(FadeIn(text2, timing2));
        StartCoroutine(FadeIn(text3, timing3));
        StartCoroutine(FadeIn(text4, timing4));
        StartCoroutine(FadeIn(text5, timing5));

        StartCoroutine(EndTutorial(timingEnd));
    }

    private IEnumerator FadeIn(TMP_Text textObject, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        textObject.CrossFadeAlpha(1, fadeTime, false);
    }

    private IEnumerator EndTutorial(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        tutorialTextCanvas.SetActive(false);
    }
}
