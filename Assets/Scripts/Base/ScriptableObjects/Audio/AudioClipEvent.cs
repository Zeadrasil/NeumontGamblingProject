using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "AudioClipEvent", menuName = "Audio/Audio Clip Event")]
public class AudioClipEvent : ScriptableObjectBase
{
	public AudioClipPlayAction OnAudioClipPlay;

	public bool OnPlayEvent(AudioClip audioClip)
	{
		if (OnAudioClipPlay != null)
		{
			OnAudioClipPlay.Invoke(audioClip);
		}

		return true;
	}

	public delegate bool AudioClipPlayAction(AudioClip audioClip);
}
