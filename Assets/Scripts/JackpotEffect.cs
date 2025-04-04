using System.Collections;
using UnityEngine;

public class JackpotEffect : MonoBehaviour
{
	[SerializeField] private GameObject jackpotEffectPrefab; // Prefab for the jackpot effect
	[SerializeField] private float minForce = 1;
	[SerializeField] private float maxForce = 1;
	[SerializeField] private float maxAngle = 1;    // Random angle for the force direction
	[SerializeField] private float maxDistance = 1; // Maximum distance from the original position
	[SerializeField] private float lifetime = 1;    // Lifetime of the effect prefab
	[SerializeField] private float rate = 1;        // Time between effect spawns
	[SerializeField] private float duration = 5;    // Total duration of the entire effect sequence

	private Coroutine jackpotCoroutine;

	void Start()
	{
		//jackpotCoroutine = StartCoroutine(StartJackpotEffect(transform.position));
	}

	// You can also call this method from other scripts to manually start the effect
	public void BeginJackpotEffect()
	{
		jackpotCoroutine = StartCoroutine(StartJackpotEffect(transform.position));
	}


	public void BeginJackpotEffect(Vector3 position)
	{
		// Stop any existing coroutine to prevent multiple instances
		if (jackpotCoroutine != null)
		{
			StopCoroutine(jackpotCoroutine);
		}

		jackpotCoroutine = StartCoroutine(StartJackpotEffect(position));
	}

	IEnumerator StartJackpotEffect(Vector3 position)
	{
		float elapsedTime = 0f;

		// Continue spawning effects until the total duration has elapsed
		while (elapsedTime < duration)
		{
			// Call the PlayJackpotEffect method to instantiate the effect
			PlayJackpotEffect(position);

			// Wait for the specified rate before spawning the next effect
			yield return new WaitForSeconds(rate);

			// Update the elapsed time
			elapsedTime += rate;
		}

		// Coroutine naturally ends after the duration expires
		jackpotCoroutine = null;
	}

	public void PlayJackpotEffect(Vector3 position)
	{
		// Instantiate the jackpot effect prefab at the specified position
		Vector3 offset = Vector3.right * Random.Range(-maxDistance, maxDistance);
		GameObject effect = Instantiate(jackpotEffectPrefab, position + offset, Quaternion.identity);

		// Get the Rigidbody component of the instantiated effect
		Rigidbody rb = effect.GetComponent<Rigidbody>();

		Vector3 force = Vector3.up * Random.Range(minForce, maxForce);

		Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(-maxAngle, maxAngle));
		force = rotation * force; // Apply a random rotation to the force vector

		// Apply a random force to the effect
		rb.AddForce(force, ForceMode.Impulse);

		// Destroy each individual effect instance after a certain duration
		// This is different from the total sequence duration
		Destroy(effect, lifetime); // You might want to make this a separate parameter
	}

	// Optional: Add method to stop the effect early if needed
	public void StopJackpotEffect()
	{
		if (jackpotCoroutine != null)
		{
			StopCoroutine(jackpotCoroutine);
			jackpotCoroutine = null;
		}
	}

	// Optional: Cancel the effect when the object is destroyed
	private void OnDestroy()
	{
		if (jackpotCoroutine != null)
		{
			StopCoroutine(jackpotCoroutine);
		}
	}
}