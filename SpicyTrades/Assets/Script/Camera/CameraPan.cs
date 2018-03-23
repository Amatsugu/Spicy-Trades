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
	private Vector2 _lastTouchDelta;
	// Use this for initialization
	void Start()
	{
		_curPos = transform.position;
		_curPos.x = Mathf.Lerp(0, GameMaster.Generator.Size.x, .5f);
		_curPos.y = Mathf.Lerp(0, GameMaster.Generator.Size.y, .25f);
		_cam = GetComponent<Camera>();
		_zoom = Mathf.Lerp(minZoom, maxZoom, .5f);
	}

	// Update is called once per frame
	void Update()
	{
		var zoomDamping = Mathf.Max(.1f, (_zoom - minZoom) / (maxZoom - minZoom));
		var touches = Input.touches;
		if (touches.Length == 1)
		{
			var dPos = touches[0].deltaPosition;
			_curPos -= new Vector3(dPos.x, dPos.y, 0) * Time.deltaTime * zoomDamping;
		}else if(touches.Length == 2)
		{
			Vector2 touchZeroPrevPos = touches[0].position - touches[0].deltaPosition;
			Vector2 touchOnePrevPos = touches[1].position - touches[1].deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touches[0].position - touches[1].position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			_zoom += deltaMagnitudeDiff * Time.deltaTime * scrollSensitivity;
		}
#if !EDITOR
		var mPos = Input.mousePosition;
		mPos.z = _cam.nearClipPlane;
		if (Input.GetKeyDown(KeyCode.Mouse1))
			_sPos = mPos;
		if (Input.GetKey(KeyCode.Mouse1))
		{
			var cPos = mPos;
			var rPos = cPos - _sPos;
			_sPos = cPos;
			_curPos -= rPos * sensitivity * Time.deltaTime * zoomDamping;
		}
		if (Input.GetKey(KeyCode.W))
			_curPos.y += 5 * sensitivity * Time.deltaTime;
		else if (Input.GetKey(KeyCode.S))
			_curPos.y -= 5 * sensitivity * Time.deltaTime;
		if (Input.GetKey(KeyCode.D))
			_curPos.x += 5 * sensitivity * Time.deltaTime;
		else if (Input.GetKey(KeyCode.A))
			_curPos.x -= 5 * sensitivity * Time.deltaTime;
		var sY = Input.mouseScrollDelta.y;
		if (sY != 0)
		{
			_zoom -= sY;
			_lt = 0;
		}
#endif
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