using System.Collections;
using TMPro;
using UnityEngine;

public class SlotMachineActions : MonoBehaviour
{
	[SerializeField] private Animator[] flareAnimators;
	[SerializeField] private Animator winAnimator;
	[SerializeField] TMP_Text winText;
	[SerializeField] private JackpotEffect jackpotEffect;

	[Header("Events")]
	[SerializeField] private EventChannel onStartGame;
	[SerializeField] private EventChannel onStopGame;
	[SerializeField] private EventChannel onSpinStart;
	[SerializeField] private IntEventChannel onSpinResult;
	[SerializeField] private EventChannel onSpinResultDone;
	[SerializeField] private IntEventChannel onUpdateBet;

	[Header("Audio")]
	[SerializeField] private AudioClipEvent onAudioEvent;
	[SerializeField] private AudioClip winLowAudioClip;
	[SerializeField] private AudioClip winMediumAudioClip;
	[SerializeField] private AudioClip winHighAudioClip;
	[SerializeField] private AudioClip[] musicAudioClips;
	[SerializeField] private float minAttractMusicTime = 10;
	[SerializeField] private float minMusicTime = 10;

	Coroutine flaresCoroutine;
	float playMusicTime = 0;
	float nextPlayMusicTime = 0;
	AudioClip musicAudioClip = null;
	bool playingGame = false;
	private int currentBet = 0;

	void Start()
	{
		onStartGame.Subscribe(OnStartGame);
		onStopGame.Subscribe(OnStopGame);
		onSpinStart.Subscribe(OnSpinStart);
		onSpinResult.Subscribe(OnSpinResults);
		onUpdateBet.Subscribe(OnUpdateBet);

		nextPlayMusicTime = Time.time + Random.Range(minAttractMusicTime, minAttractMusicTime * 1.5f);
	}
		
	void Update()
	{
		// play music
		if (musicAudioClip == null && Time.time > nextPlayMusicTime)
		{
			musicAudioClip = musicAudioClips[Random.Range(0, musicAudioClips.Length)];
			playMusicTime = Time.time + musicAudioClip.length + 1;
			StartCoroutine(PlaySoundClipDelay(1));
		}

		if (musicAudioClip != null && Time.time > playMusicTime)
		{
			if (playingGame)
			{
				nextPlayMusicTime = Time.time + Random.Range(minMusicTime, minMusicTime * 1.5f);
			}
			else
			{
				nextPlayMusicTime = Time.time + Random.Range(minAttractMusicTime, minAttractMusicTime * 1.5f);
			}

			musicAudioClip = null;
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

	void OnStartGame()
	{
		playingGame = true;
		if (musicAudioClip == null)
		{
			musicAudioClip = musicAudioClips[Random.Range(0, musicAudioClips.Length)];
			playMusicTime = Time.time + musicAudioClip.length + 1;
			StartCoroutine(PlaySoundClipDelay(1));
		}
	}

	void OnStopGame()
	{
		playingGame = false;
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


	void OnUpdateBet(int newBet)
	{
		currentBet = newBet;
	}

	IEnumerator PlaySoundClipDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		onAudioEvent.OnPlayEvent(musicAudioClip);		
	}

	IEnumerator WinSequence(int result)
	{
		float waitTime = 1;
		float relativeResult = result / (float)currentBet;
		if (relativeResult >= 0.2f && relativeResult < 2.0f)
		{
			onAudioEvent.OnPlayEvent(winLowAudioClip);
		}
		if (relativeResult >= 2.0f && relativeResult < 5.0f)
		{
			winText.text = "BIG WIN!";
			winAnimator.SetTrigger("Start");

			if (flaresCoroutine != null)
			{
				StopCoroutine(flaresCoroutine);
				flaresCoroutine = null;
			}
			flaresCoroutine = StartCoroutine(WinFlares(0.1f, 3));
			onAudioEvent.OnPlayEvent(winMediumAudioClip);
			waitTime += 2;
		}
		if (relativeResult >= 5.0f)
		{
			winText.text = "MASSIVE WIN!";
			winAnimator.SetTrigger("Start");

			if (flaresCoroutine != null)
			{
				StopCoroutine(flaresCoroutine);
				flaresCoroutine = null;
			}
			flaresCoroutine = StartCoroutine(WinFlares(0.1f, 5));
			onAudioEvent.OnPlayEvent(winHighAudioClip);
			jackpotEffect.BeginJackpotEffect();
			waitTime += 3;
		}
		
		yield return new WaitForSeconds(waitTime);

		if (relativeResult >= 2.0f)	winAnimator.SetTrigger("Done");
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
