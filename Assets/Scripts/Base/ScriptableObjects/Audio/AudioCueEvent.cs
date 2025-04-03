using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AudioCueEvent", menuName = "Audio/Audio Cue Event")]
public class AudioCueEvent : ScriptableObjectBase
{
	public AudioCuePlayAction OnAudioCuePlay;

	public bool OnPlayEvent(AudioCueData audioCue, AudioConfigurationData audioConfiguration, Vector3 positionInSpace = default)
	{
		if (OnAudioCuePlay != null)
		{
			OnAudioCuePlay.Invoke(audioCue, audioConfiguration, positionInSpace);
		}

		return true;
	}

	public delegate bool AudioCuePlayAction(AudioCueData audioCueData, AudioConfigurationData audioConfigurationData, Vector3 positionInSpace);
}
