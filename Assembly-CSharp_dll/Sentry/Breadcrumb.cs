using System;
using System.Collections.Generic;

namespace Sentry
{
	// Token: 0x020008A9 RID: 2217
	[Serializable]
	public class Breadcrumb
	{
		// Token: 0x06003055 RID: 12373 RVA: 0x000BE724 File Offset: 0x000BC924
		public Breadcrumb(string timestamp, string message)
		{
			this.timestamp = timestamp;
			this.message = message;
		}

		// Token: 0x06003056 RID: 12374 RVA: 0x000BE73C File Offset: 0x000BC93C
		public static List<Breadcrumb> CombineBreadcrumbs(Breadcrumb[] breadcrumbs, int index, int number)
		{
			List<Breadcrumb> list = new List<Breadcrumb>(number);
			int num = (index + 100 - number) % 100;
			for (int i = 0; i < number; i++)
			{
				list.Add(breadcrumbs[(i + num) % 100]);
			}
			return list;
		}

		// Token: 0x04002E76 RID: 11894
		public const int MaxBreadcrumbs = 100;

		// Token: 0x04002E77 RID: 11895
		public string timestamp;

		// Token: 0x04002E78 RID: 11896
		public string message;
	}
}
