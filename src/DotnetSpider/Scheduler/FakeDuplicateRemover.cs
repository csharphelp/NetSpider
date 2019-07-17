using Sop.Spider.DataFlow;
using Sop.Spider.Downloader;
using Sop.Spider.Scheduler.Component;

namespace Sop.Spider.Scheduler
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