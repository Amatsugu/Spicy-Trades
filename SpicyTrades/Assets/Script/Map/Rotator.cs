using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float speed;

	private float curAngle;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.localRotation = Quaternion.Euler(0, 0, curAngle);
		curAngle += Time.deltaTime * speed;
		if (curAngle >= 360)
			curAngle -= curAngle;
	}
}
