using UnityEngine;
using System.Collections;

public class Parachute : MonoBehaviour, IFlyingObjects
{

    private Pilot pilot;
    private Rigidbody rigidBody;
    private GameObject parachuteLines;
    private WindController wind;
    public float angleOfAttack;
    private float originAngleOfAttack;
    private Vector3 velocityOfAir;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        pilot = GameObject.Find("Pilot").GetComponent<Pilot>();
        wind = GameObject.Find("Harita").GetComponent<WindController>();
        ConstantValues.flyables.Add(this);

        originAngleOfAttack = angleOfAttack;
    }

    void FixedUpdate()
    {

        if (!pilot.getDeployed())
        {
            //Başlangıçta paraşütü pilotun arkasına koyuyoruz.
            pushBack();
        }
        else
        {
            fly();
        }

    }

    private void fly()
    {

        //Havaya bağlı hız
        velocityOfAir = transform.InverseTransformDirection(rigidBody.velocity - wind.GetVelocity(this));

        //İleri giderken sürüklenmek. Hücum açısı daha yüksekse, ileriye sürüklenme artmaktadır.
        Vector3 forwardDrag = PhysicalAndMathRules.getDrag(velocityOfAir.z * Vector3.forward, ConstantValues.DRAG_COEFFICIENT_FRONT, ConstantValues.AIR_DENSITY_20,
            ConstantValues.AREA_FRONT * angleOfAttack / originAngleOfAttack);

        float exposedUnder = ConstantValues.AREA_UNDER / angleOfAttack * originAngleOfAttack;
        if (velocityOfAir.z <= ConstantValues.STALL_LIMIT)
        {
            exposedUnder = 2;
        }
        //Paraşütle atlamaktan kaynaklanan sürüklenme
        //Hücum açısı daha yüksekse, altındaki alan azalacak ve dolayısıyla düşüş artacaktır.
        Vector3 fallDrag = PhysicalAndMathRules.getDrag(velocityOfAir.y * Vector3.up, ConstantValues.DRAG_COEFFICIENT_UNDER, ConstantValues.AIR_DENSITY_20,
            exposedUnder);

        //Yandan sürüklenme.
        Vector3 sideDrag = PhysicalAndMathRules.getDrag(velocityOfAir.x * Vector3.right * Mathf.Abs(velocityOfAir.z), ConstantValues.DRAG_COEFFICIENT_SIDE, ConstantValues.AIR_DENSITY_20,
                               ConstantValues.AREA_SIDE);

        //Düşme direnci, kanadın ne kadarının havaya maruz kaldığına bağlı olarak daima yerçekiminin tersi yönündedir.
        rigidBody.AddForce(fallDrag);

        //Koordinat sistemine göre cisme bir kuvvet ekler.
        rigidBody.AddRelativeForce(getGlide(velocityOfAir.y, ConstantValues.PLAYER_WEIGHT) + forwardDrag + getLift(velocityOfAir.z) + sideDrag);


        braking();

        if (Input.GetAxis("speed") > 0)
        {
            speed();
        }
    }

    private Vector3 getLift(float relativeAirSpeed)
    {
        if (relativeAirSpeed > ConstantValues.STALL_LIMIT)
        {  
            return (Vector3.up * Mathf.Pow(relativeAirSpeed, 2) * angleOfAttack); //Speed makes lift
        }
        else
        { 
            return Vector3.zero;
        }
    }

    private Vector3 getGlide(float fallVelocity, float mass)
    {
        return Vector3.forward * -fallVelocity * mass; //Fall makes speed
    }

    public bool flyAble()
    { 
        return transform.rotation.eulerAngles.x < ConstantValues.FLYABLE_ANGLE &&
            transform.rotation.eulerAngles.x > -ConstantValues.FLYABLE_ANGLE;
    }
    //Sağ ve sol fren
    private void braking()
    {

        //Frenleme sırasında kazanılan sürtünme 
        Vector3 brakeDrag = PhysicalAndMathRules.getDrag(rigidBody.velocity - wind.GetVelocity(this), ConstantValues.DRAG_COEFFICIENT_UNDER, ConstantValues.AIR_DENSITY_20,
                           ConstantValues.AREA_BRAKE);

        Vector3 brakeRightPos = transform.TransformPoint(Vector3.right * 15 + Vector3.back);
        Vector3 brakeLeftPos = transform.TransformPoint(Vector3.left * 15 + Vector3.back);

        rigidBody.AddForceAtPosition(brakeDrag * Input.GetAxis("brakeRight"), brakeRightPos);
        rigidBody.AddForceAtPosition(brakeDrag * Input.GetAxis("brakeLeft"), brakeLeftPos);

        //Frenleme yaparken hücüm açısı artar
        angleOfAttack = originAngleOfAttack + (Input.GetAxis("brakeRight") + Input.GetAxis("brakeLeft")) * 8;
    }


    private void pushBack()
    {
        //Açılırken paraşütü pilotun arkasına yerleştirir
        if (!pilot.getDeployed())
        {
            rigidBody.useGravity = false;
            rigidBody.AddRelativeForce(Vector3.back * 10000);
            rigidBody.useGravity = true;
        }
    }

    private void speed()
    {
        float maxSpeedDegrees = originAngleOfAttack - ConstantValues.SPEED_LIMIT;
        angleOfAttack = originAngleOfAttack - Input.GetAxis("speed") * maxSpeedDegrees;
    }


    public void applyWind(Vector3 wind)
    {
        rigidBody.AddForce(PhysicalAndMathRules.GetWindForce(wind - rigidBody.velocity, ConstantValues.AREA_FRONT, ConstantValues.DRAG_COEFFICIENT_FRONT));
    }

    public Vector3 GetWorldPosition()
    {
        return rigidBody.position;
    }
}
