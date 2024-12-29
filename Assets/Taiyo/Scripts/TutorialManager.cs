using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    bool isSkipTutorial;

    [SerializeField] GameObject TextTutorial;

    bool isClicked;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartTutorial();
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
    }

    IEnumerator TutrialCoroutine()
    {
        yield return new WaitForSeconds(2);
        TextTutorial.SetActive(true);

        TextTutorial.GetComponent<TextMeshProUGUI>().text = "ブラックホールに吸い込まれてしまった...";
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;

        TextTutorial.GetComponent<TextMeshProUGUI>().text = "なにもない...";
        yield return new WaitForSeconds(1);
        while (!isClicked)
        {
            yield return new WaitForEndOfFrame();
        }
        isClicked = false;

        TextTutorial.GetComponent<TextMeshProUGUI>().text = "どうしよう...";
        yield return new WaitForSeconds(1);

    }

}
