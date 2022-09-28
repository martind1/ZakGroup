using System;
using System.Collections.Generic;
using ZakDAK.Kmp;

namespace ZakDAK.Entities.DPE
{
    public partial class FLTR
    {
        //berechnete Eigenschaften
        //keine Spaltennamen (auch nicht als Lowercase)! Deshalb Prefix 'cf':
        //keine Setter!
        public ColumnList cfColumnlist { get => new(COLUMNLIST); }

        public FltrList cfFltrlist { get => new(FLTRLIST); }
    }
}
