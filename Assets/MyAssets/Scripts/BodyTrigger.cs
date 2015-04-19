using UnityEngine;
using System.Collections;

public class BodyTrigger : MonoBehaviour {

    public VehicleController vehicleController;

    void Start()
    {
        // Get the reference to the controller of this vehicle
        //vehicleController = this.transform.parent.transform.parent.GetComponent<VehicleController>();

    }

    void OnTriggerEnter(Collider collider)
    {
        vehicleController.CollidedWithVehicle(collider);
    }

    /*
    void OnTriggerExit(Collider collider)
    {
        vehicleController.TriggerExited(collider, VehicleController.TriggerType.Front);
    }
    */
}
