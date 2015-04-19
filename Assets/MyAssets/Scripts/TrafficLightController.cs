using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour {

    // Left or right road, (values 0 and 1)
    public int type = 0;

    // The trigger that is stopping the vehicles
    public Collider stopTrigger;

    // Boolean for light
    public bool redLightOn = false;

    // Lights
    public Renderer lightGreen;
    public Renderer lightRed;
    public GameObject haloRed;
    public GameObject haloGreen;
    public Color redLightColor;
    public Color greenLightColor;
    public Color offLightColor;

    public enum LightType { Green, Red };
	


	void Start () {

	}
	


	void Update () {

        

        if (type == 0)
        {
            if (Input.GetKey("a"))
            {
                SetLightOn(LightType.Green);
            }
            else
            {
                SetLightOn(LightType.Red);
            }
        }
        else
        {
            if (Input.GetKey("s"))
            {
                SetLightOn(LightType.Green);
            }
            else
            {
                SetLightOn(LightType.Red);
            }
        }
           
            

	}


    public void SetLightOn(LightType lightType)
    {

        if (lightType == LightType.Green)
        {
            // Set green on
            lightGreen.material.color = greenLightColor;
            (haloGreen.GetComponent("Halo") as Behaviour).enabled = true;

            // Set red off
            lightRed.material.color = offLightColor;
            (haloRed.GetComponent("Halo") as Behaviour).enabled = false;

            redLightOn = false;


        }
        else if (lightType == LightType.Red)
        {
            // Set red on
            lightRed.material.color = redLightColor;
            (haloRed.GetComponent("Halo") as Behaviour).enabled = true;

            // Set green on
            lightGreen.material.color = offLightColor;
            (haloGreen.GetComponent("Halo") as Behaviour).enabled = false;

            redLightOn = true;
        }
    }

    
}
