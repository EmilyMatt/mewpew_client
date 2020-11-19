using System.Collections;
using UnityEngine;

public class PlayerShipControls : MonoBehaviour
{
    public bool shipEnabled = false;

    private float maxSpeed;
    private float maxAfterSpeed;
    private float acceleration;
    private float afterAcceleration;
    private float afterTimeMax;

    private float throttle;
    private float afterburner;
    private float afterTime;
    private bool afterEnabled = true;

    private float pitchAcc;
    private float rollAcc;
    private float yawAcc;

    private float pitch;
    private float roll;
    private float yaw;

    private float hull;
    private float maxHull;
    private float shields;
    private float maxShields;
    private float eRadius = 10;

    private Rigidbody _rigidbody;
    private int times = 150;
    private PlayerShip myShip;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        myShip = GameData.myShip;

        maxSpeed = myShip.MaxSpeed;
        maxAfterSpeed = myShip.MaxAfterburnerSpeed;

        acceleration = myShip.Acceleration;
        afterAcceleration = myShip.AfterburnerAcceleration;

        pitchAcc = myShip.Pitch;
        rollAcc = myShip.Roll;
        yawAcc = myShip.Yaw;

        afterTimeMax = myShip.AfterburnerTime;
        afterTime = afterTimeMax;

        maxHull = myShip.Hull;
        hull = maxHull;
        maxShields = myShip.Shields;
        shields = maxShields;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(shipEnabled)
        {
            var mouse = GetMouseInput();
            float rollInput = Input.GetAxis("Roll");

            float throttleInput = Input.GetAxis("Throttle");
            float afterburnerInput = Input.GetAxis("Afterburner");

            //add acceleration to velocities
            throttle += throttleInput * acceleration * Time.deltaTime;
            afterburner += afterburnerInput * afterAcceleration * Time.deltaTime;

            //add rotation velocities
            pitch -= mouse.Item2 * pitchAcc * Time.deltaTime;
            roll += rollInput * rollAcc * Time.deltaTime;
            yaw += mouse.Item1 * yawAcc * Time.deltaTime;

            //apply velocities
            _rigidbody.velocity += transform.forward * throttle + transform.forward * afterburner;
            _rigidbody.angularVelocity += transform.forward * roll;
            _rigidbody.angularVelocity += transform.up * yaw;
            _rigidbody.angularVelocity += transform.right * pitch;

            //clamp velocity
            Vector3 localVel = transform.InverseTransformDirection(_rigidbody.velocity);
            localVel.z = Mathf.Clamp(localVel.z, -maxSpeed * 0.05f, maxSpeed + maxAfterSpeed * afterburner);
            _rigidbody.velocity = transform.TransformDirection(localVel);

            //apply afterburner
            afterTime -= afterburner;

            if (afterburnerInput == 0 && afterEnabled)
                afterTime += afterTimeMax * Time.deltaTime * 0.3f;

            afterTime = Mathf.Clamp(afterTime, 0, afterTimeMax);

            if (afterTime == 0)
                afterEnabled = false;

            if (!afterEnabled)
            {
                StartCoroutine(ReloadAfterburner());
            }
        }
        
        //add "drag"
        throttle *= 0.99f;
        afterburner *= 0.99f;
        roll *= 0.5f;
        pitch *= 0.5f;
        yaw *= 0.7f;


        if (times == 0)
            SendDataToServer();

        times--;

    }

    void SendDataToServer()
    {
        times = 150;
        ClientListener.SendDataUDP($"usp?i={transform.name}" +
            $"&p={Message.ParseVector3(transform.position)}" +
            $"&r={Message.ParseVector3(transform.eulerAngles)}" +
            $"&v={Message.ParseVector3(_rigidbody.velocity)}" +
            $"&av={Message.ParseVector3(_rigidbody.angularVelocity)}");
    }

    (float, float) GetMouseInput()
    {
        Vector3 screen = new Vector3(Screen.width, Screen.height);
        Vector3 mousePos = Input.mousePosition;
        float x = (mousePos.x/screen.x)*2-1;
        float y = (mousePos.y/screen.y)*2-1;
        x *= 2;
        y *= 2;
        if (x > -0.2f && x < 0.2f)
            x = 0;
        if (y > -0.4f && y < 0.4f)
            y = 0;
        x = Mathf.Clamp(x, -1, 1);
        y = Mathf.Clamp(y, -1, 1);
        return (x, y);
    }

    IEnumerator ReloadAfterburner()
    {
        yield return new WaitForSeconds(afterTimeMax / 300);
        afterEnabled = true;
    }

    public void ResetValues()
    {
        acceleration = myShip.Acceleration;
        afterAcceleration = myShip.AfterburnerAcceleration;
        maxSpeed = myShip.MaxSpeed;
        maxAfterSpeed = myShip.MaxAfterburnerSpeed;
        pitchAcc = myShip.Pitch;
        rollAcc = myShip.Roll;
        yawAcc = myShip.Yaw;
    }

}
