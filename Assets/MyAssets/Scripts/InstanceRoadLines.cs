using UnityEngine;
using System.Collections;

public class InstanceRoadLines : MonoBehaviour {

    public GameObject lineModel;
    public float spacing;
    public int numberOfLines = 15;
    private Vector3 firstPosition;
    public int instantationAxis;

	// Use this for initialization
	void Start () {
        firstPosition = lineModel.transform.position;

        

        for (int i = 1; i < numberOfLines; i++)
        {
            GameObject newLine = (GameObject)Instantiate(lineModel);
            newLine.transform.parent = this.gameObject.transform.parent;
            newLine.transform.localPosition = lineModel.transform.localPosition;

            if (instantationAxis == 0)
                newLine.transform.Translate(spacing * i, 0, 0);
            else
            {
                newLine.transform.Translate(0, 0, spacing * i);
                newLine.transform.Rotate(0, 90, 0);
            }

            newLine.transform.localScale = lineModel.transform.localScale;
        }

        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject newLine = (GameObject)Instantiate(lineModel);
            newLine.transform.parent = this.gameObject.transform.parent;
            newLine.transform.localPosition = new Vector3(lineModel.transform.localPosition.x * -1, lineModel.transform.localPosition.y, lineModel.transform.localPosition.z);
            if (instantationAxis == 0)
                newLine.transform.Translate(spacing * -i, 0, 0);
            else
            {
                newLine.transform.Translate(0, 0, spacing * -i);
                newLine.transform.Rotate(0,90,0);
            }
            newLine.transform.localScale = lineModel.transform.localScale;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
