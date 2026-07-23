using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000877 RID: 2167
public class vp_SimpleInventory : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06002DC9 RID: 11721 RVA: 0x000AFBAA File Offset: 0x000ADDAA
	// (set) Token: 0x06002DCA RID: 11722 RVA: 0x000AFBB2 File Offset: 0x000ADDB2
	public vp_SimpleInventory.InventoryWeaponStatus CurrentWeaponStatus
	{
		get
		{
			return this.m_CurrentWeaponStatus;
		}
		set
		{
			this.m_CurrentWeaponStatus = value;
		}
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06002DCB RID: 11723 RVA: 0x000AFBBC File Offset: 0x000ADDBC
	public List<vp_SimpleInventory.InventoryItemStatus> Weapons
	{
		get
		{
			List<vp_SimpleInventory.InventoryItemStatus> list = new List<vp_SimpleInventory.InventoryItemStatus>();
			foreach (vp_SimpleInventory.InventoryItemStatus item in this.m_WeaponTypes)
			{
				list.Add(item);
			}
			return list;
		}
	}

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06002DCC RID: 11724 RVA: 0x000AFC18 File Offset: 0x000ADE18
	public List<vp_SimpleInventory.InventoryItemStatus> EquippedWeapons
	{
		get
		{
			List<vp_SimpleInventory.InventoryItemStatus> list = new List<vp_SimpleInventory.InventoryItemStatus>();
			foreach (vp_SimpleInventory.InventoryItemStatus inventoryItemStatus in this.m_ItemStatusDictionary.Values)
			{
				if (inventoryItemStatus.GetType() == typeof(vp_SimpleInventory.InventoryWeaponStatus) && inventoryItemStatus.Have == 1)
				{
					list.Add(inventoryItemStatus);
				}
			}
			return list;
		}
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x000AFC98 File Offset: 0x000ADE98
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x000AFCB4 File Offset: 0x000ADEB4
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x000AFCD0 File Offset: 0x000ADED0
	private void Awake()
	{
		this.m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		IComparer @object = new vp_SimpleInventory.InventoryWeaponStatusComparer();
		this.m_WeaponTypes.Sort(new Comparison<vp_SimpleInventory.InventoryWeaponStatus>(@object.Compare));
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06002DD0 RID: 11728 RVA: 0x000AFD20 File Offset: 0x000ADF20
	protected Dictionary<string, vp_SimpleInventory.InventoryItemStatus> ItemStatusDictionary
	{
		get
		{
			if (this.m_ItemStatusDictionary == null)
			{
				this.m_ItemStatusDictionary = new Dictionary<string, vp_SimpleInventory.InventoryItemStatus>();
				for (int i = this.m_ItemTypes.Count - 1; i > -1; i--)
				{
					if (!this.m_ItemStatusDictionary.ContainsKey(this.m_ItemTypes[i].Name))
					{
						this.m_ItemStatusDictionary.Add(this.m_ItemTypes[i].Name, this.m_ItemTypes[i]);
					}
					else
					{
						this.m_ItemTypes.Remove(this.m_ItemTypes[i]);
					}
				}
				for (int j = this.m_WeaponTypes.Count - 1; j > -1; j--)
				{
					if (!this.m_ItemStatusDictionary.ContainsKey(this.m_WeaponTypes[j].Name))
					{
						this.m_ItemStatusDictionary.Add(this.m_WeaponTypes[j].Name, this.m_WeaponTypes[j]);
					}
					else
					{
						this.m_WeaponTypes.Remove(this.m_WeaponTypes[j]);
					}
				}
			}
			return this.m_ItemStatusDictionary;
		}
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x000AFE3C File Offset: 0x000AE03C
	public bool HaveItem(object name)
	{
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		return this.ItemStatusDictionary.TryGetValue((string)name, out inventoryItemStatus) && inventoryItemStatus.Have >= 1;
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x000AFE6C File Offset: 0x000AE06C
	private vp_SimpleInventory.InventoryItemStatus GetItemStatus(string name)
	{
		vp_SimpleInventory.InventoryItemStatus result;
		if (!this.ItemStatusDictionary.TryGetValue(name, out result))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				"). Unknown item type: '",
				name,
				"'."
			}));
		}
		return result;
	}

	// Token: 0x06002DD3 RID: 11731 RVA: 0x000AFEBC File Offset: 0x000AE0BC
	private vp_SimpleInventory.InventoryWeaponStatus GetWeaponStatus(string name)
	{
		if (name == null)
		{
			return null;
		}
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(name, out inventoryItemStatus))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				"). Unknown item type: '",
				name,
				"'."
			}));
			return null;
		}
		if (inventoryItemStatus.GetType() != typeof(vp_SimpleInventory.InventoryWeaponStatus))
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				"). Item is not a weapon: '",
				name,
				"'."
			}));
			return null;
		}
		return (vp_SimpleInventory.InventoryWeaponStatus)inventoryItemStatus;
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x000AFF60 File Offset: 0x000AE160
	protected void RefreshWeaponStatus()
	{
		if (!this.m_Player.CurrentWeaponWielded.Get() && this.m_RefreshWeaponStatusIterations < 50)
		{
			this.m_RefreshWeaponStatusIterations++;
			vp_Timer.In(0.1f, new vp_Timer.Callback(this.RefreshWeaponStatus), null);
			return;
		}
		this.m_RefreshWeaponStatusIterations = 0;
		string text = this.m_Player.CurrentWeaponName.Get();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.m_CurrentWeaponStatus = this.GetWeaponStatus(text);
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x000AFFE7 File Offset: 0x000AE1E7
	protected virtual int Get_CurrentWeaponAmmoCount()
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return 0;
		}
		return this.m_CurrentWeaponStatus.LoadedAmmo;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x000AFFFE File Offset: 0x000AE1FE
	protected virtual void Set_CurrentWeaponAmmoCount(int value)
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return;
		}
		this.m_CurrentWeaponStatus.LoadedAmmo = value;
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06002DD7 RID: 11735 RVA: 0x000AFFE7 File Offset: 0x000AE1E7
	// (set) Token: 0x06002DD8 RID: 11736 RVA: 0x000AFFFE File Offset: 0x000AE1FE
	protected virtual int OnValue_CurrentWeaponAmmoCount
	{
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return 0;
			}
			return this.m_CurrentWeaponStatus.LoadedAmmo;
		}
		set
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return;
			}
			this.m_CurrentWeaponStatus.LoadedAmmo = value;
		}
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x000B0018 File Offset: 0x000AE218
	protected virtual int Get_CurrentWeaponClipCount()
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return 0;
		}
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(this.m_CurrentWeaponStatus.ClipType, out inventoryItemStatus))
		{
			return 0;
		}
		return inventoryItemStatus.Have;
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06002DDA RID: 11738 RVA: 0x000B0054 File Offset: 0x000AE254
	protected virtual int OnValue_CurrentWeaponClipCount
	{
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return 0;
			}
			vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
			if (!this.ItemStatusDictionary.TryGetValue(this.m_CurrentWeaponStatus.ClipType, out inventoryItemStatus))
			{
				return 0;
			}
			return inventoryItemStatus.Have;
		}
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000B008D File Offset: 0x000AE28D
	protected virtual string Get_CurrentWeaponClipType()
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return "";
		}
		return this.m_CurrentWeaponStatus.ClipType;
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06002DDC RID: 11740 RVA: 0x000B008D File Offset: 0x000AE28D
	protected virtual string OnValue_CurrentWeaponClipType
	{
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return "";
			}
			return this.m_CurrentWeaponStatus.ClipType;
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x000B00A8 File Offset: 0x000AE2A8
	protected virtual int OnMessage_GetItemCount(string name)
	{
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(name, out inventoryItemStatus))
		{
			return 0;
		}
		return inventoryItemStatus.Have;
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000B00CD File Offset: 0x000AE2CD
	protected virtual bool OnAttempt_DepleteAmmo()
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return false;
		}
		if (this.m_CurrentWeaponStatus.LoadedAmmo < 1)
		{
			return this.m_CurrentWeaponStatus.MaxAmmo == 0;
		}
		this.m_CurrentWeaponStatus.LoadedAmmo--;
		return true;
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x000B010C File Offset: 0x000AE30C
	protected virtual bool OnAttempt_AddAmmo(object arg)
	{
		object[] array = (object[])arg;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : -1;
		vp_SimpleInventory.InventoryWeaponStatus weaponStatus = this.GetWeaponStatus(name);
		if (weaponStatus == null)
		{
			return false;
		}
		if (num == -1)
		{
			weaponStatus.LoadedAmmo = weaponStatus.MaxAmmo;
		}
		else
		{
			weaponStatus.LoadedAmmo = Mathf.Min(weaponStatus.LoadedAmmo + num, weaponStatus.MaxAmmo);
		}
		return true;
	}

	// Token: 0x06002DE0 RID: 11744 RVA: 0x000B0174 File Offset: 0x000AE374
	protected virtual bool OnAttempt_AddItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : 1;
		vp_SimpleInventory.InventoryItemStatus itemStatus = this.GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		itemStatus.CanHave = Mathf.Max(1, itemStatus.CanHave);
		if (itemStatus.Have >= itemStatus.CanHave)
		{
			return false;
		}
		itemStatus.Have = Mathf.Min(itemStatus.Have + num, itemStatus.CanHave);
		return true;
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000B01EC File Offset: 0x000AE3EC
	protected virtual bool OnAttempt_RemoveItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : 1;
		vp_SimpleInventory.InventoryItemStatus itemStatus = this.GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		if (itemStatus.Have <= 0)
		{
			return false;
		}
		itemStatus.Have = Mathf.Max(itemStatus.Have - num, 0);
		return true;
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000B0248 File Offset: 0x000AE448
	protected virtual bool OnAttempt_RemoveClip()
	{
		return this.m_CurrentWeaponStatus != null && this.GetItemStatus(this.m_CurrentWeaponStatus.ClipType) != null && this.m_CurrentWeaponStatus.LoadedAmmo < this.m_CurrentWeaponStatus.MaxAmmo && this.m_Player.RemoveItem.Try(new object[]
		{
			this.m_CurrentWeaponStatus.ClipType
		});
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000B02BC File Offset: 0x000AE4BC
	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return num == 0 || (num >= 0 && num <= this.m_WeaponTypes.Count && this.HaveItem(this.m_WeaponTypes[num - 1].Name));
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000B0311 File Offset: 0x000AE511
	protected virtual void OnStop_SetWeapon()
	{
		this.RefreshWeaponStatus();
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000B031C File Offset: 0x000AE51C
	protected virtual void OnStart_Dead()
	{
		if (this.m_ItemStatusDictionary == null)
		{
			return;
		}
		foreach (vp_SimpleInventory.InventoryItemStatus inventoryItemStatus in this.m_ItemStatusDictionary.Values)
		{
			if (inventoryItemStatus.ClearOnDeath)
			{
				inventoryItemStatus.Have = 0;
				if (inventoryItemStatus.GetType() == typeof(vp_SimpleInventory.InventoryWeaponStatus))
				{
					((vp_SimpleInventory.InventoryWeaponStatus)inventoryItemStatus).LoadedAmmo = 0;
				}
			}
		}
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000B03A8 File Offset: 0x000AE5A8
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("SetWeapon", null, new vp_Activity.Callback(this.OnStop_SetWeapon), new vp_Activity.Condition(this.CanStart_SetWeapon), null, null, null);
		eventHandler.RegisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), null, null, null, null, null);
		eventHandler.RegisterAttempt<object>("AddAmmo", new vp_Attempt<object>.Tryer<object>(this.OnAttempt_AddAmmo));
		eventHandler.RegisterAttempt<object>("AddItem", new vp_Attempt<object>.Tryer<object>(this.OnAttempt_AddItem));
		eventHandler.RegisterAttempt("DepleteAmmo", new vp_Attempt.Tryer(this.OnAttempt_DepleteAmmo));
		eventHandler.RegisterAttempt("RemoveClip", new vp_Attempt.Tryer(this.OnAttempt_RemoveClip));
		eventHandler.RegisterAttempt<object>("RemoveItem", new vp_Attempt<object>.Tryer<object>(this.OnAttempt_RemoveItem));
		eventHandler.RegisterMessage<string, int>("GetItemCount", new vp_Message<string, int>.Sender<string, int>(this.OnMessage_GetItemCount));
		eventHandler.RegisterValue<int>("CurrentWeaponAmmoCount", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponAmmoCount), new vp_Value<int>.Setter<int>(this.Set_CurrentWeaponAmmoCount));
		eventHandler.RegisterValue<int>("CurrentWeaponClipCount", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponClipCount), null);
		eventHandler.RegisterValue<string>("CurrentWeaponClipType", new vp_Value<string>.Getter<string>(this.Get_CurrentWeaponClipType), null);
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000B04E4 File Offset: 0x000AE6E4
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("SetWeapon", null, new vp_Activity.Callback(this.OnStop_SetWeapon), new vp_Activity.Condition(this.CanStart_SetWeapon), null, null, null);
		eventHandler.UnregisterActivity("Dead", new vp_Activity.Callback(this.OnStart_Dead), null, null, null, null, null);
		eventHandler.UnregisterAttempt<object>("AddAmmo", new vp_Attempt<object>.Tryer<object>(this.OnAttempt_AddAmmo));
		eventHandler.UnregisterAttempt<object>("AddItem", new vp_Attempt<object>.Tryer<object>(this.OnAttempt_AddItem));
		eventHandler.UnregisterAttempt("DepleteAmmo", new vp_Attempt.Tryer(this.OnAttempt_DepleteAmmo));
		eventHandler.UnregisterAttempt("RemoveClip", new vp_Attempt.Tryer(this.OnAttempt_RemoveClip));
		eventHandler.UnregisterAttempt<object>("RemoveItem", new vp_Attempt<object>.Tryer<object>(this.OnAttempt_RemoveItem));
		eventHandler.UnregisterMessage<string, int>("GetItemCount", new vp_Message<string, int>.Sender<string, int>(this.OnMessage_GetItemCount));
		eventHandler.UnregisterValue<int>("CurrentWeaponAmmoCount", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponAmmoCount), new vp_Value<int>.Setter<int>(this.Set_CurrentWeaponAmmoCount));
		eventHandler.UnregisterValue<int>("CurrentWeaponClipCount", new vp_Value<int>.Getter<int>(this.Get_CurrentWeaponClipCount), null);
		eventHandler.UnregisterValue<string>("CurrentWeaponClipType", new vp_Value<string>.Getter<string>(this.Get_CurrentWeaponClipType), null);
	}

	// Token: 0x04002C04 RID: 11268
	protected vp_FPPlayerEventHandler m_Player;

	// Token: 0x04002C05 RID: 11269
	[SerializeField]
	protected List<vp_SimpleInventory.InventoryItemStatus> m_ItemTypes;

	// Token: 0x04002C06 RID: 11270
	[SerializeField]
	protected List<vp_SimpleInventory.InventoryWeaponStatus> m_WeaponTypes;

	// Token: 0x04002C07 RID: 11271
	protected Dictionary<string, vp_SimpleInventory.InventoryItemStatus> m_ItemStatusDictionary;

	// Token: 0x04002C08 RID: 11272
	protected vp_SimpleInventory.InventoryWeaponStatus m_CurrentWeaponStatus;

	// Token: 0x04002C09 RID: 11273
	protected int m_RefreshWeaponStatusIterations;

	// Token: 0x02000878 RID: 2168
	protected class InventoryWeaponStatusComparer : IComparer
	{
		// Token: 0x06002DE9 RID: 11753 RVA: 0x000B061E File Offset: 0x000AE81E
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((vp_SimpleInventory.InventoryWeaponStatus)x).Name, ((vp_SimpleInventory.InventoryWeaponStatus)y).Name);
		}
	}

	// Token: 0x02000879 RID: 2169
	[Serializable]
	public class InventoryItemStatus
	{
		// Token: 0x04002C0A RID: 11274
		public string Name = "Unnamed";

		// Token: 0x04002C0B RID: 11275
		public int Have;

		// Token: 0x04002C0C RID: 11276
		public int CanHave = 1;

		// Token: 0x04002C0D RID: 11277
		public bool ClearOnDeath = true;
	}

	// Token: 0x0200087A RID: 2170
	[Serializable]
	public class InventoryWeaponStatus : vp_SimpleInventory.InventoryItemStatus
	{
		// Token: 0x04002C0E RID: 11278
		public string ClipType = "";

		// Token: 0x04002C0F RID: 11279
		public int LoadedAmmo;

		// Token: 0x04002C10 RID: 11280
		public int MaxAmmo = 10;
	}
}
