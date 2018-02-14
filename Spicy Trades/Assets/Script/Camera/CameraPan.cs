using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan : MonoBehaviour
{
	public int minZoom = 1, maxZoom = 10;
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
		_zoom = maxZoom;
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
			if (_curPos.x < 0)
				_curPos.x = 0;
			if (_curPos.y < 0)
				_curPos.y = 0;
			if (_curPos.x > MapRenderer.Map.generator.Size.x)
				_curPos.x = MapRenderer.Map.generator.Size.x;
			if (_curPos.y > MapRenderer.Map.generator.Size.y)
				_curPos.y = MapRenderer.Map.generator.Size.y;
			transform.position = _curPos;
		}
		var sY = Input.mouseScrollDelta.y;
		if (sY != 0)
		{
			_zoom -= sY;
			_lt = 0;
		}
		if (_zoom < minZoom)
			_zoom = minZoom;
		if (_zoom > maxZoom)
			_zoom = maxZoom;
		_cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _zoom, _lt += scrollSpeed * Time.deltaTime);
	}
}