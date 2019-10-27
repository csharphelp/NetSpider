using Sop.DotnetSpider.DataStorage;
using Sop.DotnetSpider.Download;
using Sop.DotnetSpider.Scheduler.Component;

namespace Sop.DotnetSpider.Scheduler
{
	internal class FakeDuplicateRemover : IDuplicateRemover
	{
		public void Dispose()
		{
		}

		public bool IsDuplicate(Request request)
		{
			Check.NotNull(request.OwnerId, nameof(request.OwnerId));
			return false;
		}

		public int Total => 0;

		public void ResetDuplicateCheck()
		{
		}
	}
}