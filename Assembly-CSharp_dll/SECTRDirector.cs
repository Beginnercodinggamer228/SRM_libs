using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class SECTRDirector : MonoBehaviour
{
	// Token: 0x060011E1 RID: 4577 RVA: 0x0004736E File Offset: 0x0004556E
	public void RegisterMember(SECTR_Member member)
	{
		this.RemoveMember(member);
		this._allMembers.Add(member);
		if (member.BoundsUpdateMode != SECTR_Member.BoundsUpdateModes.Static)
		{
			if (member.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Offset)
			{
				this._offsetUpdates.Add(member);
				return;
			}
			this._nonOffsetUpdates.Add(member);
		}
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x000473AE File Offset: 0x000455AE
	public void DeregisterMember(SECTR_Member member)
	{
		this._allMembers.Remove(member);
		if (member.BoundsUpdateMode != SECTR_Member.BoundsUpdateModes.Static)
		{
			if (member.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Offset)
			{
				this._offsetUpdates.Remove(member);
				return;
			}
			this._nonOffsetUpdates.Remove(member);
		}
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x000473EA File Offset: 0x000455EA
	private void RemoveMember(SECTR_Member member)
	{
		this._allMembers.Remove(member);
		this._nonOffsetUpdates.Remove(member);
		this._offsetUpdates.Remove(member);
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x00047413 File Offset: 0x00045613
	public void RegisterHibernator(SECTR_Hibernator hibernator)
	{
		this._hibernatorsToUpdate.Add(hibernator);
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x00047421 File Offset: 0x00045621
	public void DeregisterHibernator(SECTR_Hibernator hibernator)
	{
		this._hibernatorsToUpdate.Remove(hibernator);
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0004742F File Offset: 0x0004562F
	public void ClearRegistrations()
	{
		this._allMembers.Clear();
		this._nonOffsetUpdates.Clear();
		this._offsetUpdates.Clear();
		this._hibernatorsToUpdate.Clear();
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0004745D File Offset: 0x0004565D
	private void Start()
	{
		Log.Debug("Starting SECTRDirector", Array.Empty<object>());
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x00047470 File Offset: 0x00045670
	private void Update()
	{
		if (this._hibernatorsToUpdate.Data.Length > this.Update_localHibernators.Length)
		{
			Array.Resize<SECTR_Hibernator>(ref this.Update_localHibernators, Math.Max(this._hibernatorsToUpdate.Data.Length, 50));
		}
		int count = this._hibernatorsToUpdate.GetCount();
		this._hibernatorsToUpdate.Data.CopyTo(this.Update_localHibernators, 0);
		for (int i = 0; i < count; i++)
		{
			try
			{
				this.Update_localHibernators[i].OnUpdate();
			}
			catch (NullReferenceException)
			{
				Log.Debug("Null reference caught in SECTRDirector update.", new object[]
				{
					"position",
					i
				});
			}
		}
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x00047528 File Offset: 0x00045728
	private void LateUpdate()
	{
		for (int i = 0; i < this._offsetUpdates.Count; i++)
		{
			SECTR_Member sectr_Member = this._offsetUpdates[i];
			if (sectr_Member.enabled && !sectr_Member.IsHibernating && (sectr_Member.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Always || sectr_Member.memberTransform.hasChanged))
			{
				if (sectr_Member.BoundsUpdateMode != SECTR_Member.BoundsUpdateModes.Static)
				{
					sectr_Member.OffsetLateUpdate();
				}
				else
				{
					this._toRegister.Add(sectr_Member);
				}
			}
		}
		for (int j = 0; j < this._nonOffsetUpdates.Count; j++)
		{
			SECTR_Member sectr_Member = this._nonOffsetUpdates[j];
			if (sectr_Member.enabled && !sectr_Member.IsHibernating && (sectr_Member.BoundsUpdateMode == SECTR_Member.BoundsUpdateModes.Always || sectr_Member.memberTransform.hasChanged))
			{
				if (sectr_Member.BoundsUpdateMode != SECTR_Member.BoundsUpdateModes.Static)
				{
					sectr_Member.NonOffsetLateUpdate();
				}
				else
				{
					this._toRegister.Add(sectr_Member);
				}
			}
		}
		for (int k = 0; k < this._toRegister.Count; k++)
		{
			if (this._toRegister[k] != null)
			{
				this.RegisterMember(this._toRegister[k]);
			}
		}
		this._toRegister.Clear();
	}

	// Token: 0x04001107 RID: 4359
	private const int MIN_LOCAL_ARRAY_RESIZE_AMOUNT = 50;

	// Token: 0x04001108 RID: 4360
	private List<SECTR_Member> _allMembers = new List<SECTR_Member>();

	// Token: 0x04001109 RID: 4361
	private List<SECTR_Member> _offsetUpdates = new List<SECTR_Member>();

	// Token: 0x0400110A RID: 4362
	private List<SECTR_Member> _nonOffsetUpdates = new List<SECTR_Member>();

	// Token: 0x0400110B RID: 4363
	private ExposedArrayList<SECTR_Hibernator> _hibernatorsToUpdate = new ExposedArrayList<SECTR_Hibernator>(1000);

	// Token: 0x0400110C RID: 4364
	private List<SECTR_Member> _toRegister = new List<SECTR_Member>();

	// Token: 0x0400110D RID: 4365
	private SECTR_Hibernator[] Update_localHibernators = new SECTR_Hibernator[50];
}
