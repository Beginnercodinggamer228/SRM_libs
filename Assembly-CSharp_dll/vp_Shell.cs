using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000851 RID: 2129
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public class vp_Shell : MonoBehaviour
{
	// Token: 0x06002CDD RID: 11485 RVA: 0x000A94A8 File Offset: 0x000A76A8
	private void Awake()
	{
		this.m_Transform = base.transform;
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Audio.playOnAwake = false;
		this.m_Audio.dopplerLevel = 0f;
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x000A94F8 File Offset: 0x000A76F8
	private void OnEnable()
	{
		this.m_RestAngleFunc = null;
		this.m_RemoveTime = Time.time + this.LifeTime;
		this.m_RestTime = Time.time + this.LifeTime * 0.25f;
		this.m_Rigidbody.maxAngularVelocity = 100f;
		this.m_Rigidbody.velocity = Vector3.zero;
		this.m_Rigidbody.angularVelocity = Vector3.zero;
		this.m_Rigidbody.constraints = RigidbodyConstraints.None;
		base.GetComponent<Collider>().enabled = true;
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x000A9580 File Offset: 0x000A7780
	private void Update()
	{
		if (this.m_RestAngleFunc == null)
		{
			if (Time.time > this.m_RestTime)
			{
				this.DecideRestAngle();
			}
		}
		else
		{
			this.m_RestAngleFunc();
		}
		if (Time.time > this.m_RemoveTime)
		{
			this.m_Transform.localScale = Vector3.Lerp(this.m_Transform.localScale, Vector3.zero, Time.deltaTime * 60f * 0.2f);
			if (Time.time > this.m_RemoveTime + 0.5f)
			{
				vp_Utility.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x000A9614 File Offset: 0x000A7814
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 2f)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				this.m_Rigidbody.AddRelativeTorque(-UnityEngine.Random.rotation.eulerAngles * 0.15f);
			}
			else
			{
				this.m_Rigidbody.AddRelativeTorque(UnityEngine.Random.rotation.eulerAngles * 0.15f);
			}
			if (this.m_Audio != null && this.m_BounceSounds.Count > 0)
			{
				this.m_Audio.pitch = Time.timeScale;
				this.m_Audio.PlayOneShot(this.m_BounceSounds[UnityEngine.Random.Range(0, this.m_BounceSounds.Count)]);
				return;
			}
		}
		else if (UnityEngine.Random.value > this.m_Persistence)
		{
			base.GetComponent<Collider>().enabled = false;
			this.m_RemoveTime = Time.time + 0.5f;
		}
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x000A9710 File Offset: 0x000A7910
	protected void DecideRestAngle()
	{
		if (Mathf.Abs(this.m_Transform.eulerAngles.x - 270f) < 55f)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(this.m_Transform.position, Vector3.down), out raycastHit, 1f) && raycastHit.normal == Vector3.up)
			{
				this.m_RestAngleFunc = new vp_Shell.RestAngleFunc(this.UpRight);
				this.m_Rigidbody.constraints = (RigidbodyConstraints)80;
			}
			return;
		}
		this.m_RestAngleFunc = new vp_Shell.RestAngleFunc(this.TippedOver);
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x000A97A8 File Offset: 0x000A79A8
	protected void UpRight()
	{
		this.m_Transform.rotation = Quaternion.Lerp(this.m_Transform.rotation, Quaternion.Euler(-90f, this.m_Transform.rotation.y, this.m_Transform.rotation.z), Time.time * (Time.deltaTime * 60f * 0.05f));
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000A9814 File Offset: 0x000A7A14
	protected void TippedOver()
	{
		this.m_Transform.localRotation = Quaternion.Lerp(this.m_Transform.localRotation, Quaternion.Euler(0f, this.m_Transform.localEulerAngles.y, this.m_Transform.localEulerAngles.z), Time.time * (Time.deltaTime * 60f * 0.005f));
	}

	// Token: 0x04002AC5 RID: 10949
	private Transform m_Transform;

	// Token: 0x04002AC6 RID: 10950
	private Rigidbody m_Rigidbody;

	// Token: 0x04002AC7 RID: 10951
	private AudioSource m_Audio;

	// Token: 0x04002AC8 RID: 10952
	public float LifeTime = 10f;

	// Token: 0x04002AC9 RID: 10953
	protected float m_RemoveTime;

	// Token: 0x04002ACA RID: 10954
	public float m_Persistence = 1f;

	// Token: 0x04002ACB RID: 10955
	protected vp_Shell.RestAngleFunc m_RestAngleFunc;

	// Token: 0x04002ACC RID: 10956
	protected float m_RestTime;

	// Token: 0x04002ACD RID: 10957
	public List<AudioClip> m_BounceSounds = new List<AudioClip>();

	// Token: 0x02000852 RID: 2130
	// (Invoke) Token: 0x06002CE6 RID: 11494
	public delegate void RestAngleFunc();
}
