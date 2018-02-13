using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan : MonoBehaviour
{
	public Vector3 upperBound = new Vector3(10, 20, 5);
	public Vector3 lowerBound = new Vector3(0, 0, 1);
	public float sensitivity = .5f;
	public float scrollSpeed = 1;
	public float scrollSensitivity = 1;

	private Vector3 _curPos;
	private Vector3 _sPos;
	private Camera _cam;
	private float _lt;
	private float _zoom;
	// Use this for initialization
	void Start()
	{
		_curPos = transform.position;
		_cam = GetComponent<Camera>();
		_zoom = upperBound.z;
	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Mouse1))
			_sPos = _cam.ScreenToWorldPoint(Input.mousePosition);
		if(Input.GetKey(KeyCode.Mouse1))
		{
			var cPos = _cam.ScreenToWorldPoint(Input.mousePosition);
			var rPos =  cPos - _sPos;
			_curPos -= rPos * sensitivity;
			if (_curPos.x < lowerBound.x)
				_curPos.x = lowerBound.x;
			if (_curPos.y < lowerBound.y)
				_curPos.y = lowerBound.y;
			if (_curPos.x > upperBound.x)
				_curPos.x = upperBound.x;
			if (_curPos.y > upperBound.y)
				_curPos.y = upperBound.y;
			transform.position = _curPos;
		}
		var sY = Input.mouseScrollDelta.y;
		if (sY != 0)
		{
			_zoom -= sY;
			_lt = 0;
		}
		if (_zoom < lowerBound.z)
			_zoom = lowerBound.z;
		if (_zoom > upperBound.z)
			_zoom = upperBound.z;
		_cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _zoom, _lt += scrollSpeed * Time.deltaTime);
	}
}