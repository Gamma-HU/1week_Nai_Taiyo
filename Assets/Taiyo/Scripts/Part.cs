using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Part : MonoBehaviour
{
    [SerializeField] float hpMax;
    float hp;
    PartData partData;

    public void Initialize(PartData partData)
    {
        this.partData = partData;
        hpMax = this.partData.hp;
        hp = hpMax;
    }


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

    HPbarPart hpbar;
    float damageTimer;


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

        hp = hpMax;

    }

    protected virtual void Update()
    {

        if (isPick)
        {
            Picking();
        }

        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        else if (hp < hpMax)
        {
            Recover();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isConnected) return;

        if (other.gameObject.tag == "DamageSource")
        {
            DamageParameter damageParameter = other.gameObject.GetComponent<DamageParameter>();
            damageParameter.collisionCount += 1;
            Damage(damageParameter.damage);

            GameObject eff = Instantiate(GameManager.instance.player.damageEffectPfb, transform);
            eff.transform.position = other.ClosestPoint(transform.position);

            Destroy(other.gameObject);

            GameManager.instance.ShakeCamera(damageParameter.damage);
        }
    }

    void InitializeGhost()
    {
        ghost = transform.Find("Ghost").gameObject;
        ghost.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        ghost.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);  //透明度30%


        if (GetComponent<CapsuleCollider2D>())
        {
            ghost.AddComponent<CapsuleCollider2D>();
        }

        ghost.GetComponent<CapsuleCollider2D>().direction = GetComponent<CapsuleCollider2D>().direction;
        ghost.GetComponent<CapsuleCollider2D>().size = GetComponent<CapsuleCollider2D>().size;
        ghost.GetComponent<CapsuleCollider2D>().offset = GetComponent<CapsuleCollider2D>().offset;

        ghost.GetComponent<Collider2D>().isTrigger = true;
        // ghost.SetActive(false);
        SetAllGhostActive(false);


    }



    public virtual void StartConstructMode() //rigidbodyを持つ、Kinematic、isTriggerオンの状態にする
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

    public virtual void Connect()
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
        // ghost.SetActive(false);
        SetAllGhostActive(false);

        GetComponent<Collider2D>().isTrigger = true;
        rb.isKinematic = true;


        GameObject eff = Instantiate(GameManager.instance.player.equipEffectPfb, transform);
        eff.transform.position = _connectPoint2.transform.position;
    }

    public virtual void Pick()
    {

        GetComponent<Collider2D>().isTrigger = false;
        rb.isKinematic = true;

        isPick = true;
        isConnected = false;
        if (GameManager.instance.player.PartsList.Contains(this)) GameManager.instance.player.RemovePart(this);
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
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180 * Time.unscaledDeltaTime);
        }

        (ConnectPoint point1, ConnectPoint point2) = CheckConnectable();

        if (point1 && point2)
        {
            DrawGohst(point1, point2);
        }
        else
        {
            // ghost.SetActive(false);
            SetAllGhostActive(false);
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

    public virtual void QuitPick()
    {
        tag = "Untagged";
        rb.isKinematic = false;
        isPick = false;
        canConnect = false;
        // ghost.SetActive(false);
        SetAllGhostActive(false);
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

        if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = 0.2f;
        rb.angularDrag = 0.1f;
        rb.gravityScale = 0;
        rb.isKinematic = false;
        GetComponent<Collider2D>().isTrigger = false;

        rb.AddForce((transform.position - GameManager.instance.player.transform.position).normalized * mass * 10, ForceMode2D.Impulse);
        rb.AddTorque(mass * 10, ForceMode2D.Impulse);


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
        List<(GameObject, Vector3)> list = GetChildGhosts(this, transform.position);
        foreach ((GameObject g, Vector3 pos) in list)
        {
            // g.SetActive(true);
            SetAllGhostActive(true);
            g.transform.position = transform.position;

            //connectPointを中心に回転
            Vector3 direction = g.transform.position - myPoint.transform.position;
            direction = Quaternion.Euler(0, 0, minAngle) * direction;
            g.transform.position = myPoint.transform.position + direction;
            g.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, minAngle);

            //targetPointに移動
            g.transform.position += targetPoint.transform.position - myPoint.transform.position + pos;
        }

    }

    List<(GameObject, Vector3)> GetChildGhosts(Part part, Vector3 parentPos) //子供のゴーストを全部持ってきて相対座標とともに返す
    {
        List<(GameObject, Vector3)> list = new List<(GameObject, Vector3)>();
        list.Add((part.ghost, part.transform.position - parentPos));
        foreach (ConnectPoint connectPoint in part.connectPoints)
        {
            if (connectPoint.isConected && connectPoint.isParent)
            {
                Part childPart = connectPoint.targetConnectPoint.transform.parent.GetComponent<Part>();

                if (childPart != null)
                {
                    list.AddRange(GetChildGhosts(childPart, parentPos));
                }
            }
        }
        return list;
    }

    void SetAllGhostActive(bool active)
    {
        List<(GameObject, Vector3)> list = GetChildGhosts(this, Vector3.zero);
        foreach ((GameObject g, Vector3 pos) in list)
        {
            g.SetActive(active);
        }
    }

    public virtual void Damage(float damage)
    {
        hp -= damage;
        damageTimer = 5f;
        if (hpbar == null)
        {
            GameObject hpbarObj = Instantiate(GameManager.instance.hPbarPartPfb, GameManager.instance.hpbarFolder.transform);
            hpbar = hpbarObj.GetComponent<HPbarPart>();
        }

        if (hp <= 0)
        {
            hp = 0;
            DeattachFromParent();
        }


        hpbar.SetPart(this);
        hpbar.SetHP(hp / hpMax);
    }

    void Recover()
    {
        if (isConnected)
        {
            hp += hpMax * GameManager.instance.player.partRecoverySpeed * Time.deltaTime;
        }
        else
        {
            hp += hpMax * 0.1f * Time.deltaTime; //浮いているパーツは10秒で回復
        }


        if (hp > hpMax)
        {
            hp = hpMax;
            Destroy(hpbar.gameObject);
            return;
        }

        if (hpbar)
        {
            hpbar.SetHP(hp / hpMax);
        }
    }
}