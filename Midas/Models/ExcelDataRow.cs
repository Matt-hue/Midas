namespace Midas.Models
{
    public class ExcelDataRow
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }

        public double AexOpen { get; set; }
        public double AexClose { get; set; }

        public double DjOpen { get; set; }
        public double DjClose { get; set; }

        public double EurUsdOpen { get; set; }
        public double EurUsdClose { get; set; }

        public double CloseCloseRatio { get => AexClose / DjClose; }
        public double CorrectedCloseCloseRatio { get => AexClose/DjClose/EurUsdClose; }

        public double DynamicRangeRow { get; set; }

        public double DynamicRangeUpperRow { get => DynamicRangeRow + -1 + MidasParameters.WorkdaysThatDetermineDynamicAverage; }

        public double DynamicAverage0 { get; set; }
        public double DynamicAverage { get; set; }
        public double DeltaVsDynamicAverage { get => (CloseCloseRatio-DynamicAverage)/DynamicAverage; }
        public int? PointsBelowTrigger { get; set; }
        public int? PurchaseTrigger { get; set; }
        public double? PurchasePriceRefPoint { get; set; }
        public double? RefPrice { get; set; }
        public int WorkDayCounter { get; set; }
        public int? PurchaseDone { get; set; }
        public int? SaleDone { get; set; }
        public double? MetGoalInTime { get; set; }
        public int? DidNotMeetGoal { get; set; }
        public double TotalProceeds { get; set; }
    }
}
