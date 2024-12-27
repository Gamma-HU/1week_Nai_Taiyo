using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float frictionRatio;
    [SerializeField] float frictionAngularRatio;
    [SerializeField] float angleCorrectionRatio;
    float angleCorrectionForceRight;
    float angleCorrectionForceLeft;

    [SerializeField] Transform partFolder;


    Rigidbody2D rb;

    Vector2 inputVec;
    float deadZone = 0.1f;

    bool inputAction;
    bool inputActionDown;

    List<Part> partsList = new List<Part>();
    public List<Part> PartsList => partsList;

    Vector2 velocityPrev;
    float angularVelocityPrev;

    void OnDrawGizmos()
    {
        if (rb != null)
        {
            // ギズモの色を設定
            Gizmos.color = Color.red;

            // Rigidbody2Dの重心のワールド座標
            Vector2 centerOfMassWorld = rb.worldCenterOfMass;

            // 重心の位置に小さな球を描画
            Gizmos.DrawSphere(centerOfMassWorld, 0.1f);

            // オブジェクトの位置から重心まで線を描画
            Gizmos.DrawLine(transform.position, centerOfMassWorld);
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void Start()
    {
        partsList.Clear();
        partsList.AddRange(partFolder.GetComponentsInChildren<Part>());

        SetMass();
    }

    // Update is called once per frame
    void Update()
    {
        SetInput();


    }

    void FixedUpdate()
    {
        DoPower();
        CorrectDirection();
        DoHoldAction();
        DoPushAction();

        Friction();

        ResetInput();
    }

    void SetInput()
    {
        inputVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.Space))
        {
            if (!inputAction) inputActionDown = true;
            inputAction = true;
        }
        else
        {
            inputAction = false;
        }
    }

    void ResetInput()
    {
        inputActionDown = false;
    }

    /*
        public void SetParts()
        {
            partsList.Clear();
            //子オブジェクトのPartを取得
            foreach (Part part in partFolder.GetComponentsInChildren<Part>())
            {
                partsList.Add(part);

            }

            SetMass();
            SetAngleCorrectionForce();
        }
        */

    void SetMass()
    {
        //各部品の質量を合計してプレイヤーの質量にする
        float mass = 0;
        Vector3 massCenter = Vector3.zero;
        foreach (Part part in partsList)
        {
            mass += part.mass;
            massCenter += part.transform.position * part.mass;
        }
        rb.mass = mass;

        //重心を設定
        rb.centerOfMass = transform.InverseTransformPoint(massCenter / mass);
    }

    void SetAngleCorrectionForce()
    {
        float forceRight = 0;
        float forceLeft = 0;

        foreach (Part part in partsList)
        {
            if (part is Part_Power)
            {
                Part_Power part_Power = (Part_Power)part;
                Vector2 powerDirection = part_Power.transform.rotation * part_Power.powerVec;
                Vector2 relativePosition = part_Power.transform.position - (Vector3)rb.worldCenterOfMass; //重心からの相対位置
                Vector2 verticalComponent = powerDirection - Vector2.Dot(powerDirection, relativePosition) / relativePosition.magnitude * relativePosition.normalized; //力の方向と重心からの相対位置の内積を取り、重心からの相対位置の方向に垂直な成分を取得
                //反時計回りが正のトルクの大きさ
                float torque = -1 * Mathf.Sign(Vector3.Cross(relativePosition, verticalComponent).z) * verticalComponent.magnitude * part_Power.power; //外積を取りトルクを計算

                if (torque > 0)
                {
                    forceLeft += torque;
                }
                else
                {
                    forceRight += -torque;
                }
            }

        }
        angleCorrectionForceLeft = forceLeft;
        angleCorrectionForceRight = forceRight;
    }

    public void AddPart(Part part)
    {
        if (!partsList.Contains(part)) partsList.Add(part);
        else return;

        if (part is Part_Frame)
        {
            Part_Frame part_Frame = (Part_Frame)part;
            foreach (Part childPart in part_Frame.connectedParts)
            {
                AddPart(childPart);
                if (!partsList.Contains(part)) partsList.Add(childPart);
            }
        }
        SetMass();
        SetAngleCorrectionForce();
    }

    public void RemovePart(Part part)
    {
        partsList.Remove(part);
        if (part is Part_Frame)
        {
            Part_Frame part_Frame = (Part_Frame)part;
            foreach (Part childPart in part_Frame.connectedParts)
            {
                RemovePart(childPart);
                partsList.Remove(childPart);
            }
        }

        SetMass();
        SetAngleCorrectionForce();
    }



    public void StartConstructMode()
    {
        velocityPrev = rb.velocity;
        angularVelocityPrev = rb.angularVelocity;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (Part part in partsList)
        {
            ConstructManager.instance.SetConstructingParts(part);
        }
    }

    public void EndConstructMode()
    {
        foreach (Part part in partsList)
        {
            //部品のRigidbody2Dを削除
            if (part.GetComponent<Rigidbody2D>()) Destroy(part.GetComponent<Rigidbody2D>());
            part.GetComponent<Collider2D>().isTrigger = false;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = velocityPrev;
        rb.angularVelocity = angularVelocityPrev;
    }

    void Friction()
    {
        rb.velocity *= frictionRatio;
        rb.angularVelocity *= frictionAngularRatio;
    }

    //動力パーツの力を加える
    void DoPower()
    {
        if (inputVec.magnitude > deadZone) //入力がある場合
        {
            foreach (Part part in partsList)
            {
                //動力パーツなら力を加える
                if (part is Part_Power)
                {
                    Part_Power part_Power = (Part_Power)part;
                    part_Power.AddPower(rb, inputVec);
                }
            }
        }
    }

    //入力方向に船体の向きを補正する
    void CorrectDirection()
    {
        if (inputVec.magnitude > deadZone)
        {
            //入力方向
            float inputAngle = Mathf.Atan2(inputVec.y, inputVec.x) * Mathf.Rad2Deg;
            if (inputAngle < 0) inputAngle += 360; //0~360に変換

            //現在の船体の向き
            float angleNow = (transform.rotation.eulerAngles.z + 90f) % 360;

            float diff = inputAngle - angleNow;
            if (diff > 180)
            {
                diff -= 360;
            }
            else if (diff < -180)
            {
                diff += 360;
            }

            //if (Mathf.Abs(diff) < 30) diff = math.sign(diff) * 30; //補正する力は30~180の係数で変化
            Debug.Log(diff);

            if (diff > 10) rb.AddTorque((angleCorrectionForceRight + 1) * rb.mass * angleCorrectionRatio);
            else if (diff < -10) rb.AddTorque(-(angleCorrectionForceLeft + 1) * rb.mass * angleCorrectionRatio);
        }
    }

    void DoHoldAction()
    {
        if (inputAction)
        {
            foreach (Part part in partsList)
            {
                if (part is Part_Action)
                {
                    Part_Action part_Action = (Part_Action)part;
                    part_Action.HoldAction();
                }
            }
        }
    }

    void DoPushAction()
    {
        if (inputActionDown)
        {
            foreach (Part part in partsList)
            {
                if (part is Part_Action)
                {
                    Part_Action part_Action = (Part_Action)part;
                    part_Action.PushAction();
                }
            }
        }
    }

}
