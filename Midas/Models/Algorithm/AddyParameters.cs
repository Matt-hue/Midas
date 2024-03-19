namespace Midas.Models.Algorithm
{
    public class AddyParameters
    {
        public double RatioPercentageUnderDynamicAvg { get; set; } = -0.002; //-0.2%
        public double PointsInARowBelowAveragePriorToPurchase { get; set; } = 1;
        public int WorkdaysThatDetermineDynamicAverage { get; set; } = 10;
        public double SalesPriceIncleaseVsPurchasePrice { get; set; } = 0.003;
        public double MaxWaitingPeriodInWorkdays { get; set; } = 10;
    }
}
