using System;
using System.Collections.Generic;

namespace Sentry
{
	// Token: 0x020008A7 RID: 2215
	[Serializable]
	public class ExceptionContainer
	{
		// Token: 0x06003053 RID: 12371 RVA: 0x000BE6EC File Offset: 0x000BC8EC
		public ExceptionContainer(List<ExceptionSpec> arg)
		{
			this.values = arg;
		}

		// Token: 0x04002E74 RID: 11892
		public List<ExceptionSpec> values;
	}
}
