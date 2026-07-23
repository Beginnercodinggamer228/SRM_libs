using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000838 RID: 2104
public static class vp_AudioUtility
{
	// Token: 0x06002C2C RID: 11308 RVA: 0x000A6BB8 File Offset: 0x000A4DB8
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds, Vector2 pitchRange)
	{
		if (audioSource == null)
		{
			return;
		}
		if (sounds == null || sounds.Count == 0)
		{
			return;
		}
		AudioClip audioClip = sounds[UnityEngine.Random.Range(0, sounds.Count)];
		if (audioClip == null)
		{
			return;
		}
		if (pitchRange == Vector2.one)
		{
			audioSource.pitch = Time.timeScale;
		}
		else
		{
			audioSource.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;
		}
		audioSource.PlayOneShot(audioClip);
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x000A6C36 File Offset: 0x000A4E36
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds)
	{
		vp_AudioUtility.PlayRandomSound(audioSource, sounds, Vector2.one);
	}
}
