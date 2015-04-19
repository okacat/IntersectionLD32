using UnityEngine;
using System.Collections;

public class VehicleController : MonoBehaviour {

    // Enumerators
    public enum TriggerType { Stop, Front };
    public enum VehicleType { Car = 0, Truck = 1 };
    public enum CarType { Car = 0, Truck = 1 };

    // Type of this vehicle
    public VehicleType vehicleType;
    public bool isTarget = false;       // Is this one of the targets

    // Physics parameters
    public float maxVelocity = 8.0f;
    private float maxAcceleration = 5.0f;
    private float maxDeacceleration;
    private float slowDeacceleration = -1.0f;
    private float slowVelocity = 1.0f;
    private float currentVelocity;

    // Information about the car's surroundings
    private VehicleController vehicleInFront;
    private bool brakeHard = false;
    private TrafficLightController hitTrafficLightController = null;
    private bool stopOnLight = false;

    // References to all the model types
    public GameObject carModel;
    public GameObject truckModel;
    
    // Material of the target vehicles
    public Material targetMaterial;

    private Rigidbody rigidbody;

    public GameObject explosionPrefab;

    private bool isQuitting = false;

    // Array of possible vehicle colors
    public Color[] vehicleColors;
    
    
	void Start () {

        // Setup some of the physics parameters
        maxDeacceleration = maxAcceleration * -3.0f;

        // Get the reference to this vehicle's rigidbody
        rigidbody = this.gameObject.GetComponent<Rigidbody>();

        // Set the velocity
        currentVelocity = maxVelocity;

        // Set the appropriate model and material
        int k = Mathf.RoundToInt(Random.Range(0, vehicleColors.Length ));
        if (vehicleType == VehicleType.Car)
        {
            carModel.SetActive(true);
            truckModel.SetActive(false);

            Renderer carRenderer = carModel.transform.Find("carRegular/CarBody").GetComponent<Renderer>();
            if (isTarget)
            {
                carRenderer.material = targetMaterial;
            }
            else
            {
                carRenderer.material.color = vehicleColors[k];
            }
        }
        else if (vehicleType == VehicleType.Truck)
        {
            truckModel.SetActive(true);
            carModel.SetActive(false);

            Renderer truckRenderer = truckModel.transform.Find("truck/TruckBody").GetComponent<Renderer>();
            Renderer trailerRenderer = truckModel.transform.Find("truck/TruckTrailer").GetComponent<Renderer>();
            if (isTarget)
            {
                
                truckRenderer.material = targetMaterial;
                trailerRenderer.material = targetMaterial;
            }
            else
            {
                
                truckRenderer.material.color = vehicleColors[k];
            }
        }



	}
	
	void Update () {

        // Should stop soon
        if (hitTrafficLightController != null && hitTrafficLightController.redLightOn)
        {
            
            if (stopOnLight)
            {
                currentVelocity = ApproachVelocity(0.0f);
            }
            else
            {
                currentVelocity = ApproachVelocity(slowVelocity);
            }
            
        }
        // Drive
        else
        {
            if (vehicleInFront == null)
            {
                currentVelocity += maxAcceleration * Time.deltaTime;
                currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);
            }
            else
            {
      
                // Slow down
                currentVelocity = ApproachVelocity(vehicleInFront.currentVelocity);
            }
        }

