using UnityEngine;
using System.Collections;

public class Pilot : MonoBehaviour, IFlyingObjects
{

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private float speed = 6;
    public bool deployed;
    public bool flying;
    private Rigidbody flyingBody;
    private Quaternion startRotation;
    public Vector3 relativeVelocityAir;

    public GameObject objectToCopy;
    public Transform home;
    private Rigidbody harnessBody;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        flyingBody = GetComponent<Rigidbody>();
        startRotation = flyingBody.rotation;
        ConstantValues.flyables.Add(this);

        harnessBody = GameObject.FindWithTag("Harness").GetComponent<Rigidbody>();

        if (home == null)
            home = gameObject.transform;

      


    }

    void Update()
    {

        if (flying)
        {
            WeightShift();
        }

    }

    void FixedUpdate()
    {
        playerControl();

        


    }

    void OnTriggerEnter(Collider collider)
    { // Yerde olduğunda
        if (collider.gameObject.name == "Harita")
        {
            setFlying(false);

        }
    }

    void OnTriggerExit(Collider collider)
    { //Yerden kalkarken
        if (collider.gameObject.name == "Harita" && deployed)
        {
            setFlying(true);
        }
    }
    public bool getDeployed()
    {
        return deployed;
    }




    public void applyWind(Vector3 wind)
    { //Rüzgar farklı yönden geldiğinde alan değişmelidir
        relativeVelocityAir = transform.InverseTransformVector(flyingBody.velocity - wind);
        flyingBody.AddForce(PhysicalAndMathRules.GetWindForce(wind - flyingBody.velocity, ConstantValues.AREA_PLAYER_FRONT, ConstantValues.DRAG_COEFFICIENT_PLAYER_FRONT));
    }

    public Vector3 GetWorldPosition()
    {
        return flyingBody.position;
    }





    private void unDeployedControl()
    {

        //Pilot yerdeyse bu koda girer
        if (controller.isGrounded)
        {
            //Shifte basarak koşabilirsiniz
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 12;
            }
            else
            {
                speed = 6;
            }

            //Kullanıcı girdilerini ekleriz. A ve D sağ sola gitmeye yarar. W ve S ise ileri geri gitmeye
            moveDirection = new Vector3(Input.GetAxis("sideways"), 0, Input.GetAxis("forward"));

            moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= speed;

        }
        else
        {
            moveDirection.y -= ConstantValues.GRAVITY;
        }


        controller.Move(moveDirection * Time.deltaTime);

    }


    private void deployedControl()
    {
        //ileri gitmek
        flyingBody.AddRelativeForce(Vector3.forward * Input.GetAxis("forward") * 1000);
    }

    private void playerControl()
    {
        if (!flying)
        {
          
            if (Input.GetKeyUp(KeyCode.Space) && flyingBody.velocity.y < 0.5f && flyingBody.velocity.y > -0.5f)
            { 
                setDeployed(!deployed);
            }
            if (deployed)
            {
                deployedControl();
            }
            else
            {
                unDeployedControl();
            }
        }
    }

    private void WeightShift()
    {

        //Pilotun ağırlık aktarımı.Dönüşlerde katkı sağlar
        flyingBody.centerOfMass = Vector3.right * Input.GetAxis("leanRight") * 3;
    }

    private void setFlying(bool flying)
    {
        this.flying = flying;
        flyingBody.freezeRotation = !flying;
        if (flyingBody.freezeRotation)
        {
            flyingBody.rotation = Quaternion.Euler(startRotation.eulerAngles.x,
                                                    flyingBody.rotation.eulerAngles.y, startRotation.eulerAngles.z);
        }
    }

    private void setDeployed(bool deployed)
    { //Modu dağıtılmış veya dağıtılmamış olarak ayarlar

        //
        this.deployed = deployed;
        controller.enabled = !deployed;
        flyingBody.useGravity = deployed;
        flyingBody.isKinematic = !deployed;
    }



}
