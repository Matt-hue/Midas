using Midas.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midas.UnitTests.DataGenerators
{
    public class AexDjLinkedLists : IEnumerable<ExcelDataRow>
    {
        public IEnumerator<ExcelDataRow> GetEnumerator()
        {
            yield return new ExcelDataRow();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
