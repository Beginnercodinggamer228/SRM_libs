using System;
using System.Collections.Generic;

namespace Sentry
{
	// Token: 0x020008A8 RID: 2216
	public class SentryExceptionEvent : SentryEvent
	{
		// Token: 0x06003054 RID: 12372 RVA: 0x000BE6FB File Offset: 0x000BC8FB
		public SentryExceptionEvent(string exceptionType, string exceptionValue, List<Breadcrumb> breadcrumbs, List<StackTraceSpec> stackTrace) : base(exceptionType, breadcrumbs)
		{
			this.exception = new ExceptionContainer(new List<ExceptionSpec>
			{
				new ExceptionSpec(exceptionType, exceptionValue, stackTrace)
			});
		}

		// Token: 0x04002E75 RID: 11893
		public ExceptionContainer exception;
	}
}
