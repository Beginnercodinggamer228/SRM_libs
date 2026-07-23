using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018B RID: 395
public abstract class DroneProgramSource<T> : DroneProgramSource where T : class
{
	// Token: 0x06000867 RID: 2151 RVA: 0x0002778C File Offset: 0x0002598C
	public override bool Relevancy()
	{
		if (this.drone.ammo.IsFull())
		{
			return false;
		}
		if (!this.drone.station.battery.HasAny())
		{
			return false;
		}
		if (this.drone.ammo.Any() && !base.HasAvailableDestinationSpace(this.drone.ammo.GetSlotName(), this.drone.ammo.GetSlotCount() + 1))
		{
			return false;
		}
		foreach (T t in this.GetSources((Identifiable.Id id) => this.predicate(id) && this.drone.ammo.CouldAddToSlot(id) && (this.drone.ammo.Any() || base.HasAvailableDestinationSpace(id))))
		{
			this.sourceGameObject = this.GetTargetGameObject(t);
			this.source = t;
			if (DroneProgramSource.BLACKLIST.Add(this.sourceGameObject) && base.GeneratePath(this.GetSubnetwork(), this.GetTargetOrientations(), this.GetTargetPosition()))
			{
				return true;
			}
		}
		this.sourceGameObject = null;
		this.source = default(T);
		return false;
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x000278A4 File Offset: 0x00025AA4
	public override void Selected()
	{
		base.Selected();
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x000278AC File Offset: 0x00025AAC
	public override void Deselected()
	{
		base.Deselected();
		DroneProgramSource.BLACKLIST.Remove(this.sourceGameObject);
		this.sourceGameObject = null;
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x000278CC File Offset: 0x00025ACC
	protected override void OnPathGenerationFailed()
	{
		base.OnPathGenerationFailed();
		if (this.sourceGameObject != null)
		{
			this.sourceGameObject.AddComponent<DroneProgramSource_PathGenerationFailure>();
			this.sourceGameObject = null;
		}
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x000278F5 File Offset: 0x00025AF5
	protected sealed override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		return this.GetTargetOrientations(this.source);
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00027903 File Offset: 0x00025B03
	protected sealed override Vector3 GetTargetPosition()
	{
		return this.GetTargetPosition(this.source);
	}

	// Token: 0x0600086D RID: 2157
	protected abstract IEnumerable<T> GetSources(Predicate<Identifiable.Id> predicate);

	// Token: 0x0600086E RID: 2158
	protected abstract IEnumerable<DroneProgram.Orientation> GetTargetOrientations(T source);

	// Token: 0x0600086F RID: 2159
	protected abstract Vector3 GetTargetPosition(T source);

	// Token: 0x06000870 RID: 2160
	protected abstract GameObject GetTargetGameObject(T source);

	// Token: 0x04000759 RID: 1881
	protected T source;

	// Token: 0x0400075A RID: 1882
	private GameObject sourceGameObject;
}
