using System;

namespace Sentry
{
	// Token: 0x0200089D RID: 2205
	[Serializable]
	public class Device
	{
		// Token: 0x04002E3F RID: 11839
		public string name;

		// Token: 0x04002E40 RID: 11840
		public string family;

		// Token: 0x04002E41 RID: 11841
		public string model;

		// Token: 0x04002E42 RID: 11842
		public string model_id;

		// Token: 0x04002E43 RID: 11843
		public string arch;

		// Token: 0x04002E44 RID: 11844
		public string cpu_description;

		// Token: 0x04002E45 RID: 11845
		public float battery_level;

		// Token: 0x04002E46 RID: 11846
		public string battery_status;

		// Token: 0x04002E47 RID: 11847
		public string orientation;

		// Token: 0x04002E48 RID: 11848
		public bool simulator;

		// Token: 0x04002E49 RID: 11849
		public long memory_size;

		// Token: 0x04002E4A RID: 11850
		public DateTimeOffset? boot_time;

		// Token: 0x04002E4B RID: 11851
		public string timezone;

		// Token: 0x04002E4C RID: 11852
		public string device_type;
	}
}
