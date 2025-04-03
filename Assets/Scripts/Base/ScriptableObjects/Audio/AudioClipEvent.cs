using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "AudioClipEvent", menuName = "Audio/Audio Clip Event")]
public class AudioClipEvent : ScriptableObjectBase
{
	public AudioClipPlayAction OnAudioClipPlay;

	public bool OnPlayEvent(AudioClip audioClip, Vector3 positionInSpace = default)
	{
		if (OnAudioClipPlay != null)
		{
			OnAudioClipPlay.Invoke(audioClip, positionInSpace);
		}

		return true;
	}

	public delegate bool AudioClipPlayAction(AudioClip audioClip, Vector3 positionInSpace);
}
