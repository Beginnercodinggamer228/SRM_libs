using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000872 RID: 2162
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public abstract class vp_Pickup : MonoBehaviour
{
	// Token: 0x06002DB6 RID: 11702 RVA: 0x000AF290 File Offset: 0x000AD490
	protected virtual void Start()
	{
		this.m_Transform = base.transform;
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.m_Audio = base.GetComponent<AudioSource>();
		if (Camera.main != null)
		{
			this.m_CameraMainTransform = Camera.main.transform;
		}
		base.GetComponent<Collider>().isTrigger = true;
		this.m_Audio.clip = this.PickupSound;
		this.m_Audio.playOnAwake = false;
		this.m_Audio.minDistance = 3f;
		this.m_Audio.maxDistance = 150f;
		this.m_Audio.rolloffMode = AudioRolloffMode.Linear;
		this.m_Audio.dopplerLevel = 0f;
		this.m_SpawnPosition = this.m_Transform.position;
		this.m_SpawnScale = this.m_Transform.localScale;
		this.RespawnScaleUpDuration = ((this.m_Rigidbody == null) ? Mathf.Abs(this.RespawnScaleUpDuration) : 0f);
		if (this.BobOffset == -1f)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
		if (this.RecipientTags.Count == 0)
		{
			this.RecipientTags.Add("Player");
		}
		if (this.RemoveDuration != 0f)
		{
			vp_Timer.In(this.RemoveDuration, new vp_Timer.Callback(this.Remove), null);
		}
		if (this.m_Rigidbody != null)
		{
			if (this.RigidbodyForce != Vector3.zero)
			{
				this.m_Rigidbody.AddForce(this.RigidbodyForce, ForceMode.Impulse);
			}
			if (this.RigidbodySpin != 0f)
			{
				this.m_Rigidbody.AddTorque(UnityEngine.Random.rotation.eulerAngles * this.RigidbodySpin);
			}
		}
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x000AF448 File Offset: 0x000AD648
	protected virtual void Update()
	{
		this.UpdateMotion();
		if (this.m_Depleted && !this.m_Audio.isPlaying)
		{
			this.Remove();
		}
		if (!this.m_Depleted && this.m_Rigidbody != null && this.m_Rigidbody.IsSleeping() && !this.m_Rigidbody.isKinematic)
		{
			this.m_Rigidbody.isKinematic = true;
			foreach (Collider collider in base.GetComponents<Collider>())
			{
				if (!collider.isTrigger)
				{
					collider.enabled = false;
				}
			}
		}
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x000AF4DC File Offset: 0x000AD6DC
	protected virtual void UpdateMotion()
	{
		if (this.m_Rigidbody != null)
		{
			return;
		}
		if (this.Billboard)
		{
			if (this.m_CameraMainTransform != null)
			{
				this.m_Transform.localEulerAngles = this.m_CameraMainTransform.eulerAngles;
			}
		}
		else
		{
			this.m_Transform.localEulerAngles += this.Spin * Time.deltaTime;
		}
		if (this.BobRate != 0f && this.BobAmp != 0f)
		{
			this.m_Transform.position = this.m_SpawnPosition + Vector3.up * (Mathf.Cos((Time.time + this.BobOffset) * (this.BobRate * 10f)) * this.BobAmp);
		}
		if (this.m_Transform.localScale != this.m_SpawnScale)
		{
			this.m_Transform.localScale = Vector3.Lerp(this.m_Transform.localScale, this.m_SpawnScale, Time.deltaTime / this.RespawnScaleUpDuration);
		}
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x000AF5F0 File Offset: 0x000AD7F0
	protected virtual void OnTriggerEnter(Collider col)
	{
		if (this.m_Depleted)
		{
			return;
		}
		foreach (string b in this.RecipientTags)
		{
			if (col.gameObject.tag == b)
			{
				goto IL_4E;
			}
		}
		return;
		IL_4E:
		if (col != this.m_LastCollider)
		{
			this.m_Recipient = col.gameObject.GetComponent<vp_FPPlayerEventHandler>();
		}
		if (this.m_Recipient == null)
		{
			return;
		}
		if (this.TryGive(this.m_Recipient))
		{
			this.m_Audio.pitch = (this.PickupSoundSlomo ? Time.timeScale : 1f);
			this.m_Audio.Play();
			base.GetComponent<Renderer>().enabled = false;
			this.m_Depleted = true;
			this.m_Recipient.HUDText.Send(this.GiveMessage);
			return;
		}
		if (!this.m_AlreadyFailed)
		{
			this.m_Audio.pitch = (this.FailSoundSlomo ? Time.timeScale : 1f);
			this.m_Audio.PlayOneShot(this.PickupFailSound);
			this.m_AlreadyFailed = true;
			this.m_Recipient.HUDText.Send(this.FailMessage);
		}
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x000AF74C File Offset: 0x000AD94C
	protected virtual void OnTriggerExit(Collider col)
	{
		this.m_AlreadyFailed = false;
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x000AF755 File Offset: 0x000AD955
	protected virtual bool TryGive(vp_FPPlayerEventHandler player)
	{
		return player.AddItem.Try(new object[]
		{
			this.InventoryName,
			1
		});
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x000AF784 File Offset: 0x000AD984
	protected virtual void Remove()
	{
		if (this == null)
		{
			return;
		}
		if (this.RespawnDuration == 0f)
		{
			vp_Utility.Destroy(base.gameObject);
			return;
		}
		if (!this.m_RespawnTimer.Active)
		{
			vp_Utility.Activate(base.gameObject, false);
			vp_Timer.In(this.RespawnDuration, new vp_Timer.Callback(this.Respawn), this.m_RespawnTimer);
		}
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x000AF7EC File Offset: 0x000AD9EC
	protected virtual void Respawn()
	{
		if (this.m_Transform == null)
		{
			return;
		}
		if (Camera.main != null)
		{
			this.m_CameraMainTransform = Camera.main.transform;
		}
		this.m_RespawnTimer.Cancel();
		this.m_Transform.position = this.m_SpawnPosition;
		if (this.m_Rigidbody == null && this.RespawnScaleUpDuration > 0f)
		{
			this.m_Transform.localScale = Vector3.zero;
		}
		base.GetComponent<Renderer>().enabled = true;
		vp_Utility.Activate(base.gameObject, true);
		this.m_Audio.pitch = (this.RespawnSoundSlomo ? Time.timeScale : 1f);
		this.m_Audio.PlayOneShot(this.RespawnSound);
		this.m_Depleted = false;
		if (this.BobOffset == -1f)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
		if (this.m_Rigidbody != null)
		{
			this.m_Rigidbody.isKinematic = false;
			foreach (Collider collider in base.GetComponents<Collider>())
			{
				if (!collider.isTrigger)
				{
					collider.enabled = true;
				}
			}
		}
	}

	// Token: 0x04002BE1 RID: 11233
	protected Transform m_Transform;

	// Token: 0x04002BE2 RID: 11234
	protected Rigidbody m_Rigidbody;

	// Token: 0x04002BE3 RID: 11235
	protected AudioSource m_Audio;

	// Token: 0x04002BE4 RID: 11236
	public string InventoryName = "Unnamed";

	// Token: 0x04002BE5 RID: 11237
	public List<string> RecipientTags = new List<string>();

	// Token: 0x04002BE6 RID: 11238
	private Collider m_LastCollider;

	// Token: 0x04002BE7 RID: 11239
	private vp_FPPlayerEventHandler m_Recipient;

	// Token: 0x04002BE8 RID: 11240
	public string GiveMessage = "Picked up an item";

	// Token: 0x04002BE9 RID: 11241
	public string FailMessage = "You currently can't pick up this item!";

	// Token: 0x04002BEA RID: 11242
	protected Vector3 m_SpawnPosition = Vector3.zero;

	// Token: 0x04002BEB RID: 11243
	protected Vector3 m_SpawnScale = Vector3.zero;

	// Token: 0x04002BEC RID: 11244
	public bool Billboard;

	// Token: 0x04002BED RID: 11245
	public Vector3 Spin = Vector3.zero;

	// Token: 0x04002BEE RID: 11246
	public float BobAmp;

	// Token: 0x04002BEF RID: 11247
	public float BobRate;

	// Token: 0x04002BF0 RID: 11248
	public float BobOffset = -1f;

	// Token: 0x04002BF1 RID: 11249
	public Vector3 RigidbodyForce = Vector3.zero;

	// Token: 0x04002BF2 RID: 11250
	public float RigidbodySpin;

	// Token: 0x04002BF3 RID: 11251
	public float RespawnDuration = 10f;

	// Token: 0x04002BF4 RID: 11252
	public float RespawnScaleUpDuration;

	// Token: 0x04002BF5 RID: 11253
	public float RemoveDuration;

	// Token: 0x04002BF6 RID: 11254
	public AudioClip PickupSound;

	// Token: 0x04002BF7 RID: 11255
	public AudioClip PickupFailSound;

	// Token: 0x04002BF8 RID: 11256
	public AudioClip RespawnSound;

	// Token: 0x04002BF9 RID: 11257
	public bool PickupSoundSlomo = true;

	// Token: 0x04002BFA RID: 11258
	public bool FailSoundSlomo = true;

	// Token: 0x04002BFB RID: 11259
	public bool RespawnSoundSlomo = true;

	// Token: 0x04002BFC RID: 11260
	protected bool m_Depleted;

	// Token: 0x04002BFD RID: 11261
	protected bool m_AlreadyFailed;

	// Token: 0x04002BFE RID: 11262
	protected vp_Timer.Handle m_RespawnTimer = new vp_Timer.Handle();

	// Token: 0x04002BFF RID: 11263
	private Transform m_CameraMainTransform;
}
