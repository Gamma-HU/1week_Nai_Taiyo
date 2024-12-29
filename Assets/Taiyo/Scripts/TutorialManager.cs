using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [SerializeField] static bool isSkipTutorial;

    [SerializeField] GameObject textTutorial;
    [SerializeField] GameObject initialFrame;
    [SerializeField] GameObject initialJet;
    [SerializeField] GameObject initialTank;
    [SerializeField] GameObject imageFade;
    [SerializeField] GameObject FuelSlider;
    [SerializeField] GameObject ButtonConstruct;
    [SerializeField] GameObject ButtonSetting;

    bool isClicked;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //AudioSetting.instance.PlayBGM(1);
        AudioSetting.instance.InitialSetting();

        if (!isSkipTutorial)
        {
            textTutorial.SetActive(false);
            FuelSlider.SetActive(false);
            ButtonConstruct.SetActive(false);
            ButtonSetting.SetActive(false);
            StartTutorial();
        }
        else
        {
            textTutorial.SetActive(false);

            FuelSlider.SetActive(false);
            ButtonConstruct.SetActive(false);
            ButtonSetting.SetActive(false);


            initialFrame.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * 5, ForceMode2D.Impulse);
            initialFrame.GetComponent<Rigidbody2D>().AddTorque(10, ForceMode2D.Impulse);

            initialJet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 2).normalized * 5, ForceMode2D.Impulse);
            initialJet.GetComponent<Rigidbody2D>().AddTorque(-3, ForceMode2D.Impulse);

            initialTank.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, -1) * 5, ForceMode2D.Impulse);
            initialTank.GetComponent<Rigidbody2D>().AddTorque(3, ForceMode2D.Impulse);

            StartCoroutine(StartGameCoroutine());
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClicked = true;
        }
    }

    public void StartTutorial()
    {
        StartCoroutine(TutrialCoroutine());
        isSkipTutorial = true;
    }

    IEnumerator TutrialCoroutine()
    {
        Coroutine coroutine;
        TextMeshProUGUI textMeshProUGUI = textTutorial.GetComponent<TextMeshProUGUI>();
        yield return new WaitForSeconds(2);

        isClicked = false;
        textTutorial.SetActive(true);
        coroutine = StartCoroutine(DisplayTextOneByOne(textMeshProUGUI, "ブラックホールに吸い込まれてしまった..."));
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(DisplayTextOneByOne(textMeshProUGUI, "なにもない..."));
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(DisplayTextOneByOne(textMeshProUGUI, "どうしよう..."));
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;
        if (coroutine != null) StopCoroutine(coroutine);
        textTutorial.SetActive(false);

        initialFrame.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * 5, ForceMode2D.Impulse);
        initialFrame.GetComponent<Rigidbody2D>().AddTorque(10, ForceMode2D.Impulse);

        initialJet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 2).normalized * 5, ForceMode2D.Impulse);
        initialJet.GetComponent<Rigidbody2D>().AddTorque(-3, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1);
        initialTank.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, -1) * 5, ForceMode2D.Impulse);
        initialTank.GetComponent<Rigidbody2D>().AddTorque(3, ForceMode2D.Impulse);

        yield return new WaitForSeconds(3);

        isClicked = false;
        textTutorial.SetActive(true);
        coroutine = StartCoroutine(DisplayTextOneByOne(textMeshProUGUI, "ジェットエンジンと燃料タンク...？"));
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;
        if (coroutine != null) StopCoroutine(coroutine);


        coroutine = StartCoroutine(DisplayTextOneByOne(textMeshProUGUI, "これを使えば脱出できるかも...！！"));
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;
        if (coroutine != null) StopCoroutine(coroutine);
        textTutorial.SetActive(false);
        imageFade.SetActive(false);
        FuelSlider.SetActive(true);
        ButtonConstruct.SetActive(true);
        ButtonSetting.SetActive(true);

    }

    IEnumerator DisplayTextOneByOne(TextMeshProUGUI textMeshProUGUI, string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            if (isClicked)
            {
                textMeshProUGUI.text = text;
                isClicked = false;
                yield break;
            }
            textMeshProUGUI.text = text.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(4);
        imageFade.SetActive(false);
        FuelSlider.SetActive(true);
        ButtonConstruct.SetActive(true);
        ButtonSetting.SetActive(true);
    }

}
