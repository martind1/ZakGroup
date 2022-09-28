using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System.Text.Json;
using ZakDAK.Shared;

namespace ZakDAK.Kmp
{
    /// <summary>
    /// Service für globale Navigation
    /// </summary>
    public class GlobalService : ComponentBase
    {
#region Kommando, Status
        public enum KommandoTyp
        {
            Suchen, Refresh, Pagesize, Erfass, Edit
        }
        public event Action<int> OnDoKommando;
        private void DoKommando(int KNr) => OnDoKommando?.Invoke(KNr);
        public void Kommando(KommandoTyp KTyp)
        {
            DoKommando((int)KTyp);
        }

        public enum StatusTyp
        {
            Inactive, Browse, Edit, Insert, Query
        }

#endregion

#region Grid, NavLink
        //wird nicht benötigt. Jetzt per Kommando Event
        //später: navLink. Ziel: in Page-File: Navlink nl = new NavLink('SPED'), Data von JSON-DB/R_INIT

        public LocalService<Object> lnav;
        public int RecordCount { get; set; } = 0;
        public string Pagetitle { get; set; }

        public void SetLNav<T>(Object ln)
        {
            //EntityType = typeof(T);
            lnav = (LocalService<Object>)ln;
        }
        #endregion

#region Statustext, Eventconsole
        public bool ShowEventConsole { get; set; } = true;
        #endregion

        #region pagesize
        private int _maxRecordCount = 50;
        public int MaxRecordCount { 
            get => _maxRecordCount; 
            set { _maxRecordCount = value; Kommando(Kmp.GlobalService.KommandoTyp.Pagesize); }
        }
        public IDictionary<string, int> MaxRecordCountValues = new Dictionary<string, int>()
        {
            {"50", 50 },
            {"500", 500 },
            {"5000", 5000 },
            {"(alle)", 99999999 }
        };

        #endregion

    }
}