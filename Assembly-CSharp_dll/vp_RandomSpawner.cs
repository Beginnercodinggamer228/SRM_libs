using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200085C RID: 2140
[RequireComponent(typeof(AudioSource))]
public class vp_RandomSpawner : MonoBehaviour
{
	// Token: 0x06002D2B RID: 11563 RVA: 0x000AB8FC File Offset: 0x000A9AFC
	private void Awake()
	{
		if (this.SpawnObjects == null)
		{
			return;
		}
		int index = UnityEngine.Random.Range(0, this.SpawnObjects.Count);
		if (this.SpawnObjects[index] == null)
		{
			return;
		}
		((GameObject)vp_Utility.Instantiate(this.SpawnObjects[index], base.transform.position, base.transform.rotation)).transform.Rotate(UnityEngine.Random.rotation.eulerAngles);
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Audio.playOnAwake = true;
		if (this.Sound != null)
		{
			this.m_Audio.rolloffMode = AudioRolloffMode.Linear;
			this.m_Audio.clip = this.Sound;
			this.m_Audio.pitch = UnityEngine.Random.Range(this.SoundMinPitch, this.SoundMaxPitch) * Time.timeScale;
			this.m_Audio.Play();
		}
	}

	// Token: 0x04002B37 RID: 11063
	private AudioSource m_Audio;

	// Token: 0x04002B38 RID: 11064
	public AudioClip Sound;

	// Token: 0x04002B39 RID: 11065
	public float SoundMinPitch = 0.8f;

	// Token: 0x04002B3A RID: 11066
	public float SoundMaxPitch = 1.2f;

	// Token: 0x04002B3B RID: 11067
	public bool RandomAngle = true;

	// Token: 0x04002B3C RID: 11068
	public List<GameObject> SpawnObjects;
}
