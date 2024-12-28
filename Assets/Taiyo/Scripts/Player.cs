using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] float frictionRatio;
    [SerializeField] float frictionAngularRatio;
    [SerializeField] float angleCorrectionRatio;
    float angleCorrectionForceRight;
    float angleCorrectionForceLeft;

    [SerializeField] float fuelRecoverySpeed; //燃料回復速度

    [SerializeField] Transform partFolder;

    [SerializeField] Slider fuelSlider;



    Rigidbody2D rb;

    Vector2 inputVec;
    float deadZone = 0.1f;

    bool inputAction;
    bool inputActionDown;

    List<Part> partsList = new List<Part>();
    public List<Part> PartsList => partsList;


    bool isConstructMode;
    Vector2 velocityPrev;
    float angularVelocityPrev;
    float fuelConsumptionSum;

    float fuelMax;
    float fuel;
    float fuelUsableMin = 1f; //燃料がこれ以下になると動力パーツが動かなくなる
    bool isFuelEmpty;
    Color fuelColorNormal;
    Color fuelColorEmpty = new Color(1, 0, 0, 1);

    Vector2 massCenterWorldPos;


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
        fuelColorNormal = fuelSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
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
        if (!isConstructMode)
        {
            if (!isFuelEmpty)
            {
                DoPower();
                CorrectDirection();
            }
            DoHoldAction();
            DoPushAction();

            Friction();

            if (inputVec.magnitude < deadZone || isFuelEmpty) //入力がないとき or 燃料が空のときは回復
            {
                RecoveryFuel();
            }
        }

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
        SetPartPowerParameters();
        SetFuelMax();
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
        SetPartPowerParameters();
        SetFuelMax();

        if (partsList.Count == 0)
        {
            GameManager.instance.GameOver();
        }
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

        ConstructManager.instance.SetMassTextAndPos(rb.mass, massCenterWorldPos);
        ConstructManager.instance.SetRotationForceText(angleCorrectionForceRight, angleCorrectionForceLeft);
        ConstructManager.instance.SetFuelConsumptionText(fuelConsumptionSum);
        ConstructManager.instance.SetFuelVolumeText(fuelMax);

        isConstructMode = true;
    }

    public bool CanEndConstructMode()
    {
        bool hasPower = false;
        bool hasTank = false;

        foreach (Part part in partsList)
        {
            if (part is Part_Power)
            {
                hasPower = true;
            }
            else if (part is Part_Tank)
            {
                hasTank = true;
            }

            if (hasPower && hasTank)
            {
                break;
            }
        }

        if (!hasPower) GameManager.instance.DisplayMessage("動力パーツがありません");
        if (!hasTank) GameManager.instance.DisplayMessage("燃料タンクがありません");

        return hasPower && hasTank;
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

        if (fuel > fuelMax)
        {
            fuel = fuelMax;
        }

        isConstructMode = false;
    }


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
        massCenterWorldPos = massCenter / mass;
        rb.centerOfMass = transform.InverseTransformPoint(massCenterWorldPos);

        ConstructManager.instance.SetMassTextAndPos(mass, massCenterWorldPos);
    }

    void SetPartPowerParameters()
    {
        float forceRight = 0;
        float forceLeft = 0;

        float fuelSum = 0;

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

                fuelSum += part_Power.fuelConsumption;
            }

        }
        angleCorrectionForceLeft = forceLeft;
        angleCorrectionForceRight = forceRight;

        fuelConsumptionSum = fuelSum;

        ConstructManager.instance.SetRotationForceText(forceRight, forceLeft);
        ConstructManager.instance.SetFuelConsumptionText(fuelSum);
    }

    public void SetFuelMax()
    {
        float newFuelMax = 0;
        foreach (Part part in partsList)
        {
            if (part is Part_Tank)
            {
                Part_Tank part_Tank = (Part_Tank)part;
                newFuelMax += part_Tank.volume;
            }
        }
        fuelMax = newFuelMax;
        ConstructManager.instance.SetFuelVolumeText(fuelMax);
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

                    UseFuel(part_Power.fuelConsumption);
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

            if (diff > 1) rb.AddTorque((angleCorrectionForceRight + 1) * rb.mass * angleCorrectionRatio);
            else if (diff < -1) rb.AddTorque(-(angleCorrectionForceLeft + 1) * rb.mass * angleCorrectionRatio);
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

    public void UseFuel(float speed)
    {
        fuel -= speed * Time.deltaTime;
        if (fuel < 0)
        {
            fuel = 0;
            isFuelEmpty = true;

            fuelSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = fuelColorEmpty;
        }

        fuelSlider.value = fuel / fuelMax;
    }

    public void RecoveryFuel()
    {
        if (fuel < fuelMax) fuel += fuelRecoverySpeed * fuelMax * Time.deltaTime;

        //燃料が空の状態から燃料が回復した場合（消費燃料が多いほど復帰は遅い）
        if (isFuelEmpty && (fuel > fuelUsableMin * fuelConsumptionSum || fuel == fuelMax))
        {
            isFuelEmpty = false;
            fuelSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = fuelColorNormal;
        }

        if (fuel > fuelMax) fuel = fuelMax;

        if (fuelMax == 0) fuelSlider.value = 0;
        else fuelSlider.value = fuel / fuelMax;
    }





}
