using System;
using System.Collections.Generic;

namespace Sentry
{
	// Token: 0x020008A6 RID: 2214
	[Serializable]
	public class ExceptionSpec
	{
		// Token: 0x06003052 RID: 12370 RVA: 0x000BE6CA File Offset: 0x000BC8CA
		public ExceptionSpec(string type, string value, List<StackTraceSpec> stacktrace)
		{
			this.type = type;
			this.value = value;
			this.stacktrace = new StackTraceContainer(stacktrace);
		}

		// Token: 0x04002E71 RID: 11889
		public string type;

		// Token: 0x04002E72 RID: 11890
		public string value;

		// Token: 0x04002E73 RID: 11891
		public StackTraceContainer stacktrace;
	}
}
