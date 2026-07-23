using System;
using System.Collections.Generic;

namespace RichPresence
{
	// Token: 0x02000A2A RID: 2602
	public class Director
	{
		// Token: 0x060045FA RID: 17914 RVA: 0x000CDF9D File Offset: 0x000CC19D
		public void Register(Handler handler)
		{
			this.handlers.Add(handler);
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x000CDFAB File Offset: 0x000CC1AB
		public void Deregister(Handler handler)
		{
			this.handlers.Remove(handler);
		}

		// Token: 0x060045FC RID: 17916 RVA: 0x000CDFBC File Offset: 0x000CC1BC
		public void SetRichPresence(MainMenuData data)
		{
			foreach (Handler handler in this.handlers)
			{
				handler.SetRichPresence(data);
			}
		}

		// Token: 0x060045FD RID: 17917 RVA: 0x000CE010 File Offset: 0x000CC210
		public void SetRichPresence(InZoneData data)
		{
			foreach (Handler handler in this.handlers)
			{
				handler.SetRichPresence(data);
			}
		}

		// Token: 0x060045FE RID: 17918 RVA: 0x000CE064 File Offset: 0x000CC264
		public static bool TryGetZoneId(ZoneDirector.Zone zone, out string id)
		{
			return Director.RICH_PRESENCE_ZONE_LOOKUP.TryGetValue(zone, out id);
		}

		// Token: 0x040033D4 RID: 13268
		private List<Handler> handlers = new List<Handler>();

		// Token: 0x040033D5 RID: 13269
		private static Dictionary<ZoneDirector.Zone, string> RICH_PRESENCE_ZONE_LOOKUP = new Dictionary<ZoneDirector.Zone, string>(ZoneDirector.zoneComparer)
		{
			{
				ZoneDirector.Zone.DESERT,
				"desert"
			},
			{
				ZoneDirector.Zone.MOCHI_RANCH,
				"mochis_ranch"
			},
			{
				ZoneDirector.Zone.MOSS,
				"moss_blanket"
			},
			{
				ZoneDirector.Zone.OGDEN_RANCH,
				"ogdens_ranch"
			},
			{
				ZoneDirector.Zone.QUARRY,
				"quarry"
			},
			{
				ZoneDirector.Zone.RANCH,
				"ranch"
			},
			{
				ZoneDirector.Zone.REEF,
				"reef"
			},
			{
				ZoneDirector.Zone.RUINS,
				"ruins"
			},
			{
				ZoneDirector.Zone.SLIMULATIONS,
				"slimulations"
			},
			{
				ZoneDirector.Zone.VALLEY,
				"nimble_valley"
			},
			{
				ZoneDirector.Zone.VIKTOR_LAB,
				"viktors_lab"
			},
			{
				ZoneDirector.Zone.WILDS,
				"wilds"
			}
		};
	}
}
