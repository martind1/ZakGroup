using System;
using System.Collections.Generic;
using ZakDAK.Kmp;

namespace ZakDAK.Entities.DPE
{
    public partial class FLTR
    {
        //berechnete Eigenschaften
        public ColumnList Columnlist { get => new(COLUMNLIST); }

        public FltrList Fltrlist { get => new(FLTRLIST); }
    }
}
