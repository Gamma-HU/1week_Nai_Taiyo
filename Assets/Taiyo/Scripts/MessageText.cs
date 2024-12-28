using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class MessageText : MonoBehaviour
{
    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(200, 2).SetRelative())
                .Join(text.DOFade(0, 2))
                .OnComplete(() => Destroy(gameObject));
    }

}
