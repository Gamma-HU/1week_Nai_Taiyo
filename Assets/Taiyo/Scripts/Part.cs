using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Part : MonoBehaviour
{
    public float mass;
    //public bool isFrame;
    public bool isCockpit;
    public bool isConstructMode;
    bool isPick;
    bool canConnect;
    public bool CanConnect => canConnect;

    [SerializeField] List<ConnectPoint> connectPoints = new List<ConnectPoint>();
    GameObject gohst;

    ConnectPoint _connectPoint1;
    ConnectPoint _connectPoint2;
    float _angle;

    //ConnectPoint connectedToParentPoint;


    void Awake()
    {
        if (GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = mass;
            rb.drag = 0.2f;
            rb.angularDrag = 0.1f;
            rb.gravityScale = 0;
            rb.isKinematic = false;

        }

        InitializeGohst();

        connectPoints.Clear();
        connectPoints.AddRange(GetComponentsInChildren<ConnectPoint>());
    }

    void Update()
    {

        if (isPick)
        {
            Picking();
        }


    }

    void InitializeGohst()
    {
        gohst = transform.Find("Gohst").gameObject;
        gohst.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        gohst.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);  //透明度30%

        if (GetComponent<BoxCollider2D>())
        {
            gohst.AddComponent<BoxCollider2D>();
        }
        else if (GetComponent<CircleCollider2D>())
        {
            gohst.AddComponent<CircleCollider2D>();
        }
        else if (GetComponent<PolygonCollider2D>())
        {
            gohst.AddComponent<PolygonCollider2D>();
        }
        else if (GetComponent<CapsuleCollider2D>())
        {
            gohst.AddComponent<CapsuleCollider2D>();
        }
        else if (GetComponent<EdgeCollider2D>())
        {
            gohst.AddComponent<EdgeCollider2D>();
        }

        gohst.GetComponent<Collider2D>().CopyCollider2D(GetComponent<Collider2D>()); //コライダーをコピー
        gohst.GetComponent<Collider2D>().isTrigger = true;
        gohst.SetActive(false);


    }



    public void StartConstructMode() //rigidbodyを持つ、Kinematic、isTriggerオンの状態にする
    {
        isConstructMode = true;
        GetComponent<Collider2D>().isTrigger = true;
        if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }

    public void EndConstructMode()
    {
        isConstructMode = false;
        GetComponent<Collider2D>().isTrigger = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().isKinematic = false;
    }

    public void Connect()
    {
        tag = "Part";
        transform.rotation *= Quaternion.Euler(0, 0, _angle);
        transform.position += _connectPoint2.transform.position - _connectPoint1.transform.position;

        Part targetPart = _connectPoint2.transform.parent.GetComponent<Part>();


        if (!targetPart.isCockpit)
        {
            Part_Frame part_Frame = (Part_Frame)targetPart;
            part_Frame.AddConnectedPart(this);
        }
        _connectPoint1.Connect(_connectPoint2);

        transform.SetParent(targetPart.transform);

        Player player = GameManager.instance.player;
        player.AddPart(this);

        isPick = false;
        canConnect = false;
        gohst.SetActive(false);
    }

    public void Pick()
    {
        isPick = true;
        GameManager.instance.player.RemovePart(this);
        PartSpawner.instance.AddFloatingPart(this);

        foreach (ConnectPoint connectPoint in connectPoints)
        {
            if (connectPoint.isConected && !connectPoint.isParent) //子としてくっついていた場合
            {
                Part parentPart = connectPoint.targetConnectPoint.transform.parent.GetComponent<Part>();
                if (!parentPart.isCockpit)
                {
                    parentPart.GetComponent<Part_Frame>().RemoveConnectedPart(this);
                }
                connectPoint.QuitConnect();
            }
        }

    }


    void Picking()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;

        if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180 * Time.deltaTime);
        }

        (ConnectPoint point1, ConnectPoint point2) = CheckConnectable();

        if (point1 && point2)
        {
            DrawGohst(point1, point2);
        }
        else
        {
            gohst.SetActive(false);
        }

        if (point1 && point2 && !gohst.GetComponent<Ghost>().IsCollide)
        {
            canConnect = true;
        }
        else
        {
            canConnect = false;
        }
    }

    public void QuitPick()
    {

        PartSpawner.instance.AddFloatingPart(this);
        tag = "Untagged";

        //if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = 0.2f;
        rb.angularDrag = 0.1f;
        rb.gravityScale = 0;
        rb.isKinematic = true;


        isPick = false;
        canConnect = false;
        gohst.SetActive(false);
    }

    (ConnectPoint, ConnectPoint) CheckConnectable()
    {
        ConnectPoint point1 = null;
        ConnectPoint point2 = null;
        float dist = ConstructManager.instance.ConnectPointDistance;

        foreach (ConnectPoint connectPoint1 in connectPoints)
        {
            foreach (Part part in GameManager.instance.player.PartsList)
            {
                //if (part == this) continue;
                foreach (ConnectPoint connectPoint2 in part.connectPoints)
                {
                    //両方がフレームモードでない場合はスキップ
                    if (!connectPoint1.isFrameMode && !connectPoint2.isFrameMode)
                    {
                        continue;
                    }

                    if (connectPoint1.isConected || connectPoint2.isConected)
                    {
                        continue;
                    }


                    float distance = (connectPoint1.transform.position - connectPoint2.transform.position).magnitude;
                    if (distance < ConstructManager.instance.ConnectPointDistance && distance < dist)
                    {
                        dist = distance;
                        point1 = connectPoint1;
                        point2 = connectPoint2;
                    }
                }
            }
        }

        _connectPoint1 = point1;
        _connectPoint2 = point2;
        return (point1, point2);
    }

    void DrawGohst(ConnectPoint myPoint, ConnectPoint targetPoint)
    {
        List<float> targetAnglelist = new List<float>();
        foreach (float angle in targetPoint.angleList)
        {
            targetAnglelist.Add(angle + targetPoint.transform.eulerAngles.z);
        }

        List<float> myAnglelist = new List<float>();
        foreach (float angle in myPoint.angleList)
        {
            myAnglelist.Add(angle + myPoint.transform.eulerAngles.z);
        }

        float minAngle = 360;
        float nearestTargetAngle = 0;
        foreach (float myAngle in myAnglelist)
        {
            foreach (float targetAngle in targetAnglelist)
            {
                float angle = (targetAngle - myAngle + 180) % 360; //-360~360

                //-180~180に変換
                if (angle > 180)
                {
                    angle -= 360;
                }
                else if (angle < -180)
                {
                    angle += 360;
                }

                if (Mathf.Abs(angle) < Mathf.Abs(minAngle))
                {
                    minAngle = angle;
                    nearestTargetAngle = targetAngle;
                }
            }
        }

        _angle = minAngle;

        gohst.SetActive(true);
        gohst.transform.position = transform.position;

        //connectPointを中心に回転
        Vector3 direction = gohst.transform.position - myPoint.transform.position;
        direction = Quaternion.Euler(0, 0, minAngle) * direction;
        gohst.transform.position = myPoint.transform.position + direction;
        gohst.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, minAngle);

        //targetPointに移動
        gohst.transform.position += targetPoint.transform.position - myPoint.transform.position;
    }

}
