using System;
using System.Collections;
using System.Collections.Generic;
using Noise;
using UnityEngine;

// Token: 0x020006C0 RID: 1728
public class ActorVortexer : SRBehaviour
{
	// Token: 0x06002409 RID: 9225 RVA: 0x0008B1BB File Offset: 0x000893BB
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x0008B1D0 File Offset: 0x000893D0
	public void OnDestroy()
	{
		for (int i = this.actors.Count - 1; i >= 0; i--)
		{
			this.Disconnect(i, false);
		}
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x0008B200 File Offset: 0x00089400
	public void FixedUpdate()
	{
		for (int i = this.actors.Count - 1; i >= 0; i--)
		{
			ActorVortexer.Actor actor = this.actors[i];
			if (!ActorVortexer.IsConnected(actor))
			{
				this.Disconnect(i, false);
			}
			else if (Randoms.SHARED.GetProbability(this.ejectChancePerSecond * Time.fixedDeltaTime))
			{
				this.Disconnect(i, true);
			}
			else
			{
				float value = Noise.PerlinNoise(this.timeDir.WorldTime(), 0f, (float)(actor.jointObj.GetInstanceID() * 1000), 500f, this.tornadoHeight, 1f) + this.heightOffset;
				float num = this.heightSpeed * Time.fixedDeltaTime;
				actor.height = Mathf.Clamp(value, actor.height - num, actor.height + num);
				actor.angleRads += Randoms.SHARED.GetInRange(-0.1f, -0.09f);
				float num2 = 0f;
				float num3 = 0f;
				if ((double)this.tornadoEccentricity != 0.0)
				{
					num2 = Mathf.Sin(actor.height * 3.1415927f * 2f / this.tornadoHeight) * this.tornadoEccentricity;
					num3 = -Mathf.Sin(actor.height * 3.1415927f * 2f / this.tornadoHeight) * this.tornadoEccentricity;
				}
				float num4 = Mathf.Lerp(this.spawnRad, this.maxRad, actor.height / this.tornadoHeight);
				if (this.treatZAsUp)
				{
					actor.jointObj.transform.localPosition = new Vector3(num4 * Mathf.Cos(actor.angleRads) + num2, num4 * Mathf.Sin(actor.angleRads) + num3, actor.height);
				}
				else
				{
					actor.jointObj.transform.localPosition = new Vector3(num4 * Mathf.Cos(actor.angleRads) + num2, actor.height, num4 * Mathf.Sin(actor.angleRads) + num3);
				}
				actor.jointObj.transform.eulerAngles = new Vector3(0f, actor.angleRads * 180f / 3.1415927f, 0f);
			}
		}
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x0008B440 File Offset: 0x00089640
	public void Connect(GameObject gameObject)
	{
		if (this.maxJointedActors > 0 && this.actors.Count >= this.maxJointedActors)
		{
			this.Disconnect(Randoms.SHARED.GetInt(this.actors.Count), true);
		}
		GameObject gameObject2 = new GameObject("Joint");
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		gameObject2.transform.SetParent(base.transform, true);
		float num = gameObject.transform.position.y - base.transform.position.y;
		float num2 = 0f;
		float num3 = 0f;
		if (this.tornadoEccentricity != 0f)
		{
			num2 = Mathf.Sin(num * 3.1415927f * 2f / this.tornadoHeight) * this.tornadoEccentricity;
			num3 = -Mathf.Sin(num * 3.1415927f * 2f / this.tornadoHeight) * this.tornadoEccentricity;
		}
		float angleRads = Mathf.Atan2(gameObject.transform.position.z - (base.transform.position.z + num3), gameObject.transform.position.x - (base.transform.position.x + num2));
		this.Connect(gameObject, gameObject2, angleRads, num);
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x0008B5A4 File Offset: 0x000897A4
	private void Connect(GameObject gameObject, GameObject jointObj, float angleRads, float height)
	{
		FixedJoint fixedJoint = jointObj.AddComponent<FixedJoint>();
		fixedJoint.breakForce = (fixedJoint.breakTorque = 1000f);
		jointObj.GetComponent<Rigidbody>().isKinematic = true;
		SafeJointReference.AttachSafely(gameObject, fixedJoint, true);
		fixedJoint.connectedAnchor = Vector3.zero;
		Vacuumable component = gameObject.GetComponent<Vacuumable>();
		component.capture(fixedJoint);
		component.SetTornadoed(true);
		SRBehaviour.SpawnAndPlayFX(this.spawnFX, jointObj.transform.position, jointObj.transform.rotation);
		this.actors.Add(new ActorVortexer.Actor
		{
			gameObject = gameObject,
			jointObj = jointObj,
			joint = fixedJoint,
			angleRads = angleRads,
			height = height
		});
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x0008B658 File Offset: 0x00089858
	private void Disconnect(int index, bool eject)
	{
		ActorVortexer.Actor actor = this.actors[index];
		this.actors.RemoveAt(index);
		if (actor.gameObject != null)
		{
			actor.gameObject.GetComponent<Vacuumable>().release();
			Rigidbody component = actor.gameObject.GetComponent<Rigidbody>();
			component.velocity = Vector3.zero;
			if (actor.jointObj != null && eject)
			{
				Vector3 lhs = actor.jointObj.transform.position - base.transform.position;
				lhs.y = 0f;
				Vector3 normalized = Vector3.Cross(lhs, Vector3.down).normalized;
				SRSingleton<SceneContext>.Instance.StartCoroutine(ActorVortexer.DelayedAddVelocity(component, normalized * this.ejectSpeed + Vector3.up * this.vertEjectSpeed));
			}
		}
		Destroyer.Destroy(actor.jointObj, "ActorVortexer.Eject");
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x0008B74A File Offset: 0x0008994A
	private static IEnumerator DelayedAddVelocity(Rigidbody body, Vector3 velChange)
	{
		yield return new WaitForEndOfFrame();
		if (body != null)
		{
			body.AddForce(velChange, ForceMode.VelocityChange);
		}
		yield break;
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x0008B760 File Offset: 0x00089960
	private static bool IsConnected(ActorVortexer.Actor actor)
	{
		return actor.gameObject != null && actor.jointObj != null && actor.joint != null;
	}

	// Token: 0x0400231B RID: 8987
	public GameObject spawnFX;

	// Token: 0x0400231C RID: 8988
	public float spawnRad = 0.5f;

	// Token: 0x0400231D RID: 8989
	public float maxRad = 5f;

	// Token: 0x0400231E RID: 8990
	public float tornadoHeight = 45f;

	// Token: 0x0400231F RID: 8991
	public float tornadoEccentricity = 5f;

	// Token: 0x04002320 RID: 8992
	public float heightSpeed = 5f;

	// Token: 0x04002321 RID: 8993
	public float heightOffset;

	// Token: 0x04002322 RID: 8994
	public float ejectSpeed = 30f;

	// Token: 0x04002323 RID: 8995
	public float vertEjectSpeed = 20f;

	// Token: 0x04002324 RID: 8996
	public bool treatZAsUp = true;

	// Token: 0x04002325 RID: 8997
	[Tooltip("Maximum number of actors we can handle at a time, or 0 for infinite.")]
	public int maxJointedActors;

	// Token: 0x04002326 RID: 8998
	private float ejectChancePerSecond = 0.03f;

	// Token: 0x04002327 RID: 8999
	public const float MIN_ANGULAR_SPEED = -0.1f;

	// Token: 0x04002328 RID: 9000
	public const float MAX_ANGULAR_SPEED = -0.09f;

	// Token: 0x04002329 RID: 9001
	private List<ActorVortexer.Actor> actors = new List<ActorVortexer.Actor>();

	// Token: 0x0400232A RID: 9002
	private TimeDirector timeDir;

	// Token: 0x020006C1 RID: 1729
	private class Actor
	{
		// Token: 0x0400232B RID: 9003
		public GameObject gameObject;

		// Token: 0x0400232C RID: 9004
		public GameObject jointObj;

		// Token: 0x0400232D RID: 9005
		public Joint joint;

		// Token: 0x0400232E RID: 9006
		public float angleRads;

		// Token: 0x0400232F RID: 9007
		public float height;
	}
}
