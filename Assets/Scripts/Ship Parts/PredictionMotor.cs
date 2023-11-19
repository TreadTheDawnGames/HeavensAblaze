using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System;
using System.Collections.Generic;
using FishNet.Transporting;
using FishNet.Observing;
using System.Linq;
using System.Collections;
using FishNet.Component.Prediction;
using UnityEngine.InputSystem;
using System.Data;
using FishNet.Managing.Timing;

public class PredictionMotor : NetworkBehaviour
{



    #region Types.
    /* It's strongly recommended to use structures for your data.
     * Datas are cached on client and server, and will create garbage
     * if you use a class. */

    /* MoveData may be named whatever you like. In my script it's used to
     * store client inputs, which are later used to move the object identically
     * on the server and owner. */
    public struct MoveData : IReplicateData
    {
        public float Thrust;
        public float Lift;
        public float Lateral;
        public float Pitch;
        public float Roll;
        public float Yaw;
        public bool Brake;
        public bool Fire;

        public MoveData(float thrust, float lift, float lateral, float pitch, float roll, float yaw, bool brake, bool fire)
        {
            Thrust = thrust;
            Lift = lift;
            Lateral = lateral;
            Pitch = pitch;
            Roll = roll;
            Yaw = yaw;
            Brake = brake;
            Fire = fire;
            _tick = 0;
        }
        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;



    }

