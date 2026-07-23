using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000786 RID: 1926
public class SlimeStage : MonoBehaviour
{
	// Token: 0x06002838 RID: 10296 RVA: 0x000988AB File Offset: 0x00096AAB
	public void Awake()
	{
		this.anim = base.GetComponentInParent<Animator>();
		this.animActiveId = Animator.StringToHash("active");
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x000988CC File Offset: 0x00096ACC
	public void OnTriggerEnter(Collider col)
	{
		if (!col.isTrigger && this.joint == null)
		{
			Identifiable.Id id = Identifiable.GetId(col.gameObject);
			if (Identifiable.IsSlime(id))
			{
				if (Identifiable.IsTarr(id))
				{
					SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.SLIME_STAGE_TARRS, 1);
				}
				this.slime = col.gameObject;
				this.slime.transform.rotation = Quaternion.Euler(new Vector3(0f, this.slime.transform.rotation.eulerAngles.y, 0f));
				this.slime.GetComponent<SlimeSubbehaviourPlexer>().RegisterBehaviorBlocker();
				this.slime.GetComponent<SlimeFaceAnimator>().SetTrigger("triggerAwe");
				this.joint = SlimeStage.CreateJoint(((Identifiable.IsLargo(id) || Identifiable.IsTarr(id)) ? this.largoJointBody : this.jointBody).gameObject);
				SafeJointReference.AttachSafely(this.slime, this.joint, true);
				base.StartCoroutine(this.DelayedSetJointBreakForce());
				this.isJointActive = true;
				this.jointBody.transform.localRotation = Quaternion.Euler(Vector3.zero);
				this.largoJointBody.transform.localRotation = Quaternion.Euler(Vector3.zero);
				SRBehaviour.InstantiateDynamic(this.activationFX, base.transform.position, base.transform.rotation, false);
				this.anim.SetBool(this.animActiveId, true);
				this.attractor.SetAweFactor(1f);
			}
		}
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x00098A66 File Offset: 0x00096C66
	private IEnumerator DelayedSetJointBreakForce()
	{
		yield return new WaitForSeconds(1f);
		if (this.joint != null)
		{
			this.joint.breakForce = this.jointBreakForce;
			this.joint.breakTorque = this.jointBreakTorque;
		}
		yield break;
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x00098A78 File Offset: 0x00096C78
	public void FixedUpdate()
	{
		if (this.isJointActive && this.joint == null)
		{
			if (this.slime != null)
			{
				this.slime.GetComponent<SlimeSubbehaviourPlexer>().UnregisterBehaviorBlocker();
				this.slime = null;
			}
			this.anim.SetBool(this.animActiveId, false);
			this.attractor.SetAweFactor(0f);
			this.isJointActive = false;
		}
		if (this.joint != null)
		{
			this.jointBody.transform.Rotate(Vector3.up, 90f * Time.fixedDeltaTime);
			this.largoJointBody.transform.Rotate(Vector3.up, 90f * Time.fixedDeltaTime);
			if (this.joint.connectedBody == null)
			{
				Destroyer.Destroy(this.joint, "SlimeStage.FixedUpdate");
			}
		}
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x00098B5C File Offset: 0x00096D5C
	private static Joint CreateJoint(GameObject parent)
	{
		ConfigurableJoint configurableJoint = parent.AddComponent<ConfigurableJoint>();
		configurableJoint.anchor = Vector3.zero;
		configurableJoint.autoConfigureConnectedAnchor = false;
		configurableJoint.connectedAnchor = Vector3.zero;
		SoftJointLimitSpring softJointLimitSpring = default(SoftJointLimitSpring);
		softJointLimitSpring.damper = 0.2f;
		softJointLimitSpring.spring = 1000f;
		configurableJoint.xMotion = ConfigurableJointMotion.Limited;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
		configurableJoint.linearLimitSpring = softJointLimitSpring;
		configurableJoint.angularXLimitSpring = softJointLimitSpring;
		configurableJoint.angularYZLimitSpring = softJointLimitSpring;
		return configurableJoint;
	}

	// Token: 0x040027D4 RID: 10196
	public Rigidbody jointBody;

	// Token: 0x040027D5 RID: 10197
	public Rigidbody largoJointBody;

	// Token: 0x040027D6 RID: 10198
	public Attractor attractor;

	// Token: 0x040027D7 RID: 10199
	public GameObject activationFX;

	// Token: 0x040027D8 RID: 10200
	public float jointBreakForce = 20f;

	// Token: 0x040027D9 RID: 10201
	public float jointBreakTorque = float.PositiveInfinity;

	// Token: 0x040027DA RID: 10202
	private Joint joint;

	// Token: 0x040027DB RID: 10203
	private bool isJointActive;

	// Token: 0x040027DC RID: 10204
	private GameObject slime;

	// Token: 0x040027DD RID: 10205
	private Animator anim;

	// Token: 0x040027DE RID: 10206
	private int animActiveId;
}
