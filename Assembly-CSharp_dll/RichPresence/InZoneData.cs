using System;

namespace RichPresence
{
	// Token: 0x02000A27 RID: 2599
	public struct InZoneData
	{
		// Token: 0x060045F2 RID: 17906 RVA: 0x000CDF5C File Offset: 0x000CC15C
		public InZoneData(ZoneDirector.Zone zone)
		{
			this.zone = zone;
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x000CDF65 File Offset: 0x000CC165
		public override string ToString()
		{
			return string.Format("{0} [zone={1}]", base.GetType().Name, this.zone);
		}

		// Token: 0x040033D2 RID: 13266
		public ZoneDirector.Zone zone;
	}
}
