using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200084C RID: 2124
[RequireComponent(typeof(AudioSource))]
public class vp_Debris : MonoBehaviour
{
	// Token: 0x06002CBC RID: 11452 RVA: 0x000A8B10 File Offset: 0x000A6D10
	private void Awake()
	{
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Colliders = base.GetComponentsInChildren<Collider>();
		foreach (Collider collider in this.m_Colliders)
		{
			if (collider.GetComponent<Rigidbody>())
			{
				this.m_PiecesInitial.Add(collider, new Dictionary<string, object>
				{
					{
						"Position",
						collider.transform.localPosition
					},
					{
						"Rotation",
						collider.transform.localRotation
					}
				});
			}
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x000A8BA4 File Offset: 0x000A6DA4
	private void OnEnable()
	{
		this.m_Destroy = false;
		this.m_Audio.playOnAwake = true;
		foreach (Collider collider in this.m_Colliders)
		{
			Rigidbody component = collider.GetComponent<Rigidbody>();
			if (component != null)
			{
				collider.transform.localPosition = (Vector3)this.m_PiecesInitial[collider]["Position"];
				collider.transform.localRotation = (Quaternion)this.m_PiecesInitial[collider]["Rotation"];
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
				component.AddExplosionForce(this.Force / Time.timeScale / vp_TimeUtility.AdjustedTimeScale, base.transform.position, this.Radius, this.UpForce);
				Collider c = collider;
				vp_Timer.In(UnityEngine.Random.Range(this.LifeTime * 0.5f, this.LifeTime * 0.95f), delegate()
				{
					if (c != null)
					{
						vp_Utility.Destroy(c.gameObject);
					}
				}, null);
			}
		}
		vp_Timer.In(this.LifeTime, delegate()
		{
			this.m_Destroy = true;
		}, null);
		if (this.Sounds.Count > 0)
		{
			this.m_Audio.rolloffMode = AudioRolloffMode.Linear;
			this.m_Audio.clip = this.Sounds[UnityEngine.Random.Range(0, this.Sounds.Count)];
			this.m_Audio.pitch = UnityEngine.Random.Range(this.SoundMinPitch, this.SoundMaxPitch) * Time.timeScale;
			this.m_Audio.Play();
		}
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x000A8D4C File Offset: 0x000A6F4C
	private void Update()
	{
		if (this.m_Destroy && !base.GetComponent<AudioSource>().isPlaying)
		{
			vp_Utility.Destroy(base.gameObject);
		}
	}

	// Token: 0x04002AA7 RID: 10919
	public float Radius = 2f;

	// Token: 0x04002AA8 RID: 10920
	public float Force = 10f;

	// Token: 0x04002AA9 RID: 10921
	public float UpForce = 1f;

	// Token: 0x04002AAA RID: 10922
	private AudioSource m_Audio;

	// Token: 0x04002AAB RID: 10923
	public List<AudioClip> Sounds = new List<AudioClip>();

	// Token: 0x04002AAC RID: 10924
	public float SoundMinPitch = 0.8f;

	// Token: 0x04002AAD RID: 10925
	public float SoundMaxPitch = 1.2f;

	// Token: 0x04002AAE RID: 10926
	public float LifeTime = 5f;

	// Token: 0x04002AAF RID: 10927
	protected bool m_Destroy;

	// Token: 0x04002AB0 RID: 10928
	protected Collider[] m_Colliders;

	// Token: 0x04002AB1 RID: 10929
	protected Dictionary<Collider, Dictionary<string, object>> m_PiecesInitial = new Dictionary<Collider, Dictionary<string, object>>();
}
