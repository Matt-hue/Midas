using Midas.Models;
using Midas.Models.Algorithm;
using Midas.Utilities;

namespace Midas.Extensions
{
    public static class MovingAverageExtensions
    {
        public static IEnumerable<double> MovingAverage<T>(this IEnumerable<T> inputStream, Func<T, double> selector, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(selector(item));
                yield return ma.Current;
            }
        }

        public static IEnumerable<double> MovingAverage(this IEnumerable<double> inputStream, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(item);
                yield return ma.Current;
            }
        }

        public static double GetNodeMovingAverage(this LinkedListNode<CandlePair> node, int period)
        {
            var closeCloseRatios = new List<double>();

            int counter = period;
            var nodeTracker = node;
            while (counter > 0)
            {
                counter--;

                if (nodeTracker != null)
                {
                    closeCloseRatios.Add(nodeTracker.Value.CloseCloseRatio);
                    nodeTracker = nodeTracker.Previous;
                }
                else
                {
                    closeCloseRatios.Add(0);
                }

            }

            return closeCloseRatios.Average();
        }

        public static double GetNodeMovingAverage2(this LinkedListNode<CandlePair> node, int period)
        {
            var closeCloseRatios = new List<double>();

            int counter = period;
            var nodeTracker = node;
            while (counter > 0)
            {
                counter--;
                if (nodeTracker == null)
                    continue;

                closeCloseRatios.Add(nodeTracker.Value.CloseCloseRatio);
                nodeTracker = nodeTracker.Previous;

            }

            return closeCloseRatios.Average();
        }

        public static double GetNodeMovingAverage(this LinkedListNode<ExcelDataRow> node, int period)
        {
            var closeCloseRatios = new List<double>();

            int counter = period;
            var nodeTracker = node;
            while (counter > 0)
            {
                counter--;

                if (nodeTracker != null)
                {
                    closeCloseRatios.Add(nodeTracker.Value.CloseCloseRatio);
                    nodeTracker = nodeTracker.Previous;
                }
                else
                {
                    closeCloseRatios.Add(0);
                }

            }

            return closeCloseRatios.Average();
        }
    }
}
