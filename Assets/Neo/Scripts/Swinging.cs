using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Swinging : MonoBehaviour
{
    public static Tween SwingingTween;

    [SerializeField] private float amplitude;
    [SerializeField] private float swingingTime;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DORotate(new Vector3(0,0,-amplitude), swingingTime, RotateMode.WorldAxisAdd)
                      .SetLoops(-1, LoopType.Yoyo)
                      .SetEase(Ease.InOutSine)
                      .SetAutoKill(true);
    }

    void OnDisable()
    {
        //オブジェクトが破壊されるときにTweenを停止しておかないと「Tweenするモノがないよ～」っていわれる
        if(SwingingTween != null && SwingingTween.IsActive())
        {
            SwingingTween.Kill();
        }
    }
}
