using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors
{
    public interface IPaasResourceProcessor
    {
        Task<string> GetPaaSResourceJson(Uri resourceUri);

        void CaptureSnapshots(Uri resourceUri, string captureEvent);
    }
}
