using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class DroneAnimator : SRAnimator<Drone>
{
	// Token: 0x0600067D RID: 1661 RVA: 0x00022E2D File Offset: 0x0002102D
	public override void Awake()
	{
		base.Awake();
		this.SetAnimation(DroneAnimator.Id.IDLE);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00022E3C File Offset: 0x0002103C
	public void Start()
	{
		this.battery.onHasAnyChanged -= this.OnBatteryHasAnyChanged;
		this.battery.onHasAnyChanged += this.OnBatteryHasAnyChanged;
		this.OnBatteryHasAnyChanged();
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00022E74 File Offset: 0x00021074
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.battery != null)
		{
			this.battery.onHasAnyChanged -= this.OnBatteryHasAnyChanged;
			this.battery.onHasAnyChanged += this.OnBatteryHasAnyChanged;
			this.OnBatteryHasAnyChanged();
		}
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00022EC9 File Offset: 0x000210C9
	public override void OnDisable()
	{
		base.OnDisable();
		this.battery.onHasAnyChanged -= this.OnBatteryHasAnyChanged;
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00022EE8 File Offset: 0x000210E8
	static DroneAnimator()
	{
		DroneAnimator.ANIMATION_DICT = new Dictionary<DroneAnimator.Id, int>(DroneAnimator.IdComparer.Instance);
		foreach (DroneAnimator.Id id in Enum.GetValues(typeof(DroneAnimator.Id)).Cast<DroneAnimator.Id>())
		{
			DroneAnimator.ANIMATION_DICT.Add(id, Animator.StringToHash(Enum.GetName(typeof(DroneAnimator.Id), id)));
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00022F80 File Offset: 0x00021180
	public void SetAnimation(DroneAnimator.Id id)
	{
		this.onStateExit.Clear();
		if (base.animator.isInitialized)
		{
			foreach (KeyValuePair<DroneAnimator.Id, int> keyValuePair in DroneAnimator.ANIMATION_DICT)
			{
				base.animator.SetBool(keyValuePair.Value, keyValuePair.Key == id);
			}
		}
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x00023000 File Offset: 0x00021200
	private void OnBatteryHasAnyChanged()
	{
		base.animator.SetBool(DroneAnimator.HAS_BATTERY, this.battery.HasAny());
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000684 RID: 1668 RVA: 0x0002301D File Offset: 0x0002121D
	private DroneStationBattery battery
	{
		get
		{
			return base.parent.station.battery;
		}
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00023030 File Offset: 0x00021230
	public void OnStateExit(DroneAnimatorState.Id id, Action callback)
	{
		if (this.onStateExit.ContainsKey(id))
		{
			Dictionary<DroneAnimatorState.Id, Action> dictionary = this.onStateExit;
			dictionary[id] = (Action)Delegate.Combine(dictionary[id], callback);
			return;
		}
		this.onStateExit[id] = callback;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0002307C File Offset: 0x0002127C
	public void OnStateExit(DroneAnimatorState.Id id)
	{
		Action action;
		if (this.onStateExit.TryGetValue(id, out action) && action != null)
		{
			action();
			this.onStateExit[id] = null;
		}
	}

	// Token: 0x0400060F RID: 1551
	private static Dictionary<DroneAnimator.Id, int> ANIMATION_DICT;

	// Token: 0x04000610 RID: 1552
	private static readonly int HAS_BATTERY = Animator.StringToHash("HAS_BATTERY");

	// Token: 0x04000611 RID: 1553
	private Dictionary<DroneAnimatorState.Id, Action> onStateExit = new Dictionary<DroneAnimatorState.Id, Action>(DroneAnimatorState.IdComparer.Instance);

	// Token: 0x0200012A RID: 298
	public enum Id
	{
		// Token: 0x04000613 RID: 1555
		IDLE,
		// Token: 0x04000614 RID: 1556
		MOVE,
		// Token: 0x04000615 RID: 1557
		GATHER,
		// Token: 0x04000616 RID: 1558
		DEPOSIT,
		// Token: 0x04000617 RID: 1559
		REST,
		// Token: 0x04000618 RID: 1560
		IDLE_CELEBRATE,
		// Token: 0x04000619 RID: 1561
		IDLE_GRUMP
	}

	// Token: 0x0200012B RID: 299
	public class IdComparer : IEqualityComparer<DroneAnimator.Id>
	{
		// Token: 0x06000688 RID: 1672 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(DroneAnimator.Id a, DroneAnimator.Id b)
		{
			return a == b;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(DroneAnimator.Id a)
		{
			return (int)a;
		}

		// Token: 0x0400061A RID: 1562
		public static DroneAnimator.IdComparer Instance = new DroneAnimator.IdComparer();
	}
}
