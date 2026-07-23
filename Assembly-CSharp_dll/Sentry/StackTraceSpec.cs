using System;

namespace Sentry
{
	// Token: 0x020008A5 RID: 2213
	[Serializable]
	public class StackTraceSpec
	{
		// Token: 0x06003051 RID: 12369 RVA: 0x000BE69A File Offset: 0x000BC89A
		public StackTraceSpec(string filename, string function, int lineNo, bool inApp)
		{
			this.filename = filename;
			this.function = function;
			this.lineno = lineNo;
			this.in_app = inApp;
		}

		// Token: 0x04002E6C RID: 11884
		public string filename;

		// Token: 0x04002E6D RID: 11885
		public string function;

		// Token: 0x04002E6E RID: 11886
		public string module = "";

		// Token: 0x04002E6F RID: 11887
		public int lineno;

		// Token: 0x04002E70 RID: 11888
		public bool in_app;
	}
}
