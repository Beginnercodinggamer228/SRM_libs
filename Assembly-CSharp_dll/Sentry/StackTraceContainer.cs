using System;
using System.Collections.Generic;

namespace Sentry
{
	// Token: 0x020008A4 RID: 2212
	[Serializable]
	public class StackTraceContainer
	{
		// Token: 0x06003050 RID: 12368 RVA: 0x000BE68B File Offset: 0x000BC88B
		public StackTraceContainer(List<StackTraceSpec> frames)
		{
			this.frames = frames;
		}

		// Token: 0x04002E6B RID: 11883
		public List<StackTraceSpec> frames;
	}
}
