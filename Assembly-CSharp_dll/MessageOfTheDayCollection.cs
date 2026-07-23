using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000041 RID: 65
[CreateAssetMenu(menuName = "Services/MessageOfTheDayCollection")]
[Serializable]
public class MessageOfTheDayCollection : ScriptableObject
{
	// Token: 0x0600010E RID: 270 RVA: 0x00009D09 File Offset: 0x00007F09
	public BundledMessageOfTheDay GetRandomMessage()
	{
		return Randoms.SHARED.Pick<BundledMessageOfTheDay>(this.messages, null);
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00009D1C File Offset: 0x00007F1C
	public BundledMessageOfTheDay GetRandomMessage(Predicate<BundledMessageOfTheDay> messageFilter)
	{
		return Randoms.SHARED.Pick<BundledMessageOfTheDay>(from msg in this.messages
		where messageFilter(msg)
		select msg, null);
	}

	// Token: 0x04000168 RID: 360
	public List<BundledMessageOfTheDay> messages;
}
