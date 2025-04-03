using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static AudioCueData;

public class AudioManager : Singleton<AudioManager>
{
	// Reference to the AudioMixer in Unity, which controls various audio groups.
	[SerializeField] AudioMixer audioMixer;

	// Serialized FloatData variables to store and manage volume levels.
	[SerializeField] FloatData masterVolume;
	[SerializeField] FloatData sfxVolume;
	[SerializeField] FloatData musicVolume;

	// Event triggered when audio settings change.
	[SerializeField] Event onAudioChange;
	[SerializeField] AudioCueEvent onPlayAudioCueEvent;
	[SerializeField] AudioClipEvent onPlayAudioClipEvent;
	[SerializeField] AudioClipEvent onPlay3DAudioClipEvent;

	[SerializeField] AudioConfigurationData audioConfigurationData;
	[SerializeField] AudioConfigurationData audio3DConfigurationData;

	private List<AudioEmitter> audioEmitters = new List<AudioEmitter>();

	// Constants for storing volume settings in PlayerPrefs.
	const string MASTER_VOLUME = "MasterVolume";
	const string SFX_VOLUME = "SFXVolume";
	const string MUSIC_VOLUME = "MusicVolume";

	void Start()
	{
		// Subscribes to the event to update audio settings when triggered.
		//onAudioChange?.Subscribe(OnAudioChange);

		onPlayAudioCueEvent.OnAudioCuePlay += OnPlayAudioCue;
		//onPlayAudioClipEvent?.Subscribe(OnPlayAudioClip);
		//onPlay3DAudioClipEvent?.Subscribe(OnPlayAudio3DClip);

		// Loads saved volume settings from PlayerPrefs or defaults to 1.
		masterVolume.Value = PlayerPrefs.GetFloat(MASTER_VOLUME, 1);
		sfxVolume.Value = PlayerPrefs.GetFloat(SFX_VOLUME, 1);
		musicVolume.Value = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1);

		// Applies the loaded volume settings to the respective audio groups.
		SetGroupVolume(MASTER_VOLUME, masterVolume);
		SetGroupVolume(SFX_VOLUME, sfxVolume);
		SetGroupVolume(MUSIC_VOLUME, musicVolume);
	}

	// Updates the audio mixer settings when the audio change event is triggered.
	public void OnAudioChange()
	{
		SetGroupVolume(MASTER_VOLUME, masterVolume);
		SetGroupVolume(SFX_VOLUME, sfxVolume);
		SetGroupVolume(MUSIC_VOLUME, musicVolume);
	}

	public bool OnPlayAudioCue(AudioCueData audioCueData, AudioConfigurationData audioConfigurationData, Vector3 positionInSpace)
	{
		AudioEmitter audioEmitter = GetAudioEmitter();
		if (audioConfigurationData == null)
		{
			audioConfigurationData = this.audioConfigurationData;
		}
		audioEmitter.PlayAudioClip(audioCueData.GetClip(), audioConfigurationData, audioCueData.looping, positionInSpace);

		return true;		
	}

	public void OnPlayAudioClip(AudioClip audioClip)
	{
		AudioEmitter audioEmitter = GetAudioEmitter();
		audioEmitter.PlayAudioClip(audioClip, audioConfigurationData, false);
	}

	public void OnPlayAudio3DClip(AudioClip audioClip)
	{
		AudioEmitter audioEmitter = GetAudioEmitter();
		audioEmitter.PlayAudioClip(audioClip, audio3DConfigurationData, false);
	}

	private AudioEmitter GetAudioEmitter(Transform parent = null)
	{
		AudioEmitter audioEmitter = audioEmitters.Find(ae => !ae.IsPlaying());
		if (audioEmitter == null)
		{
			// Create a new GameObject with the AudioEmitter component
			GameObject emitterObject = new GameObject("AudioEmitter");
			audioEmitter = emitterObject.AddComponent<AudioEmitter>();

			// Set the parent if one is provided
			if (parent != null)
			{
				emitterObject.transform.SetParent(parent);
			}
			else
			{
				// No parent specified, set parent to AudioManager
				emitterObject.transform.SetParent(transform); // Or whatever default parent you want
			}

			audioEmitters.Add(audioEmitter);
		}
		return audioEmitter;
	}

	// Retrieves the current volume of a specified audio group in linear form.
	public float GetGroupVolume(string groupName)
	{
		audioMixer.GetFloat(groupName, out float dB);
		return DBToLinear(dB);
	}

	// Sets the volume of a specific audio group, converts it to decibels, and saves it in PlayerPrefs.
	public void SetGroupVolume(string groupName, float value)
	{
		float dB = LinearToDB(value);
		audioMixer.SetFloat(groupName, dB);
		PlayerPrefs.SetFloat(groupName, value);
	}

	// Converts a linear volume scale (0 to 1) to decibels for the audio mixer.
	// Decibels are logarithmic, not linear. For example, 20 dB is not twice the power ratio of 10 dB.
	public static float LinearToDB(float linear)
	{
		return (linear != 0) ? 20.0f * Mathf.Log10(linear) : -144.0f; // -144 dB is near silence.
	}

	// Converts a decibel value back to a linear scale (0 to 1).
	public static float DBToLinear(float dB)
	{
		return Mathf.Pow(10.0f, dB / 20.0f);
	}

	/// <summary>
	/// Converts semitones to Unity's pitch value
	/// </summary>
	/// <param name="semitones">Number of semitones (positive or negative)</param>
	/// <returns>Unity pitch value where 1.0 is the original pitch</returns>
	public static float SemitonesToPitch(float semitones)
	{
		// The formula is 2^(semitones/12)
		// Each octave (12 semitones) doubles the frequency
		// Each semitone is the 12th root of 2 (~1.059) times the previous semitone
		return Mathf.Pow(2f, semitones / 12f);
	}

	/// <summary>
	/// Converts Unity's pitch value to semitones
	/// </summary>
	/// <param name="pitch">Unity pitch value (typically between 0.5 and 3.0)</param>
	/// <returns>Number of semitones relative to the original pitch</returns>
	public static float PitchToSemitones(float pitch)
	{
		// The formula is 12 * log2(pitch)
		// This is the inverse of the SemitonesToPitch function
		// Note: Log(value, 2f) is logarithm with base 2
		return 12f * Mathf.Log(pitch, 2f);
	}
}
