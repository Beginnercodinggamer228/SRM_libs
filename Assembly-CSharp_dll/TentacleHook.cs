using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004AE RID: 1198
public class TentacleHook : SRBehaviour, Attachment
{
	// Token: 0x0600190A RID: 6410 RVA: 0x00061884 File Offset: 0x0005FA84
	public void Init(GameObject source, GameObject target, Vector3 attachPoint, bool causeFear, float intermediateHeight)
	{
		TentacleHook.allHooked.Add(target);
		float magnitude = (attachPoint - source.transform.position).magnitude;
		this.parentSafeJoint = SafeJointReference.AttachSafely(source, this.parentJoint, true);
		this.parentJoint.minDistance = 0f;
		this.parentJoint.maxDistance = magnitude;
		this.hookSafeJoint = SafeJointReference.AttachSafely(target, this.hookJoint, true);
		this.hookJoint.transform.position = attachPoint;
		this.hookJoint.connectedAnchor = Vector3.zero;
		this.hookEndObj = this.hookJoint.gameObject;
		if (causeFear)
		{
			SlimeEmotions component = target.GetComponent<SlimeEmotions>();
			if (component != null)
			{
				component.Adjust(SlimeEmotions.Emotion.FEAR, 1f);
			}
		}
		else
		{
			SlimeFaceAnimator component2 = target.GetComponent<SlimeFaceAnimator>();
			if (component2 != null)
			{
				component2.SetTrigger("triggerLongAwe");
			}
		}
		this.targetFleeer = target.GetComponent<FleeThreats>();
		if (this.targetFleeer != null)
		{
			this.targetFleeer.AddGrappler(this);
		}
		this.target = target;
		this.AdjustConnector();
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x0006199C File Offset: 0x0005FB9C
	public void Awake()
	{
		this.tentacleMaterial = this.tentacleObject.GetComponent<Renderer>().material;
		this.tentacleMaterial.SetFloat("_Alpha", 0f);
		this.fadeInTime = new float?(Time.time + 0.5f);
		SECTR_AudioSystem.Play(this.shootCue, this.parentJoint.transform.position, false);
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x00061A08 File Offset: 0x0005FC08
	public void OnDestroy()
	{
		Destroyer.Destroy(this.tentacleMaterial, "TentacleHook.OnDestroy");
		if (this.targetFleeer != null)
		{
			this.targetFleeer.RemoveGrappler(this);
		}
		if (this.target != null)
		{
			TentacleHook.allHooked.Remove(this.target);
		}
		TentacleHook.allHooked.RemoveAll((GameObject x) => x == null);
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x00061A88 File Offset: 0x0005FC88
	public void FixedUpdate()
	{
		if (!this.snapping && (this.hookJoint == null || this.parentJoint == null || this.hookJoint.connectedBody == null || this.parentJoint.connectedBody == null))
		{
			this.Snap();
		}
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x00061AE8 File Offset: 0x0005FCE8
	private void OnJointBreak(float breakForce)
	{
		if (this.hookJoint != null && this.hookJoint.connectedBody == null)
		{
			Destroyer.Destroy(this.hookJoint, "TentacleHook.OnJointBreak#1");
			this.hookJoint = null;
			Destroyer.Destroy(this.hookSafeJoint, "TentacleHook.OnJointBreak#2");
		}
		if (this.parentJoint != null && this.parentJoint.connectedBody == null)
		{
			Destroyer.Destroy(this.parentJoint, "TentacleHook.OnJointBreak#3");
			this.parentJoint = null;
			Destroyer.Destroy(this.parentSafeJoint, "TentacleHook.OnJointBreak#4");
		}
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x00061B88 File Offset: 0x0005FD88
	public void Update()
	{
		if (this.snapping)
		{
			this.snapProgress = Mathf.Min(1f, this.snapProgress + Time.deltaTime / 0.2f);
			float value = 1f - this.snapProgress;
			this.tentacleMaterial.SetFloat("_Alpha", value);
			if (this.snapProgress >= 1f)
			{
				Destroyer.Destroy(base.gameObject, "TentacleHook.Update");
			}
			return;
		}
		float time = Time.time;
		if (this.fadeInTime != null)
		{
			float num = time;
			float? num2 = this.fadeInTime;
			if (num <= num2.GetValueOrDefault() & num2 != null)
			{
				float value2 = 1f - (this.fadeInTime.Value - time) / 0.5f;
				this.tentacleMaterial.SetFloat("_Alpha", value2);
			}
			else
			{
				this.tentacleMaterial.SetFloat("_Alpha", 1f);
				if (this.target != null)
				{
					SECTR_AudioSystem.Play(this.grabCue, this.target.transform.position, false);
				}
				this.fadeInTime = null;
			}
		}
		if (this.parentJoint != null && !this.pauseRetract)
		{
			this.parentJoint.maxDistance = Mathf.Max(0f, this.parentJoint.maxDistance - Time.deltaTime * this.retractSpeed);
		}
		this.AdjustConnector();
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x00061CF2 File Offset: 0x0005FEF2
	public void SetPauseRetract(bool pauseRetract)
	{
		this.pauseRetract = pauseRetract;
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x00061CFB File Offset: 0x0005FEFB
	public static bool IsAlreadyHooked(GameObject obj)
	{
		return TentacleHook.allHooked.Contains(obj);
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x00061D08 File Offset: 0x0005FF08
	private void AdjustConnector()
	{
		if (this.hookJoint != null)
		{
			Vector3 forward = this.hookJoint.transform.position - this.parentEnd.transform.position;
			if (forward.sqrMagnitude > 0f)
			{
				this.parentEnd.transform.forward = forward;
				this.hookJoint.transform.forward = forward;
			}
		}
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00061D7C File Offset: 0x0005FF7C
	private void Snap()
	{
		this.snapping = true;
		Destroyer.Destroy(this.hookJoint, "TentacleHook.Snap#1");
		Destroyer.Destroy(this.parentJoint, "TentacleHook.Snap#2");
		if (this.hookEndObj != null)
		{
			Rigidbody component = this.hookEndObj.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x040018C4 RID: 6340
	[Tooltip("The joint connecting us to our hooked victim.")]
	public FixedJoint hookJoint;

	// Token: 0x040018C5 RID: 6341
	[Tooltip("The joint connecting us back to the tentacle-user.")]
	public SpringJoint parentJoint;

	// Token: 0x040018C6 RID: 6342
	[Tooltip("The part of ourselves that is connected to our parent.")]
	public GameObject parentEnd;

	// Token: 0x040018C7 RID: 6343
	[Tooltip("In meters per second, how quickly we reduce the length of the tentacle.")]
	public float retractSpeed = 1f;

	// Token: 0x040018C8 RID: 6344
	public SECTR_AudioCue shootCue;

	// Token: 0x040018C9 RID: 6345
	public SECTR_AudioCue grabCue;

	// Token: 0x040018CA RID: 6346
	private const float CONVERT_TO_FIXED_DIST = 0.3f;

	// Token: 0x040018CB RID: 6347
	private const float CONVERT_TO_FIXED_DIST_SQR = 0.09f;

	// Token: 0x040018CC RID: 6348
	public GameObject tentacleObject;

	// Token: 0x040018CD RID: 6349
	private Material tentacleMaterial;

	// Token: 0x040018CE RID: 6350
	private float? fadeInTime;

	// Token: 0x040018CF RID: 6351
	private FleeThreats targetFleeer;

	// Token: 0x040018D0 RID: 6352
	private GameObject target;

	// Token: 0x040018D1 RID: 6353
	private bool snapping;

	// Token: 0x040018D2 RID: 6354
	private float snapProgress;

	// Token: 0x040018D3 RID: 6355
	private GameObject hookEndObj;

	// Token: 0x040018D4 RID: 6356
	private bool pauseRetract;

	// Token: 0x040018D5 RID: 6357
	private static List<GameObject> allHooked = new List<GameObject>();

	// Token: 0x040018D6 RID: 6358
	private const float FADE_IN_TIME = 0.5f;

	// Token: 0x040018D7 RID: 6359
	private const float SNAP_TIME = 0.2f;

	// Token: 0x040018D8 RID: 6360
	private SafeJointReference parentSafeJoint;

	// Token: 0x040018D9 RID: 6361
	private SafeJointReference hookSafeJoint;
}