        // Move the vehicle accordingly to the velocity
        this.transform.Translate(currentVelocity * Time.deltaTime, 0.0f, 0.0f);
        
	}

    float ApproachVelocity(float targetVelocity)
    {
        float vel = 0.0f;
        float deaccelerate = slowDeacceleration;
        
        if (brakeHard || Mathf.Abs(currentVelocity - targetVelocity) > 1.0f) deaccelerate = maxDeacceleration;

        if (currentVelocity < targetVelocity)
        {
            vel = currentVelocity + maxAcceleration * Time.deltaTime;
        }
        else
        {
            vel = currentVelocity + deaccelerate * Time.deltaTime;
        }

        //vel = Mathf.Clamp(vel, -maxVelocity, maxVelocity);
        vel = Mathf.Clamp(vel, 0.0f, maxVelocity);

        return vel;
    }

    // A collider has entered the front trigger
    public void TriggerEntered(Collider collider, TriggerType triggerType)
    {

        // Is something right in front of the vehicle?
        if (triggerType == TriggerType.Stop)
        {
            brakeHard = true;
        }

        // Was it a vehicle?
        if (collider.gameObject.transform.parent.gameObject.CompareTag(this.gameObject.tag))
        {
            vehicleInFront = collider.gameObject.transform.parent.gameObject.GetComponent<VehicleController>(); 

        }

        // Was it a stop light trigger?
        if (collider.CompareTag("StopTrigger"))
        {
            // Should the car really stop now?
            if (triggerType == TriggerType.Stop)
            {
                stopOnLight = true;
                //Debug.Log("I really should stop now");
            }
            hitTrafficLightController = collider.transform.parent.gameObject.GetComponent<TrafficLightController>();
            //Debug.Log("Hit a traffic light: " + hitTrafficLightController);
        }
        
    }


    // A collider has exited the front trigger
    public void TriggerExited(Collider collider, TriggerType triggerType)
    {

        // Something exited right in front of the vehicle?
        if (triggerType == TriggerType.Stop)
        {
            brakeHard = true;
        }

        // Was it a vehicle?
        if (collider.gameObject.transform.parent.gameObject.CompareTag(this.gameObject.tag))
        {
         
            if (vehicleInFront != null)
            {
                if (vehicleInFront.Equals(collider.gameObject.GetComponent<VehicleController>()))
                {
                    vehicleInFront = null;
                }
            }
        }

        // Was it a trigger to stop?
        if (collider.CompareTag("StopTrigger"))
        {
            stopOnLight = false;
            hitTrafficLightController = null;
        }
       
    }

    // Car's body trigger collided with another
    public void CollidedWithVehicle(Collider collider)
    {
        // Don't do collisions if the game is not running
        if (UIManager.Instance.gameState != UIManager.GameState.Play)
            return;

        if (collider.CompareTag("VehicleBodyTrigger"))
        {
            
            // Look at this reference chain and cringe
            VehicleController otherController = collider.transform.parent.transform.parent.gameObject.GetComponent<VehicleController>();

            /* 
             * Check to see who get's destroyed
             * Truck - Car      : Car destroyed
             * Car - Car        : Both destroyed
             * Truck - Truck    : Both destroyed */

            if (this.vehicleType == VehicleType.Car && otherController.vehicleType == VehicleType.Truck ) 
            {
                if (this.isTarget)
                {
                    UIManager.Instance.AddToScore(UIManager.scoreTargetCar);
                }
                else
                {
                    UIManager.Instance.AddToScore(UIManager.penaltyCar);
                }
                Destroy(this.gameObject);
                Explode();
            }
            else if (this.vehicleType == VehicleType.Car && otherController.vehicleType == VehicleType.Car)
            {
                if (this.isTarget)
                {
                    UIManager.Instance.AddToScore(UIManager.scoreTargetCar);
                }
                else
                {
                    UIManager.Instance.AddToScore(UIManager.penaltyCar);
                }

                Destroy(this.gameObject);
                Explode();
                //Destroy(collider.transform.parent.gameObject);

            }
            else if(this.vehicleType == VehicleType.Truck && otherController.vehicleType == VehicleType.Truck)
            {
                if (this.isTarget)
                {
                    UIManager.Instance.AddToScore(UIManager.scoreTargetTruck);
                }
                else
                {
                    UIManager.Instance.AddToScore(UIManager.penaltyTruck);
                }

                Destroy(this.gameObject);
                Explode();                
            }

            
            
        }
    }

    void OnDestroy()
    {
        if (isQuitting == false)
        {
        }
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void Explode()
    {
        Instantiate(explosionPrefab, this.transform.position, Quaternion.Euler(0, 0, 0));
        MyAudioManager.Instance.PlayExplode();
    }




}
