using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Control.FusionTasksTests
{
	[TestClass]
	public class TaskExtensionsTests
	{
		[TestMethod]
		public void TaskAsAsyncTest()
		{
			var task = Task.Delay(500);
			var asy = task.AsAsync();
			FSharpAsync.RunSynchronously(asy, FSharpOption<int>.None, FSharpOption<CancellationToken>.None);
		}
	}
}
