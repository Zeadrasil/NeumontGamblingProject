using System.Collections;
using UnityEngine;

public class SlotMachineActions : MonoBehaviour
{
	[SerializeField] Animator[] flareAnimators;

	Coroutine flaresCoroutine;
	void Start()
	{
		flaresCoroutine = StartCoroutine(RandomFlares(0.2f, 0.4f, 2));
		//flaresCoroutine = StartCoroutine(StepFlares(0.5f, 3));
		//flaresCoroutine = StartCoroutine(RoundFlares(0.1f));
		//flaresCoroutine = StartCoroutine(WinFlares(0.1f));
	}
		
	void Update()
	{
		
	}

	IEnumerator RandomFlares(float minTime, float maxTime, int numFlares)
	{
		while (true)
		{
			for (int i = 0; i < numFlares; i++)
			{
				int randomIndex = Random.Range(0, flareAnimators.Length);
				flareAnimators[randomIndex].SetTrigger("Flare");
			}
			yield return new WaitForSeconds(Random.Range(minTime, maxTime));
		}
	}

	IEnumerator StepFlares(float time, int steps)
	{
		int index = 0;
		while (true)
		{
			index++;
			index = index % steps;
			for (int i = index; i < flareAnimators.Length; i += steps)
			{
				flareAnimators[i].SetTrigger("Flare");
			}
			yield return new WaitForSeconds(time);
		}
	}

	IEnumerator RoundFlares(float time)
	{
		int index = 0;
		while (true)
		{
			index++;
			index = index % flareAnimators.Length;
			flareAnimators[index].SetTrigger("Flare");
			yield return new WaitForSeconds(time);
		}
	}

	IEnumerator WinFlares(float time)
	{
		int index = -1;
		while (true)
		{
			index = ++index % (flareAnimators.Length / 2);

			flareAnimators[index].SetTrigger("Flare");
			flareAnimators[(flareAnimators.Length-1) - index].SetTrigger("Flare");
			yield return new WaitForSeconds(time);
		}
	}
}
