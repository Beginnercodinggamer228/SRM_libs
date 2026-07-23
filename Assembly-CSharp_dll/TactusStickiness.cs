using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200078F RID: 1935
public class TactusStickiness : MonoBehaviour
{
	// Token: 0x06002855 RID: 10325 RVA: 0x0009918C File Offset: 0x0009738C
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.waiter = base.GetComponentInParent<WaitForChargeup>();
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x000991AC File Offset: 0x000973AC
	public void OnCollisionEnter(Collision col)
	{
		if (this.waiter != null && this.waiter.IsWaiting())
		{
			return;
		}
		if (!this.ineligibleGameObjIds.ContainsKey(col.gameObject.GetInstanceID()))
		{
			this.ineligibleGameObjIds[col.gameObject.GetInstanceID()] = double.PositiveInfinity;
			this.CreateJointObject(col.gameObject);
		}
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x00099218 File Offset: 0x00097418
	public void Update()
	{
		List<int> list = null;
		foreach (KeyValuePair<int, double> keyValuePair in this.ineligibleGameObjIds)
		{
			if (this.timeDir.HasReached(keyValuePair.Value))
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(keyValuePair.Key);
			}
		}
		if (list != null)
		{
			foreach (int key in list)
			{
				this.ineligibleGameObjIds.Remove(key);
			}
		}
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000992D8 File Offset: 0x000974D8
	private void ReportBrokenJoint(int objID)
	{
		this.ineligibleGameObjIds[objID] = this.timeDir.HoursFromNowOrStart(0.016666668f);
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000992F8 File Offset: 0x000974F8
	private void CreateJointObject(GameObject stuckObj)
	{
		GameObject gameObject = new GameObject("Joint");
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.position = stuckObj.transform.position;
		gameObject.transform.rotation = stuckObj.transform.rotation;
		gameObject.AddComponent<Rigidbody>().isKinematic = true;
		FixedJoint joint = gameObject.AddComponent<FixedJoint>();
		SafeJointReference.AttachSafely(stuckObj, joint, true);
		gameObject.AddComponent<TactusStickiness.JointHelper>().SetTactusStickiness(this, stuckObj, this.timeDir.HoursFromNowOrStart(this.jointTTLMins * 0.016666668f));
	}

	// Token: 0x04002803 RID: 10243
	public float jointBreakForce = 10f;

	// Token: 0x04002804 RID: 10244
	public float jointBreakTorque = float.PositiveInfinity;

	// Token: 0x04002805 RID: 10245
	public float jointTTLMins = 10f;

	// Token: 0x04002806 RID: 10246
	private Dictionary<int, double> ineligibleGameObjIds = new Dictionary<int, double>();

	// Token: 0x04002807 RID: 10247
	private TimeDirector timeDir;

	// Token: 0x04002808 RID: 10248
	private WaitForChargeup waiter;

	// Token: 0x02000790 RID: 1936
	public class JointHelper : MonoBehaviour
	{
		// Token: 0x0600285B RID: 10331 RVA: 0x000993C0 File Offset: 0x000975C0
		public void SetTactusStickiness(TactusStickiness stickiness, GameObject stuckObj, double expiration)
		{
			this.stickiness = stickiness;
			this.stuckObj = stuckObj;
			this.joint = base.GetComponent<Joint>();
			this.expiration = expiration;
			this.plexer = stuckObj.GetComponent<SlimeSubbehaviourPlexer>();
			this.plexer != null;
			this.stuckColliders = stuckObj.GetComponents<Collider>();
			Collider[] array = this.stuckColliders;
			for (int i = 0; i < array.Length; i++)
			{
				Physics.IgnoreCollision(array[i], stickiness.GetComponent<Collider>(), true);
			}
			base.StartCoroutine(this.DelayedSetJointBreakForce());
		}

		// Token: 0x0600285C RID: 10332 RVA: 0x00099444 File Offset: 0x00097644
		public void OnDestroy()
		{
			this.plexer != null;
			foreach (Collider collider in this.stuckColliders)
			{
				if (collider != null)
				{
					Physics.IgnoreCollision(collider, this.stickiness.GetComponent<Collider>(), false);
				}
			}
		}

		// Token: 0x0600285D RID: 10333 RVA: 0x00099492 File Offset: 0x00097692
		public void OnJointBreak(float force)
		{
			this.stickiness.ReportBrokenJoint(this.stuckObj.GetInstanceID());
			Destroyer.Destroy(base.gameObject, "TactusStickiness.OnJointBreak");
		}

		// Token: 0x0600285E RID: 10334 RVA: 0x000994BA File Offset: 0x000976BA
		public void Update()
		{
			if (this.stickiness.timeDir.HasReached(this.expiration))
			{
				this.stickiness.ReportBrokenJoint(this.stuckObj.GetInstanceID());
				Destroyer.Destroy(base.gameObject, "TactusStickiness.Update");
			}
		}

		// Token: 0x0600285F RID: 10335 RVA: 0x000994FA File Offset: 0x000976FA
		public IEnumerator DelayedSetJointBreakForce()
		{
			yield return new WaitForSeconds(1f);
			if (this.joint != null)
			{
				this.joint.breakForce = this.stickiness.jointBreakForce;
				this.joint.breakTorque = this.stickiness.jointBreakTorque;
			}
			yield break;
		}

		// Token: 0x04002809 RID: 10249
		private TactusStickiness stickiness;

		// Token: 0x0400280A RID: 10250
		private GameObject stuckObj;

		// Token: 0x0400280B RID: 10251
		private Joint joint;

		// Token: 0x0400280C RID: 10252
		private double expiration;

		// Token: 0x0400280D RID: 10253
		private SlimeSubbehaviourPlexer plexer;

		// Token: 0x0400280E RID: 10254
		private Collider[] stuckColliders;
	}
}
