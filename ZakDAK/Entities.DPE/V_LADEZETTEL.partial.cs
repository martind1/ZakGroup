using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ZakDAK.Kmp;

namespace ZakDAK.Entities.DPE
{
    public partial class V_LADEZETTEL
    {
        //berechnete Eigenschaften
        [NotMapped]
        public bool? cfHOFL_OK
        {
            get => HOFL_OK == "J" ? true : HOFL_OK == "N" ? false : null;
            //set { HOFL_OK = value == null ? null : value == true ? "J" : "N"; } }
            set { }
        }
    }
}
