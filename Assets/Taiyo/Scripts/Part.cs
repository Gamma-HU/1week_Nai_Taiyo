using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Part : MonoBehaviour
{
    [SerializeField] public float HP = 100;
    public float mass;
    //public bool isFrame;
    public bool isCockpit;
    public bool isConstructMode;
    bool isPick;
    bool canConnect;
    public bool CanConnect => canConnect;
    public bool isConnected;

    [SerializeField] List<ConnectPoint> connectPoints = new List<ConnectPoint>();
    GameObject ghost;

    ConnectPoint _connectPoint1;
    ConnectPoint _connectPoint2;
    float _angle;

    Rigidbody2D rb;


    protected virtual void Awake()
    {
        connectPoints.Clear();
        connectPoints.AddRange(GetComponentsInChildren<ConnectPoint>());

        foreach (ConnectPoint connectPoint in connectPoints)
        {
            connectPoint.ActivateImage(false);
        }

        if (isCockpit) return;

        if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = 0.2f;
        rb.angularDrag = 0.1f;
        rb.gravityScale = 0;
        rb.isKinematic = false;

        InitializeGhost();



    }

    void Update()
    {

        if (isPick)
        {
            Picking();
        }


    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DamageSource")
        {
            DamageParameter damageParameter = collision.gameObject.GetComponent<DamageParameter>();
            this.HP -= damageParameter.damage;
            damageParameter.collisionCount += 1;
            if (this.HP <= 0)
            {
                DeattachFromParent();
            }
        }
    }

    void InitializeGhost()
    {
        ghost = transform.Find("Ghost").gameObject;
        ghost.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        ghost.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);  //透明度30%

        if (GetComponent<BoxCollider2D>())
        {
            ghost.AddComponent<BoxCollider2D>();
        }
        else if (GetComponent<CircleCollider2D>())
        {
            ghost.AddComponent<CircleCollider2D>();
        }
        else if (GetComponent<PolygonCollider2D>())
        {
            ghost.AddComponent<PolygonCollider2D>();
        }
        else if (GetComponent<CapsuleCollider2D>())
        {
            ghost.AddComponent<CapsuleCollider2D>();
        }
        else if (GetComponent<EdgeCollider2D>())
        {
            ghost.AddComponent<EdgeCollider2D>();
        }

        ghost.GetComponent<Collider2D>().CopyCollider2D(GetComponent<Collider2D>()); //コライダーをコピー
        ghost.GetComponent<Collider2D>().isTrigger = true;
        ghost.SetActive(false);


    }



    public void StartConstructMode() //rigidbodyを持つ、Kinematic、isTriggerオンの状態にする
    {
        isConstructMode = true;

        foreach (ConnectPoint connectPoint in connectPoints)
        {
            connectPoint.ActivateImage(true);
        }

        if (isCockpit) return;

        if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = 0.2f;
        rb.angularDrag = 0.1f;
        rb.gravityScale = 0;
        rb.isKinematic = isConnected;
        GetComponent<Collider2D>().isTrigger = isConnected;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

    }

    public void EndConstructMode()
    {
        isConstructMode = false;
        if (rb) //playerにくっついていないとき
        {
            rb.isKinematic = isConnected;
            GetComponent<Collider2D>().isTrigger = isConnected;
        }

        foreach (ConnectPoint connectPoint in connectPoints)
        {
            connectPoint.ActivateImage(false);
        }
    }

    public void Connect()
    {
        PartSpawner.instance.RemoveFloatingPart(this);

        tag = "Part";
        transform.rotation *= Quaternion.Euler(0, 0, _angle);
        float z = transform.position.z;
        transform.position += _connectPoint2.transform.position - _connectPoint1.transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);

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

        isConnected = true;
        isPick = false;
        canConnect = false;
        ghost.SetActive(false);


        GetComponent<Collider2D>().isTrigger = true;
        rb.isKinematic = true;
    }

    public void Pick()
    {

        GetComponent<Collider2D>().isTrigger = false;
        rb.isKinematic = true;

        isPick = true;
        isConnected = false;
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
        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);

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
            ghost.SetActive(false);
        }

        if (point1 && point2 && !ghost.GetComponent<Ghost>().IsCollide)
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
        tag = "Untagged";
        rb.isKinematic = false;
        isPick = false;
        canConnect = false;
        ghost.SetActive(false);
    }

    public void DeattachFromParent()
    {
        isConnected = false;
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

        ghost.SetActive(true);
        ghost.transform.position = transform.position;

        //connectPointを中心に回転
        Vector3 direction = ghost.transform.position - myPoint.transform.position;
        direction = Quaternion.Euler(0, 0, minAngle) * direction;
        ghost.transform.position = myPoint.transform.position + direction;
        ghost.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, minAngle);

        //targetPointに移動
        ghost.transform.position += targetPoint.transform.position - myPoint.transform.position;
    }

}
