using UnityEngine;

public class RectNoise : MonoBehaviour
{
	[SerializeField] private float rate;
	[SerializeField] private float amplitude;

	RectTransform rectTransform;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		float x = Mathf.PerlinNoise(Time.time * rate, 0);
		float y = Mathf.PerlinNoise(0, Time.time * rate);

		rectTransform.anchoredPosition = new Vector3(
			Mathf.Lerp(-amplitude, amplitude, x),
			Mathf.Lerp(-amplitude, amplitude, y),
			rectTransform.localPosition.z
		);
	}
}
