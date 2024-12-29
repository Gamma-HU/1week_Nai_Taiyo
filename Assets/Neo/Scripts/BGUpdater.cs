using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGUpdater : MonoBehaviour
{
    public RectTransform background;   // �w�i��Image (RectTransform)
    public Transform player;           // ���@��Transform
    public Camera mainCamera;          // ���C���J����

    public Vector2 backgroundSize;     // �w�i�摜�̕��ƍ���
    public float margin = 0.1f;        // �w�i�̃G�b�W�𒴂���ۂ̗]�T����

    private Vector2 cameraHalfSize;    // �J�����̎��씼���̃T�C�Y

    void Start()
    {
        // �J�����̔����̃T�C�Y���v�Z�i��ʂ̏㉺���E�ɕK�v�j
        cameraHalfSize = new Vector2(
            mainCamera.orthographicSize * mainCamera.aspect,
            mainCamera.orthographicSize
        );
        backgroundSize = background.sizeDelta;
    }

    void LateUpdate()
    {
        Vector2 cameraPosition = player.position;

        // �J�����[�͈̔�
        Vector2 cameraMin = cameraPosition - cameraHalfSize;
        Vector2 cameraMax = cameraPosition + cameraHalfSize;

        // �w�i�[�͈̔�
        Vector2 backgroundMin = background.anchoredPosition - (backgroundSize / 2);
        Vector2 backgroundMax = background.anchoredPosition + (backgroundSize / 2);

        // �w�i���J�����͈̔͂ɂ��킹�Ĉړ�
        if (cameraMin.x < backgroundMin.x || cameraMax.x > backgroundMax.x)
        {
            background.anchoredPosition = new Vector2(player.position.x, background.anchoredPosition.y);
        }
        if (cameraMin.y < backgroundMin.y || cameraMax.y > backgroundMax.y)
        {
            background.anchoredPosition = new Vector2(background.anchoredPosition.x, player.position.y);
        }
    }
}
