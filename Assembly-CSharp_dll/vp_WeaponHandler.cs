using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000895 RID: 2197
public class vp_WeaponHandler : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06002FFD RID: 12285 RVA: 0x000BD027 File Offset: 0x000BB227
	// (set) Token: 0x06002FFE RID: 12286 RVA: 0x000BD03D File Offset: 0x000BB23D
	public List<vp_Weapon> Weapons
	{
		get
		{
			if (this.m_Weapons == null)
			{
				this.InitWeaponLists();
			}
			return this.m_Weapons;
		}
		set
		{
			this.m_Weapons = value;
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06002FFF RID: 12287 RVA: 0x000BD046 File Offset: 0x000BB246
	public vp_Weapon CurrentWeapon
	{
		get
		{
			return this.m_CurrentWeapon;
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06003000 RID: 12288 RVA: 0x000BD04E File Offset: 0x000BB24E
	[Obsolete("Please use the 'CurrentWeaponIndex' parameter instead.")]
	public int CurrentWeaponID
	{
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06003001 RID: 12289 RVA: 0x000BD04E File Offset: 0x000BB24E
	public int CurrentWeaponIndex
	{
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x06003002 RID: 12290 RVA: 0x000BD058 File Offset: 0x000BB258
	public vp_Weapon WeaponBeingSet
	{
		get
		{
			if (!this.m_Player.SetWeapon.Active)
			{
				return null;
			}
			if (this.m_Player.SetWeapon.Argument == null)
			{
				return null;
			}
			return this.Weapons[Mathf.Max(0, (int)this.m_Player.SetWeapon.Argument - 1)];
		}
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x000BD0B8 File Offset: 0x000BB2B8
	protected virtual void Awake()
	{
		this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
		if (this.Weapons != null)
		{
			this.StartWeapon = Mathf.Clamp(this.StartWeapon, 0, this.Weapons.Count);
		}
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x000BD110 File Offset: 0x000BB310
	protected void InitWeaponLists()
	{
		List<vp_Weapon> list = null;
		vp_FPCamera componentInChildren = base.transform.GetComponentInChildren<vp_FPCamera>();
		if (componentInChildren != null)
		{
			list = this.GetWeaponList(Camera.main.transform);
			if (list != null && list.Count > 0)
			{
				this.m_WeaponLists.Add(list);
			}
		}
		List<vp_Weapon> list2 = new List<vp_Weapon>(base.transform.GetComponentsInChildren<vp_Weapon>());
		if (list != null && list.Count == list2.Count)
		{
			this.Weapons = this.m_WeaponLists[0];
			return;
		}
		List<Transform> list3 = new List<Transform>();
		foreach (vp_Weapon vp_Weapon in list2)
		{
			if ((!(componentInChildren != null) || !list.Contains(vp_Weapon)) && !list3.Contains(vp_Weapon.Parent))
			{
				list3.Add(vp_Weapon.Parent);
			}
		}
		foreach (Transform target in list3)
		{
			List<vp_Weapon> weaponList = this.GetWeaponList(target);
			this.DeactivateAll(weaponList);
			this.m_WeaponLists.Add(weaponList);
		}
		if (this.m_WeaponLists.Count < 1)
		{
			Debug.LogError("Error (" + this + ") WeaponHandler found no weapons in its hierarchy. Disabling self.");
			base.enabled = false;
			return;
		}
		this.Weapons = this.m_WeaponLists[0];
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x000BD298 File Offset: 0x000BB498
	public void EnableWeaponList(int index)
	{
		if (this.m_WeaponLists == null)
		{
			return;
		}
		if (this.m_WeaponLists.Count < 1)
		{
			return;
		}
		if (index < 0 || index > this.m_WeaponLists.Count - 1)
		{
			return;
		}
		this.Weapons = this.m_WeaponLists[index];
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x000BD2E4 File Offset: 0x000BB4E4
	protected List<vp_Weapon> GetWeaponList(Transform target)
	{
		List<vp_Weapon> list = new List<vp_Weapon>();
		if (target.GetComponent<vp_Weapon>())
		{
			Debug.LogError("Error: (" + this + ") Hierarchy error. This component should sit above any vp_Weapons in the gameobject hierarchy.");
			return list;
		}
		foreach (vp_Weapon item in target.GetComponentsInChildren<vp_Weapon>(true))
		{
			list.Insert(list.Count, item);
		}
		if (list.Count == 0)
		{
			Debug.LogError("Error: (" + this + ") Hierarchy error. This component must be added to a gameobject with vp_Weapon components in child gameobjects.");
			return list;
		}
		IComparer @object = new vp_WeaponHandler.WeaponComparer();
		list.Sort(new Comparison<vp_Weapon>(@object.Compare));
		return list;
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x000BD37D File Offset: 0x000BB57D
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x000BD399 File Offset: 0x000BB599
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x000BD3B5 File Offset: 0x000BB5B5
	protected virtual void Update()
	{
		this.InitWeapon();
		this.UpdateFiring();
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x000BD3C4 File Offset: 0x000BB5C4
	protected virtual void UpdateFiring()
	{
		if (!this.m_Player.IsLocal.Get() && !this.m_Player.IsAI.Get())
		{
			return;
		}
		if (!this.m_Player.Attack.Active)
		{
			return;
		}
		if (this.m_Player.SetWeapon.Active || (this.m_CurrentWeapon != null && !this.m_CurrentWeapon.Wielded))
		{
			return;
		}
		this.m_Player.Fire.Try();
	}

	// Token: 0x0600300B RID: 12299 RVA: 0x000BD458 File Offset: 0x000BB658
	public virtual void SetWeapon(int weaponIndex)
	{
		if (this.Weapons == null || this.Weapons.Count < 1)
		{
			Debug.LogError("Error: (" + this + ") Tried to set weapon with an empty weapon list.");
			return;
		}
		if (weaponIndex < 0 || weaponIndex > this.Weapons.Count)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				") Weapon list does not have a weapon with index: ",
				weaponIndex
			}));
			return;
		}
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.ResetState();
		}
		this.DeactivateAll(this.Weapons);
		this.ActivateWeapon(weaponIndex);
	}

	// Token: 0x0600300C RID: 12300 RVA: 0x000BD500 File Offset: 0x000BB700
	public void DeactivateAll(List<vp_Weapon> weaponList)
	{
		foreach (vp_Weapon vp_Weapon in weaponList)
		{
			vp_Weapon.ActivateGameObject(false);
			vp_FPWeapon vp_FPWeapon = vp_Weapon as vp_FPWeapon;
			if (vp_FPWeapon != null && vp_FPWeapon.Weapon3rdPersonModel != null)
			{
				vp_Utility.Activate(vp_FPWeapon.Weapon3rdPersonModel, false);
			}
		}
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x000BD578 File Offset: 0x000BB778
	public void ActivateWeapon(int index)
	{
		this.m_CurrentWeaponIndex = index;
		this.m_CurrentWeapon = null;
		if (this.m_CurrentWeaponIndex > 0)
		{
			this.m_CurrentWeapon = this.Weapons[this.m_CurrentWeaponIndex - 1];
			if (this.m_CurrentWeapon != null)
			{
				this.m_CurrentWeapon.ActivateGameObject(true);
			}
		}
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x000BD5CF File Offset: 0x000BB7CF
	public virtual void CancelTimers()
	{
		vp_Timer.CancelAll("EjectShell");
		this.m_DisableAttackStateTimer.Cancel();
		this.m_SetWeaponTimer.Cancel();
		this.m_SetWeaponRefreshTimer.Cancel();
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x000BD5FC File Offset: 0x000BB7FC
	public virtual void SetWeaponLayer(int layer)
	{
		if (this.m_CurrentWeaponIndex < 1 || this.m_CurrentWeaponIndex > this.Weapons.Count)
		{
			return;
		}
		vp_Layer.Set(this.Weapons[this.m_CurrentWeaponIndex - 1].gameObject, layer, true);
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x000BD63A File Offset: 0x000BB83A
	private void InitWeapon()
	{
		if (this.m_CurrentWeaponIndex == -1)
		{
			this.SetWeapon(0);
			vp_Timer.In(this.SetWeaponDuration + 0.1f, delegate()
			{
				if (this.StartWeapon > 0 && this.StartWeapon < this.Weapons.Count + 1 && !this.m_Player.SetWeapon.TryStart<int>(this.StartWeapon))
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Warning (",
						this,
						") Requested 'StartWeapon' (",
						this.Weapons[this.StartWeapon - 1].name,
						") was denied, likely by the inventory. Make sure it's present in the inventory from the beginning."
					}));
				}
			}, null);
		}
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x000BD66C File Offset: 0x000BB86C
	public void RefreshAllWeapons()
	{
		foreach (vp_Weapon vp_Weapon in this.Weapons)
		{
			vp_Weapon.Refresh();
			vp_Weapon.RefreshWeaponModel();
		}
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000BD6C4 File Offset: 0x000BB8C4
	public int GetWeaponIndex(vp_Weapon weapon)
	{
		return this.Weapons.IndexOf(weapon) + 1;
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000BD6D4 File Offset: 0x000BB8D4
	protected virtual void OnStart_Reload()
	{
		this.m_Player.Attack.Stop(this.m_Player.CurrentWeaponReloadDuration.Get() + this.ReloadAttackSleepDuration);
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x000BD704 File Offset: 0x000BB904
	protected virtual void OnStart_SetWeapon()
	{
		this.CancelTimers();
		if (this.WeaponBeingSet == null || this.WeaponBeingSet.AnimationType != 2)
		{
			this.m_Player.Reload.Stop(this.SetWeaponDuration + this.SetWeaponReloadSleepDuration);
			this.m_Player.Zoom.Stop(this.SetWeaponDuration + this.SetWeaponZoomSleepDuration);
			this.m_Player.Attack.Stop(this.SetWeaponDuration + this.SetWeaponAttackSleepDuration);
		}
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.Wield(false);
		}
		this.m_Player.SetWeapon.AutoDuration = this.SetWeaponDuration;
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x000BD7BC File Offset: 0x000BB9BC
	protected virtual void OnStop_SetWeapon()
	{
		int weapon = 0;
		if (this.m_Player.SetWeapon.Argument != null)
		{
			weapon = (int)this.m_Player.SetWeapon.Argument;
		}
		this.SetWeapon(weapon);
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.Wield(true);
		}
		vp_Timer.In(this.SetWeaponRefreshStatesDelay, delegate()
		{
			this.m_Player.RefreshActivityStates();
			if (this.m_CurrentWeapon != null && this.m_Player.CurrentWeaponAmmoCount.Get() == 0)
			{
				this.m_Player.AutoReload.Try();
			}
		}, this.m_SetWeaponRefreshTimer);
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x000BD834 File Offset: 0x000BBA34
	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return num != this.m_CurrentWeaponIndex && num >= 0 && num <= this.Weapons.Count && !this.m_Player.Reload.Active;
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x000BD88C File Offset: 0x000BBA8C
	protected virtual bool CanStart_Attack()
	{
		return !(this.m_CurrentWeapon == null) && !this.m_Player.Attack.Active && !this.m_Player.SetWeapon.Active && !this.m_Player.Reload.Active;
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x000BD8E6 File Offset: 0x000BBAE6
	protected virtual void OnStop_Attack()
	{
		vp_Timer.In(this.AttackStateDisableDelay, delegate()
		{
			if (!this.m_Player.Attack.Active && this.m_CurrentWeapon != null)
			{
				this.m_CurrentWeapon.SetState("Attack", false, false, false);
			}
		}, this.m_DisableAttackStateTimer);
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x000BD908 File Offset: 0x000BBB08
	protected virtual bool OnAttempt_SetPrevWeapon()
	{
		int num = this.m_CurrentWeaponIndex - 1;
		if (num < 1)
		{
			num = this.Weapons.Count;
		}
		int num2 = 0;
		while (!this.m_Player.SetWeapon.TryStart<int>(num))
		{
			num--;
			if (num < 1)
			{
				num = this.Weapons.Count;
			}
			num2++;
			if (num2 > this.Weapons.Count)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x000BD970 File Offset: 0x000BBB70
	protected virtual bool OnAttempt_SetNextWeapon()
	{
		int num = this.m_CurrentWeaponIndex + 1;
		int num2 = 0;
		while (!this.m_Player.SetWeapon.TryStart<int>(num))
		{
			if (num > this.Weapons.Count + 1)
			{
				num = 0;
			}
			num++;
			num2++;
			if (num2 > this.Weapons.Count)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x000BD9C8 File Offset: 0x000BBBC8
	protected virtual bool OnAttempt_SetWeaponByName(string name)
	{
		for (int i = 0; i < this.Weapons.Count; i++)
		{
			if (this.Weapons[i].name == name)
			{
				return this.m_Player.SetWeapon.TryStart<int>(i + 1);
			}
		}
		return false;
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x000BDA19 File Offset: 0x000BBC19
	protected virtual bool Get_CurrentWeaponWielded()
	{
		return !(this.m_CurrentWeapon == null) && this.m_CurrentWeapon.Wielded;
	}

	// Token: 0x17000353 RID: 851
	// (get) Token: 0x0600301D RID: 12317 RVA: 0x000BDA19 File Offset: 0x000BBC19
	protected virtual bool OnValue_CurrentWeaponWielded
	{
		get
		{
			return !(this.m_CurrentWeapon == null) && this.m_CurrentWeapon.Wielded;
		}
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x000BDA36 File Offset: 0x000BBC36
	protected virtual string Get_CurrentWeaponName()
	{
		if (this.m_CurrentWeapon == null || this.Weapons == null)
		{
			return "";
		}
		return this.m_CurrentWeapon.name;
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x0600301F RID: 12319 RVA: 0x000BDA36 File Offset: 0x000BBC36
	protected virtual string OnValue_CurrentWeaponName
	{
		get
		{
			if (this.m_CurrentWeapon == null || this.Weapons == null)
			{
				return "";
			}
			return this.m_CurrentWeapon.name;
		}
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x000BD04E File Offset: 0x000BB24E
	protected virtual int Get_CurrentWeaponID()
	{
		return this.m_CurrentWeaponIndex;
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06003021 RID: 12321 RVA: 0x000BD04E File Offset: 0x000BB24E
	protected virtual int OnValue_CurrentWeaponID
	{
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x000BD04E File Offset: 0x000BB24E
	protected virtual int Get_CurrentWeaponIndex()
	{
		return this.m_CurrentWeaponIndex;
	}

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06003023 RID: 12323 RVA: 0x000BD04E File Offset: 0x000BB24E
	protected virtual int OnValue_CurrentWeaponIndex
	{
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x000BDA5F File Offset: 0x000BBC5F
	public virtual int Get_CurrentWeaponType()
	{
		if (!(this.CurrentWeapon == null))
		{
			return this.CurrentWeapon.AnimationType;
		}
		return 0;
	}

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x06003025 RID: 12325 RVA: 0x000BDA5F File Offset: 0x000BBC5F
	public virtual int OnValue_CurrentWeaponType
	{
		get
		{
			if (!(this.CurrentWeapon == null))
			{
				return this.CurrentWeapon.AnimationType;
			}
			return 0;
		}
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x000BDA7C File Offset: 0x000BBC7C
	public virtual int Get_CurrentWeaponGrip()
	{
		if (!(this.CurrentWeapon == null))
		{
			return this.CurrentWeapon.AnimationGrip;
		}
		return 0;
	}

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x06003027 RID: 12327 RVA: 0x000BDA7C File Offset: 0x000BBC7C
	public virtual int OnValue_CurrentWeaponGrip
	{
		get
		{
			if (!(this.CurrentWeapon == null))
			{
				return this.CurrentWeapon.AnimationGrip;
			}
			return 0;
		}
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x000BDA9C File Offset: 0x000BBC9C
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Attack", null, new vp_Activity.Callback(this.OnStop_Attack), new vp_Activity.Condition(this.CanStart_Attack), null, null, null);
		eventHandler.RegisterActivity("SetWeapon", new vp_Activity.Callback(this.OnStart_SetWeapon), new vp_Activity.Callback(this.OnStop_SetWeapon), new vp_Activity.Condition(this.CanStart_SetWeapon), null, null, null);
		eventHandler.RegisterAttempt("SetNextWeapon", new vp_Attempt.Tryer(this.OnAttempt_SetNextWeapon));
		eventHandler.RegisterAttempt("SetPrevWeapon", new vp_Attempt.Tryer(this.OnAttempt_SetPrevWeapon));
		eventHandler.RegisterAttempt<string>("SetWeaponByName", new vp_Attempt<string>.Tryer<string>(this.OnAttempt_SetWeaponByName));
		eventHandler.RegisterActivity("Reload", new vp_Activity.Callback(this.OnStart_Reload), null, null, null, null, null);
		eventHandler.RegisterValue<int>("CurrentWeaponGrip", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponGrip), null);
		eventHandler.RegisterValue<int>("CurrentWeaponID", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponID), null);
		eventHandler.RegisterValue<int>("CurrentWeaponIndex", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponIndex), null);
		eventHandler.RegisterValue<string>("CurrentWeaponName", new vp_Value<string>.Getter<string>(this.Get_CurrentWeaponName), null);
		eventHandler.RegisterValue<int>("CurrentWeaponType", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponType), null);
		eventHandler.RegisterValue<bool>("CurrentWeaponWielded", new vp_Value<bool>.Getter<bool>(this.Get_CurrentWeaponWielded), null);
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x000BDC04 File Offset: 0x000BBE04
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Attack", null, new vp_Activity.Callback(this.OnStop_Attack), new vp_Activity.Condition(this.CanStart_Attack), null, null, null);
		eventHandler.UnregisterActivity("SetWeapon", new vp_Activity.Callback(this.OnStart_SetWeapon), new vp_Activity.Callback(this.OnStop_SetWeapon), new vp_Activity.Condition(this.CanStart_SetWeapon), null, null, null);
		eventHandler.UnregisterAttempt("SetNextWeapon", new vp_Attempt.Tryer(this.OnAttempt_SetNextWeapon));
		eventHandler.UnregisterAttempt("SetPrevWeapon", new vp_Attempt.Tryer(this.OnAttempt_SetPrevWeapon));
		eventHandler.UnregisterAttempt<string>("SetWeaponByName", new vp_Attempt<string>.Tryer<string>(this.OnAttempt_SetWeaponByName));
		eventHandler.UnregisterActivity("Reload", new vp_Activity.Callback(this.OnStart_Reload), null, null, null, null, null);
		eventHandler.UnregisterValue<int>("CurrentWeaponGrip", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponGrip), null);
		eventHandler.UnregisterValue<int>("CurrentWeaponID", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponID), null);
		eventHandler.UnregisterValue<int>("CurrentWeaponIndex", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponIndex), null);
		eventHandler.UnregisterValue<string>("CurrentWeaponName", new vp_Value<string>.Getter<string>(this.Get_CurrentWeaponName), null);
		eventHandler.UnregisterValue<int>("CurrentWeaponType", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponType), null);
		eventHandler.UnregisterValue<bool>("CurrentWeaponWielded", new vp_Value<bool>.Getter<bool>(this.Get_CurrentWeaponWielded), null);
	}

	// Token: 0x04002E13 RID: 11795
	public int StartWeapon;

	// Token: 0x04002E14 RID: 11796
	public float AttackStateDisableDelay = 0.5f;

	// Token: 0x04002E15 RID: 11797
	public float SetWeaponRefreshStatesDelay = 0.5f;

	// Token: 0x04002E16 RID: 11798
	public float SetWeaponDuration = 0.1f;

	// Token: 0x04002E17 RID: 11799
	public float SetWeaponReloadSleepDuration = 0.3f;

	// Token: 0x04002E18 RID: 11800
	public float SetWeaponZoomSleepDuration = 0.3f;

	// Token: 0x04002E19 RID: 11801
	public float SetWeaponAttackSleepDuration = 0.3f;

	// Token: 0x04002E1A RID: 11802
	public float ReloadAttackSleepDuration = 0.3f;

	// Token: 0x04002E1B RID: 11803
	public bool ReloadAutomatically = true;

	// Token: 0x04002E1C RID: 11804
	protected vp_PlayerEventHandler m_Player;

	// Token: 0x04002E1D RID: 11805
	protected List<vp_Weapon> m_Weapons;

	// Token: 0x04002E1E RID: 11806
	protected List<List<vp_Weapon>> m_WeaponLists = new List<List<vp_Weapon>>();

	// Token: 0x04002E1F RID: 11807
	protected int m_CurrentWeaponIndex = -1;

	// Token: 0x04002E20 RID: 11808
	protected vp_Weapon m_CurrentWeapon;

	// Token: 0x04002E21 RID: 11809
	protected vp_Timer.Handle m_SetWeaponTimer = new vp_Timer.Handle();

	// Token: 0x04002E22 RID: 11810
	protected vp_Timer.Handle m_SetWeaponRefreshTimer = new vp_Timer.Handle();

	// Token: 0x04002E23 RID: 11811
	protected vp_Timer.Handle m_DisableAttackStateTimer = new vp_Timer.Handle();

	// Token: 0x04002E24 RID: 11812
	protected vp_Timer.Handle m_DisableReloadStateTimer = new vp_Timer.Handle();

	// Token: 0x02000896 RID: 2198
	protected class WeaponComparer : IComparer
	{
		// Token: 0x0600302E RID: 12334 RVA: 0x000BDF28 File Offset: 0x000BC128
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((vp_Weapon)x).gameObject.name, ((vp_Weapon)y).gameObject.name);
		}
	}
}
