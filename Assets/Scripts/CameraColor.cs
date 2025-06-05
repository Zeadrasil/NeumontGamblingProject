using UnityEngine;

public class CameraColor : MonoBehaviour
{
	[SerializeField] Camera _camera;
	[SerializeField] float rate = 1f;

	void Update()
	{
		_camera.backgroundColor = Color.HSVToRGB(
			Mathf.PingPong(Time.time * rate, 1),
			1,  // Saturation (keep at 1 for vibrant colors)
			1   // Value/Brightness (keep at 1 for bright colors)
		);
	}
}
