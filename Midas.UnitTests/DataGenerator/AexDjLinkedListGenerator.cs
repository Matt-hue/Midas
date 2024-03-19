using Midas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas.UnitTests.DataGenerator
{
    public static class AexDjLinkedListGenerator
    {
        public static LinkedList<ExcelDataRow> CreateLinkedList(int size)
        {

            var ll = new LinkedList<ExcelDataRow>();

            for (int i = 1; i < size + 1; i++)
            {
                var r = new ExcelDataRow()
                {
                    Id = i,
                    AexClose = i,
                    DjClose = 2,
                };
                ll.AddLast(r);
            }

            return ll;
        }
    }
}
