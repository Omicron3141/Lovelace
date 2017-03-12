using UnityEngine;
using System.Collections;

public class RandomTrackingZRotation : MonoBehaviour {

	float traveltime = 0;
	float timeUntilChange = 0;
	Quaternion newRotation;
	Quaternion orgRotation;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (timeUntilChange < 0) {
			float newZ = Random.Range (0, 360);
			Vector3 newEuler = transform.rotation.eulerAngles;
			newEuler.z = newZ;
			newRotation = Quaternion.Euler (newEuler);
			orgRotation = transform.rotation;
			timeUntilChange = Mathf.Abs(newZ - transform.rotation.eulerAngles.z);
			while (timeUntilChange > 180) {
				timeUntilChange -= 180;
			}

			timeUntilChange /= 45;
			traveltime = timeUntilChange;
		} else {
			timeUntilChange -= Time.deltaTime;
			transform.rotation = Quaternion.Slerp (orgRotation, newRotation, (traveltime - timeUntilChange) / traveltime);
		}
			
	}
}
