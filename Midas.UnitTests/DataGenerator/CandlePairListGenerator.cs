using Midas.Data.Entities;
using Midas.Models;
using Midas.Models.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas.UnitTests.DataGenerator
{
    public static class CandlePairListGenerator
    {
        public static LinkedList<CandlePair> CreateLinkedList(int size)
        {

            var ll = new LinkedList<CandlePair>();

            var datum = DateTime.UtcNow.AddDays(-10);

            for (int i = 1; i < size + 1; i++)
            {
                var c1 = new Candle () { Open = i*.2, Close = i};
                var c2 = new Candle () { Open = i*5, Close = 2 };

                var r = new CandlePair(datum.AddDays(i), c1, c2)
                {
                    FirstIndexClose = i,
                    SecondIndexClose = 2,
                };
                ll.AddLast(r);
            }

            return ll;
        }
    }
}