    /* ReconcileData may also be named differently. This contains data about how
     * to reset the object to the server values. These values will be sent to the client. */
    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, float time)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            _tick = 0;
        }
        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }
    #endregion

    #region Misc
    /// <summary>
    /// How much force to add per move.
    /// </summary>
    public float MoveRate = 30f;
    /// <summary>
    /// Rigidbody on this object.
    /// </summary>
    private Rigidbody _rigidbody;
    public enum InputType { Joystick, Keyboard, Gamepad, Mouse, Disabled }
    public InputType inputType = InputType.Joystick;
    /// <summary>
    /// True if subscribed to the TimeManager.
    /// </summary>
    private bool _subscribed = false;

    public float moveSpeed = 0.2f;

    [SerializeField]
    private float thrustMultiplier = 2;
    private float rotSpeed = 1f;
    private float spaceStablizeSpeed = 0.15f;
    private float stablizeBrake = 0.75f;

    /// <summary>
    ///[SerializeField]
    /// </summary>
    //private AnimationCurve shipCurve;
    // float shipSensitivityValue;

    private float _nextShootTime = 0;

    private bool fire = false;

    [SerializeField]
    private float spaceBrakeSpeed = 0.15f;

    private float clampVelocityMagnitude = 50f;

    public PlayerShip playerShip;



    public List<BlasterV3> blasters = new List<BlasterV3>();
    public float _thrust;

    public bool blastersUseAimpoint = true;

    private IdleCamera activeIdleCam;

    public GameObject mainCam;
    public GameObject thirdPersonCam;


    [SerializeField]
    [SyncVar(ReadPermissions = ReadPermission.Observers)]
    public Color syncedLaserColor;
    public Color publicLaserColor;


    public int invertThrust, invertLift, invertLateral, invertPitch, invertRoll, invertYaw;


    public ColorPicker colorPicker;

    [SerializeField]
    public InputManager inputManager;
    [SerializeField]
    PersonalizationManager personalizationManager;
    [SerializeField]
    public MainMenu mainMenu;



    #endregion
    

    private void Awake()
    {
        

        
        _rigidbody = GetComponent<Rigidbody>();
        blasters = GetComponentsInChildren<BlasterV3>().ToList<BlasterV3>();
        foreach (BlasterV3 blaster in blasters)
        {
            blaster.myShip = this;
            blaster.Setup();
        }
       

    }

    
            

    [ServerRpc(RequireOwnership=true)]
    public void ChangeColor(PredictionMotor script, Color changeToColor)
    {
        script.syncedLaserColor = changeToColor;
    }

    public void ChangeBlastersUseAimpoint(bool useAimpoint)
    {
        foreach (BlasterV3 blaster in blasters)
        {
            blaster.isUsingAimpoint = useAimpoint;
        }
    }
       

    #region Managing
    /// <summary>
    /// Subscribe or unsubscribe to the TimeManager for Tick events.
    /// </summary>
    /// <param name="subscribe"></param>
    private void SubscribeToTimeManager(bool subscribe)
    {
        //TimeManager could be null if exiting the application or not yet initialized.
        if (base.TimeManager == null)
            return;

        /* If already subscribed/unsubscribed there
         * is no need to do it again. */
        if (subscribe == _subscribed)
            return;
        _subscribed = subscribe;

        if (subscribe)
        {
            base.TimeManager.OnTick += TimeManager_OnTick;
            base.TimeManager.OnPostTick += TimeManager_OnPostTick;


        }
        else
        {
            base.TimeManager.OnTick -= TimeManager_OnTick;
            base.TimeManager.OnPostTick -= TimeManager_OnPostTick;

        }
    }
    private void OnDestroy()
    {
        

        

        /* When the object is destroyed remove its TimeManager
         * subscription. This is so the events are not calling
         * to a null object. */
        SubscribeToTimeManager(false);
    }

    public float collisionDivider = 5f;
    private void OnCollisionEnter(Collision collision)
    {
            ShipPart childPart = collision.GetContact(0).thisCollider.GetComponent<ShipPart>();
        if(IsClient)
            childPart.damageHudCounterpart?.GetComponent<DamageHologram>()?.UpdateCounterpart(childPart.hitPoints);

        if (IsServer)
        {


            if (_rigidbody.velocity.magnitude > 2)
                childPart.hitPoints -= _rigidbody.velocity.magnitude / collisionDivider;


            childPart.DestroyIfDead();
        }
    }
   


    public List<ShipPart> shipParts = new List<ShipPart>();
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            gameObject.GetComponent<PredictionMotor>().enabled = false;
            
        }

        if (IsOwner)
        {






            invertThrust = PlayerPrefs.GetInt("invertThrust") == 1 ? -1 : 1;
            invertLift = PlayerPrefs.GetInt("invertLift") == 1 ? -1 : 1;
            invertLateral = PlayerPrefs.GetInt("invertLateral") == 1 ? -1 : 1;
            invertRoll = PlayerPrefs.GetInt("invertRoll") == 1 ? -1 : 1;
            invertPitch = PlayerPrefs.GetInt("invertPitch") == 1 ? -1 : 1;
            invertYaw = PlayerPrefs.GetInt("invertYaw") == 1 ? -1 : 1;

            //mainCam=transform.GetComponentInChildren<CameraDampener>().gameObject;

            colorPicker = FindObjectOfType<ColorPicker>();
            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>();


            }
            if (personalizationManager == null)
            {
                personalizationManager = FindObjectOfType<PersonalizationManager>();


            }
            personalizationManager.aimpoint = GetComponentInChildren<AimPoint>();
            if (mainMenu == null)
            {
                mainMenu = FindObjectOfType<MainMenu>();


            }

            personalizationManager.ship = this;
            inputManager.ship = this;
            colorPicker.ship = this;
            mainMenu.ship = this;

            if (playerShip == null)
            {
                playerShip = new PlayerShip();
            }
            inputManager.ChangeInputTypeAndActivateShip(PlayerPrefs.GetInt("inputType", 0));

            /* Both the server and owner must have a reference to the rigidbody.
             * Forces are applied to both the owner and server so that the objects
             * move the same. This value could be set in OnStartServer and OnStartClient
             * but to keep it simple I'm using Awake. */
            //ChangeColor(this,PlayerPrefs.GetString("LaserColor"));
            //Debug.Log(PlayerPrefs.GetString("LaserColor"));

            //shipCurve = inputManager.curve;

            
            foreach (IdleCamera cam in FindObjectsOfType<IdleCamera>().ToList<IdleCamera>())
            {
                if (cam.name == "Idle Camera")
                {
                    activeIdleCam = cam;
                    //                break;
                }


            }
            foreach (Camera camera in FindObjectsOfType<Camera>().ToList<Camera>())
            {
                if (camera.transform.root.name.StartsWith("DEBRIS")|| camera.transform.root.GetComponent<CameraDampener>() != null)
                {
                    Destroy(camera.gameObject);
                }
            }











            mainCam = GetCamInChildren(transform).gameObject;
            mainCam.SetActive(true);

            GetChildRecursive(gameObject);
            int i = 0;

            foreach (ShipPart part in shipParts)
            {
                if (part!=null)
                {
                    part.partId = i;
                    i++;
                }

            }

        }


        /* The TimeManager won't be set until at least
         * OnStartClient or OnStartServer, so do not
         * try to subscribe before these events. */
        if (activeIdleCam != null)
            activeIdleCam.enabled = false;
        
        SubscribeToTimeManager(true);

    }
    CameraDampener GetCamInChildren(Transform parent)
    {

        if (parent.TryGetComponent<CameraDampener>(out CameraDampener cam))
            return cam;
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (GetCamInChildren(parent.GetChild(i)) != null)
                {
                    Debug.Log("found " + parent.GetChild(i));
                    return GetCamInChildren(parent.GetChild(i));
                }
            }
            return null;
            //return cam;
        }
    }

    CameraDampener GetMainCam(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if(parent.TryGetComponent<CameraDampener>(out CameraDampener cam))
            {
                return cam;
            }
            else
            {
                GetMainCam(parent.GetChild(i));
                Debug.LogError("no cam in " + parent.GetChild(i).name);
            }
        }

        return null;
    }

    private void GetChildRecursive(GameObject obj)
    {
        if (obj == null)
            return;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            if(child.TryGetComponent<ShipPart>(out ShipPart part))
                shipParts.Add(part);
            GetChildRecursive(child.gameObject);
        }
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if(activeIdleCam!=null)
            activeIdleCam.enabled = true;
        //Instantiate(Camera.main);
       // Camera.main.enabled = true;

    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        /* The TimeManager won't be set until at least
        * OnStartClient or OnStartServer, so do not
        * try to subscribe before these events. */
        SubscribeToTimeManager(true);
    }

    /* OnTick is the equivalent to FixedUpdate. OnTick is called
     * right before physics are updated. */
    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            /* Reconciliation must be done first.
             * This will correct the clients position to as it is
             * on the server and replay cached client inputs.
             * When using reconcile on the client default
             * should be passed in as the data, and false for asServer.
             * This indicates a client-side reconcile. */


            Reconciliation(default, false);
            /* Gather data needed to know how the object is moved. This is used
             * by the server and client. */
            GatherInputs(out MoveData data);
            /* When moving on the client pass in the just gathered data, and
             * false for asServer. This will move the client locally using data.
             * You do not need to pass in the replaying value. */
            Move(data, false);

        }

        if (base.IsServer)
        {
            /* Server has to move the same as client; this helps keep the object in sync.
             * Pass in default for the data, and true for asServer. The server automatically
             * knows which data to use when asServer is true. Like when calling from client,
             * you do not need to set replaying. */
            Move(default, true);

            /* As shown below the reconcile is sent using OnPostTick because you will
             * want to send the objects position, rotation, ect, after the physics have simulated.
             * If you are using a method to move your object that does not rely on physics, such as
             * a character controller or moving the transform directly, you may opt-out of using
             * OnPostTick and send the Reconcile here. */
        }
    }

    /* OnPostTick is after physics have simulated for the tick. */
    private void TimeManager_OnPostTick()
    {
        if (IsServer)
        {
            ReconcileData data = new ReconcileData(transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity, Time.time);
            Reconciliation(data, true);
        }
        ///* Build the reconcile using current data of the object. This is sent to the client, and the
        /// * client will reset using these values. It's EXTREMELY important to send anything that might
        /// * affect the movement, position, and rotation of the object. This includes but is not limited to: 
        /// * transform position and rotation, rigidbody velocities, colliders, ect. 
        /// * 
        /// * Explained further: if you are using prediction on a vehicle that is controlled by wheel colliders, those
        /// * colliders most likely will behave independently of the vehicle root. You must send the colliders position,
        /// * rotation, and any other value that can change from movement or affect movement.
        /// * 
        /// * Another example would be running with stamina. If running depends on stamina you will want to also
        /// * send back stamina along with running state so that the client can adjust their side locally if it differs.
        /// * If stamina somehow existed on the client but not the server then the server would move slower and a desync
        /// * would occur. If you did not send stamina/run state back the client would continue to desync until they also
        /// * ran out of stamina.
        /// * 
        /// * If you are using an asset that uses physics internally there is a fair chance you will need to expose values
        /// * that affect movement or ask the author to make the asset support prediction. */
        ///
        ///* When all data is reset properly the chances of a desync are very low, and near impossible when not using physics.
        /// * Even when a desync does occur it's often incredibly small and will be corrected without any visual disturbances.
        /// * There are some cases however where if a desync is serious enough the client may teleport to the corrected value.
        /// * I've included a component to help reduce any visual jitter during large desyncs. */
        ///* After building the data to send back to the client pass it into the reconcile method,
        ///* while using true for asServer. You should call the reconcile method every tick on both
        ///* the server and client. Fish-Networking internally knows if there is new data to send or not
        ///* and will not waste bandwidth by regularly resending unchanged data. */
    }

    #endregion

    /* GatherInputs takes local inputs of the client and puts them into MoveData.
     * When no inputs are available the method is exited early. Note that data is set
     * to default at the very beginning of the method. You should pass default data into the
     * replicate method. Like when the server sends reconcile, the data will send redundantly to help
     * ensure it goes through, and also will stop sending data that hasn't changed after a few iterations.
     * You are welcome to always fill out data instead of sending default when there is no input
     * but this will cost you bandwidth. */
    private void GatherInputs(out MoveData data)
    {
        //Set to default.
        data = default;

        float thrust = 0;
        float lift = 0;
        float lateral = 0;
        float pitch = 0;
        float roll = 0;
        float yaw = 0;
        bool brake = false;
        bool fire = false;
        bool swapUseAimpoint = false;
        if (!inputManager.menuUp)
        {



            if (inputType == InputType.Joystick)
            {
                thrust = -playerShip.Joystick.Thrust.ReadValue<float>();
                lift = playerShip.Joystick.Lift.ReadValue<float>();
                lateral = playerShip.Joystick.Lateral.ReadValue<float>();
                pitch = -playerShip.Joystick.Pitch.ReadValue<float>();
                roll = -playerShip.Joystick.Roll.ReadValue<float>();
                yaw = playerShip.Joystick.Yaw.ReadValue<float>();
                brake = playerShip.Joystick.Brake.IsInProgress();
                fire = playerShip.Joystick.Fire.IsInProgress();
                swapUseAimpoint = playerShip.Joystick.SwapUseAimpoint.WasPressedThisFrame();
            }
            else if (inputType == InputType.Keyboard)
            {
                thrust = playerShip.Keyboard.Thrust.ReadValue<float>();
                lift = playerShip.Keyboard.Lift.ReadValue<float>();
                lateral = playerShip.Keyboard.Lateral.ReadValue<float>();
                pitch = playerShip.Keyboard.Pitch.ReadValue<float>();
                roll = -playerShip.Keyboard.Roll.ReadValue<float>();
                yaw = playerShip.Keyboard.Yaw.ReadValue<float>();
                brake = playerShip.Keyboard.Brake.IsInProgress();
                fire = playerShip.Keyboard.Fire.IsInProgress();
                swapUseAimpoint = playerShip.Keyboard.SwapUseAimpoint.WasPressedThisFrame();

            }
            else if (inputType == InputType.Mouse)
            {
                thrust = playerShip.Mouse.Thrust.ReadValue<float>();
                lift = playerShip.Mouse.Lift.ReadValue<float>();
                lateral = playerShip.Mouse.Lateral.ReadValue<float>();
                pitch = playerShip.Mouse.Pitch.ReadValue<float>();
                roll = -playerShip.Mouse.Roll.ReadValue<float>();
                yaw = playerShip.Mouse.Yaw.ReadValue<float>();
                brake = playerShip.Mouse.Brake.IsInProgress();
                fire = playerShip.Mouse.Fire.IsInProgress();
                swapUseAimpoint = playerShip.Mouse.SwapUseAimpoint.WasPressedThisFrame();

            }
            else if (inputType == InputType.Gamepad)
            {
                thrust = playerShip.Gamepad.Thrust.ReadValue<float>();
                lift = playerShip.Gamepad.Lift.ReadValue<float>();
                lateral = playerShip.Gamepad.Lateral.ReadValue<float>();
                pitch = playerShip.Gamepad.Pitch.ReadValue<float>();
                roll = -playerShip.Gamepad.Roll.ReadValue<float>();
                yaw = playerShip.Gamepad.Yaw.ReadValue<float>();
                brake = playerShip.Gamepad.Brake.IsInProgress();
                fire = playerShip.Gamepad.Fire.IsInProgress();
                swapUseAimpoint = playerShip.Gamepad.SwapUseAimpoint.WasPressedThisFrame();

            }
        }

        float sensitivityVal = (inputManager.sensitivityValue / 100.0f);

        //Debug.Log(thrust);

        //Be sure to add new control to this. control == off
         if (thrust == 0f && lift == 0f && lateral == 0f && pitch == 0f && roll == 0f && yaw == 0f && !brake && !fire && !swapUseAimpoint)
           return;

        //If there is input then populate data.
        //data = new MoveData(thrust * invertThrust, lift * invertLift, lateral*invertLateral, pitch*-invertPitch, roll*invertRoll, yaw*invertYaw, brake);

        this.fire = fire;

        if (swapUseAimpoint)
        {
            personalizationManager.UpdateUseAimpoint(false);
        }

        if (thrust > 0f)
            thrust *= thrustMultiplier;

        data = new MoveData(
            thrust  * invertThrust  * moveSpeed, 
            lift    * invertLift    * moveSpeed,
            lateral * invertLateral * moveSpeed, 
            -inputManager.curve.Evaluate(pitch) * invertPitch * sensitivityVal,
            inputManager.curve.Evaluate(roll)   * invertRoll  * sensitivityVal,
            inputManager.curve.Evaluate(yaw)    * invertYaw   * sensitivityVal ,
            brake, fire);

    }


    [Replicate]
    private void Move(MoveData data, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        /* You can use asServer to know if the server is calling this method
         * or the client. */

        /* Replaying may be true when as client and when inputs are being replayed.
         * When you call Move replaying is false, as you are
         * manually calling the method. However, when the client reconciles, cached
         * inputs are replayed automatically. This is in part how prediction works.
         * A good example of how you might use the replaying boolean is to show
         * a special effect when jumping.
         * 
         * When replaying is false you are calling the method from your code, and perhaps
         * if input indicates the player is jumping you will want to play audio or a special
         * effect. However, when cached inputs are automatically replayed the same jump
         * input may be called multiple times, but replaying will be true. You can filter
         * out playing the audio/vfx multiple times by not running the logic if replaying
         * is true. */

        

        Vector3 force = new Vector3(data.Lateral, data.Lift, data.Thrust);
        //Make Ship Go
        _rigidbody.AddRelativeForce(force, ForceMode.Force);


        _rigidbody.velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, clampVelocityMagnitude);



        Vector3 torque = new Vector3(data.Pitch, data.Yaw, data.Roll) * rotSpeed;
        //Debug.Log(inputManager.sensitivityValue / 100f);

        _rigidbody.AddRelativeTorque(torque, ForceMode.VelocityChange); ;

        StableizeAll(StableizeX() * stablizeBrake, StableizeY() * stablizeBrake, StableizeZ() * stablizeBrake, gameObject);
        if (data.Brake)
        {
            BrakeAll(BrakeX(), BrakeY(), BrakeZ());
        }


        if (fire)
        {
            fire = false;
            //if (IsClient)
            {
                if (TimeManager.ClientUptime > _nextShootTime)
                {
                    _nextShootTime = TimeManager.ClientUptime + 0.25f;

                    foreach (BlasterV3 blaster in blasters)
                    {
                        if (blaster != null)
                        {
                            blaster.ClientFire(_rigidbody);
                        }
                    }
                }
                //StartServerCooldown();

            }
        }

    }

   


    /* Reconcile is responsible for resetting the clients object using data from
     * the server. You must specify what to reset but Fish-Networking will automatically
     * replay cached data, apply physics per replay, and so on.
     * 
     * With that said, physic simulations are performed with every data replayed. If you have other
     * physics objects in the same physics scenes they will also simulate when this object
     * is replaying datas. You resolve this behavior by putting this object or other objects
     * in their own physics scene. In addition, there are a varity of events and components that can be used
     * to reset other objects during a reconcile as well. These are covered in another video. You may find
     * a link in the description of this video. */
    [Reconcile]
    private void Reconciliation(ReconcileData data, bool asServer, Channel channel = Channel.Unreliable)
    {
        transform.position = data.Position;
        transform.rotation = data.Rotation;
        _rigidbody.velocity = data.Velocity;
        _rigidbody.angularVelocity = data.AngularVelocity;

        //Debug.Log("Reconcile: Position: " + transform.position + " data.position: " + data.Position);
    }


    #region Stablization and brake
    public void BrakeAll(float brakeX, float brakeY, float brakeZ)
    {

        Vector3 locVel = new Vector3(brakeX, brakeY, brakeZ);
        _rigidbody.velocity = transform.TransformDirection(locVel);
    }

    public float BrakeZ()
    {
        //Debug.Log("Z + " + context);
        var locVel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity);

        float blend = MathF.Pow(0.5f, spaceBrakeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.z, blend);
        locVel.z = MovSpeed;
        return locVel.z;
    }
    public float BrakeY()
    {
        //Debug.Log("Z + " + context);
        var locVel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity);

        float blend = MathF.Pow(0.5f, spaceBrakeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.y, blend);
        locVel.y = MovSpeed;
        return locVel.y;
    }
    public float BrakeX()
    {
        //Debug.Log("Z + " + context);
        var locVel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity);

        float blend = MathF.Pow(0.5f, spaceBrakeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.x, blend);
        locVel.x = MovSpeed;
        return locVel.x;
    }



    public void StableizeAll(float stablizeX, float stableizeY, float stableizeZ, GameObject gameObject)
    {
        Vector3 locVel = new Vector3(stablizeX, stableizeY, stableizeZ);
        gameObject.GetComponent<Rigidbody>().angularVelocity = gameObject.transform.TransformDirection(locVel);
    }
    public float StableizeZ()
    {
        var locVel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().angularVelocity);


        float blend = MathF.Pow(0.5f, spaceStablizeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.z, blend);
        locVel.z = MovSpeed;
        return locVel.z;
    }
    public float StableizeY()
    {
        //Debug.Log("Z + " + context);
        var locVel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().angularVelocity);

        float blend = MathF.Pow(0.5f, spaceStablizeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.y, blend);
        locVel.y = MovSpeed;
        return locVel.y;
    }
    public float StableizeX()
    {
        //Debug.Log("Z + " + context);
        var locVel = transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().angularVelocity);

        float blend = MathF.Pow(0.5f, spaceStablizeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.x, blend);
        locVel.x = MovSpeed;
        return locVel.x;
    }
    #endregion
}

