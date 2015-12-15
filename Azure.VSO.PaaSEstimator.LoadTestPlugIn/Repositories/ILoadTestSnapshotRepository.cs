using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories
{
    public interface ILoadTestSnapshotRepository
    {
        void AddLoadTestSnapshot(LoadTestSnapShot loadTestSnapshot);
    }
}
