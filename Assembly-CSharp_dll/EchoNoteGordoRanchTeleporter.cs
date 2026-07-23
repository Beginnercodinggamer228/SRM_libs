using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class EchoNoteGordoRanchTeleporter : SRBehaviour
{
	// Token: 0x06000A03 RID: 2563 RVA: 0x0002C22C File Offset: 0x0002A42C
	public void OnEnable()
	{
		HolidayModel holiday = SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel();
		this.ring.SetActive(SRSingleton<SceneContext>.Instance.GameModel.AllEchoNoteGordos().Any((KeyValuePair<string, EchoNoteGordoModel> pair) => pair.Value.state == EchoNoteGordoModel.State.POPPED && holiday.eventEchoNoteGordos.Any((HolidayModel.EventEchoNoteGordo e) => e.objectId == pair.Key)));
	}

	// Token: 0x04000849 RID: 2121
	[Tooltip("Parent GameObject containing the portal ring.")]
	public GameObject ring;
}
