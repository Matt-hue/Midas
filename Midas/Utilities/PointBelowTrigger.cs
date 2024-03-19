using Midas.Models;

namespace Midas.Utilities
{
    public class PointBelowTrigger
    {
        private double _deltaVsDynamicAverage;
        public int? a()
        {
            if (_deltaVsDynamicAverage < MidasParameters.RatioPercentageUnderDynamicAvg)
            {
                if (1 == 0)
                {

                }
                return 1;
            }
            else
            {
                return null;
            }
        }
    }
}
