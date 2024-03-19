namespace Midas.Models
{
    public static class MidasParameters
    {
        public static double RatioPercentageUnderDynamicAvg { get; set; } = -0.002; //-0.2%
        public static double PointsInARowBelowAveragePriorToPurchase { get; set; } = 1;
        public static int WorkdaysThatDetermineDynamicAverage { get; set; } = 10;
        public static double SalesPriceIncleaseVsPurchasePrice { get; set; } = 0.003;
        public static double MaxWaitingPeriodInWorkdays { get; set; } = 10;
    }
}
