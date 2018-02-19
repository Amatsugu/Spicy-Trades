using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPan : MonoBehaviour
{
	public float minAngle = 20, maxAngle = 90;
	public float sensitivity = .5f;
	public int minZoom = 1, maxZoom = 10;
	public float scrollSpeed = 1;
	public float scrollSensitivity = 1;
	public AnimationCurve zoomRotationCurve = new AnimationCurve();

	private Vector3 _curPos;
	private Vector3 _sPos;
	private Camera _cam;
	private float _lt;
	private float _zoom;
	// Use this for initialization
	void Start()
	{
		_curPos = transform.position;
		_curPos.x = Mathf.Lerp(0, MapRenderer.Map.generator.Size.x, .5f);
		_curPos.y = Mathf.Lerp(0, MapRenderer.Map.generator.Size.y, .25f);
		_cam = GetComponent<Camera>();
		_zoom = Mathf.Lerp(minZoom, maxZoom, .5f);
	}

	// Update is called once per frame
	void Update()
	{
		var mPos = Input.mousePosition;
		mPos.z = _cam.nearClipPlane;
		if (Input.GetKeyDown(KeyCode.Mouse1))
			_sPos = _cam.ScreenToWorldPoint(mPos);
		if(Input.GetKey(KeyCode.Mouse1))
		{
			var cPos = _cam.ScreenToWorldPoint(mPos);
			var r = _cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(r, out hit, 100))
			{

			Debug.DrawRay(r.origin, r.direction * 10, Color.red);
			Debug.DrawLine(r.origin, hit.point, Color.cyan);
			}
			Debug.Log(Input.mousePosition);
			var rPos = cPos - _sPos;
			_curPos -= rPos * sensitivity;
			/*if (_curPos.x < 0)
				_curPos.x = 0;
			if (_curPos.y < 0)
				_curPos.y = 0;
			if (_curPos.x > MapRenderer.Map.generator.Size.x)
				_curPos.x = MapRenderer.Map.generator.Size.x;
			if (_curPos.y > MapRenderer.Map.generator.Size.y)
				_curPos.y = MapRenderer.Map.generator.Size.y;*/
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
		_curPos.z = Mathf.Lerp(_curPos.z, -_zoom, _lt);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Mathf.Lerp(minAngle, maxAngle, zoomRotationCurve.Evaluate((_zoom - minZoom) / (maxZoom - minZoom))), 0, 0), _lt += scrollSpeed * Time.deltaTime);
		transform.position = _curPos;
		//_cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _zoom, _lt += scrollSpeed * Time.deltaTime);
	}
}