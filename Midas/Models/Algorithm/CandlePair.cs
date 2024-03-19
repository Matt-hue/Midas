using Midas.Data.Entities;

namespace Midas.Models.Algorithm
{
    public class CandlePair
    {
        public CandlePair(DateTime datum, Candle firstIndex, Candle second) 
        {
            Datum = datum;

            FirstIndexIndexOpen = firstIndex.Open;
            FirstIndexClose = firstIndex.Close;

            SecondIndexOpen = second.Open;
            SecondIndexClose = second.Close;
        }
        public DateTime Datum { get; set; }

        public double FirstIndexIndexOpen { get; set; }
        public double FirstIndexClose { get; set; }

        public double SecondIndexOpen { get; set; }
        public double SecondIndexClose { get; set; }

        //public double EurUsdOpen { get; set; }
        //public double EurUsdClose { get; set; }

        public double CloseCloseRatio { get => FirstIndexClose / SecondIndexClose; }
        //public double CorrectedCloseCloseRatio { get => FirstIndexClose / SecondIndexClose / EurUsdClose; }

        //public double DynamicRangeRow { get; set; }

        //public double DynamicRangeUpperRow { get => DynamicRangeRow + -1 + MidasParameters.WorkdaysThatDetermineDynamicAverage; }

        public double DynamicAverage { get; set; }
        public double DeltaVsDynamicAverage { get => (CloseCloseRatio - DynamicAverage) / DynamicAverage; }
        public int? PointsBelowTrigger { get; set; }
        public int? PurchaseTrigger { get; set; }
        public double? PurchasePriceRefPoint { get; set; }
        public double? RefPrice { get; set; }
        public int WorkDayCounter { get; set; }
        public int? PurchaseDone { get; set; }
        public int? SaleDone { get; set; }
        public double? MetGoalInTime { get; set; }
        public double? DidNotMeetGoal { get; set; }
        public double TotalProceeds { get => MetGoalInTime.GetValueOrDefault(0) + DidNotMeetGoal.GetValueOrDefault(0); }
    }
}
