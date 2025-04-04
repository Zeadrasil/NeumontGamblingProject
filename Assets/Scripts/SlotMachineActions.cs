using System.Collections;
using UnityEngine;

public class SlotMachineActions : MonoBehaviour
{
	[SerializeField] private Animator[] flareAnimators;
	[SerializeField] private JackpotEffect jackpotEffect;

	[Header("Events")]
	[SerializeField] private EventChannel onSpinStart;
	[SerializeField] private IntEventChannel onSpinResult;
	[SerializeField] private EventChannel onSpinResultDone;

	[Header("Audio")]
	[SerializeField] private AudioClipEvent onAudioEvent;
	[SerializeField] private AudioClip winLowAudioClip;
	[SerializeField] private AudioClip winMediumAudioClip;
	[SerializeField] private AudioClip winHighAudioClip;
	[SerializeField] private AudioClip[] musicAudioClips;
	[SerializeField] private float minMusicTime = 10;

	Coroutine flaresCoroutine;
	float playMusicTime;

	void Start()
	{
		onSpinStart.Subscribe(OnSpinStart);
		onSpinResult.Subscribe(OnSpinResults);

		playMusicTime = Time.time + Random.Range(minMusicTime, minMusicTime * 1.5f);
	}
		
	void Update()
	{
		// play music
		if (Time.time > playMusicTime)
		{
			AudioClip musicAudioClip = musicAudioClips[Random.Range(0, musicAudioClips.Length)];
			onAudioEvent.OnPlayEvent(musicAudioClip);
			playMusicTime = Time.time + musicAudioClip.length + Random.Range(minMusicTime, minMusicTime * 1.5f);
		}

		// play random light flares
		if (flaresCoroutine == null)
		{
			switch (Random.Range(0, 3))
			{
				case 0:
					flaresCoroutine = StartCoroutine(StepFlares(0.5f, 3, Random.Range(5, 8)));
					break;
				case 1:
					flaresCoroutine = StartCoroutine(RoundFlares(0.1f, Random.Range(5, 8)));
					break;
				case 2:
					flaresCoroutine = StartCoroutine(RandomFlares(0.2f, 0.4f, 2, Random.Range(5, 8)));
					break;
			}
		}
	}

	void OnSpinStart()
	{
		if (flaresCoroutine != null)
		{
			StopCoroutine(flaresCoroutine);
			flaresCoroutine = null;
		}
		flaresCoroutine = StartCoroutine(RoundFlares(0.1f, 5));
	}

	void OnSpinResults(int result)
	{
		StartCoroutine(WinSequence(result));
	}

	IEnumerator WinSequence(int result)
	{
		float waitTime = 2;
		if (result > 0 && result < 10)
		{
			onAudioEvent.OnPlayEvent(winLowAudioClip);
		}
		if (result >= 10)
		{
			if (flaresCoroutine != null)
			{
				StopCoroutine(flaresCoroutine);
				flaresCoroutine = null;
			}
			flaresCoroutine = StartCoroutine(WinFlares(0.1f, 3));
			onAudioEvent.OnPlayEvent(winMediumAudioClip);
			waitTime += 2;
		}
		if (result >= 30)
		{
			if (flaresCoroutine != null)
			{
				StopCoroutine(flaresCoroutine);
				flaresCoroutine = null;
			}
			flaresCoroutine = StartCoroutine(WinFlares(0.1f, 5));
			jackpotEffect.BeginJackpotEffect();
			onAudioEvent.OnPlayEvent(winHighAudioClip);
			waitTime += 4;
		}

		yield return new WaitForSeconds(waitTime);

		onSpinResultDone.RaiseEvent();
	}

	IEnumerator RandomFlares(float minTime, float maxTime, int numFlares, float duration)
	{
		float time = Time.time + duration;
		while (Time.time < time)
		{
			for (int i = 0; i < numFlares; i++)
			{
				int randomIndex = Random.Range(0, flareAnimators.Length);
				flareAnimators[randomIndex].SetTrigger("Flare");
			}
			yield return new WaitForSeconds(Random.Range(minTime, maxTime));
		}

		StopCoroutine(flaresCoroutine);
		flaresCoroutine = null;
	}

	IEnumerator StepFlares(float stepTime, int steps, float duration)
	{
		int index = 0;
		float time = Time.time + duration;
		while (Time.time < time)

		{
			index++;
			index = index % steps;
			for (int i = index; i < flareAnimators.Length; i += steps)
			{
				flareAnimators[i].SetTrigger("Flare");
			}
			yield return new WaitForSeconds(stepTime);
		}

		StopCoroutine(flaresCoroutine);
		flaresCoroutine = null;
	}

	IEnumerator RoundFlares(float stepTime, float duration)
	{
		int index = 0;
		float time = Time.time + duration;
		while (Time.time < time)
		{
			index++;
			index = index % flareAnimators.Length;
			flareAnimators[index].SetTrigger("Flare");
			yield return new WaitForSeconds(stepTime);
		}

		StopCoroutine(flaresCoroutine);
		flaresCoroutine = null;
	}

	IEnumerator WinFlares(float stepTime, float duration)
	{
		int index = -1;
		float time = Time.time + duration;
		while (Time.time < time)
		{
			index = ++index % (flareAnimators.Length / 2);

			flareAnimators[index].SetTrigger("Flare");
			flareAnimators[(flareAnimators.Length-1) - index].SetTrigger("Flare");
			yield return new WaitForSeconds(stepTime);
		}

		StopCoroutine(flaresCoroutine);
		flaresCoroutine = null;
	}
}
