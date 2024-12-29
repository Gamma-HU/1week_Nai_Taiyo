using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEtester : MonoBehaviour
{
    //enginesound��volume�͕ϐ��ɐݒ肵�ăG���W���̐��ɉ����ĉ��ʂ��グ��Ɨǂ������H

    [SerializeField]
    AudioSource Attachsound;
    [SerializeField]
    AudioSource Enginesound;
    public float LoopEndTime= 8.0f;
    public float LoopLengthTime= 2.0f;
    public float EnginesoundTime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Enginesound.time > 0)
            {
                StopCoroutine("VolumeDown");
                EnginesoundTime = Enginesound.time - 0.3f;
                Enginesound.Stop();
                Enginesound.volume = 0.8f;
            }
            else
            {
                EnginesoundTime = 0;
            }
            EnginesoundLoader();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine("VolumeDown");
        }
        if (Enginesound.time >= LoopEndTime)
        {
            Enginesound.time -= LoopLengthTime;
        }
    }
    public void AttachsoundLoader()
    {
        Attachsound.Play();//�s�b�`�̓G���W���̑��������(1.9)�A���i�̏d���̒l�Ɉˑ����ăs�b�`��ύX
    }
    
    public void EnginesoundLoader()
    {
        Enginesound.time = EnginesoundTime;       
        Enginesound.Play();//�s�b�`��0.2�A���ʂ�0.8
    }

    IEnumerator VolumeDown()
    {
        while (Enginesound.volume > 0)
        {
            Enginesound.volume -= 0.04f;
            yield return new WaitForSeconds(0.025f);
            
        }
        Enginesound.Stop();
        Enginesound.volume = 0.8f;
    }
}
