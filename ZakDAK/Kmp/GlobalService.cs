using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System.Text.Json;
using ZakDAK.Shared;

namespace ZakDAK.Services
{
    /// <summary>
    /// Service für globale Navigation
    /// </summary>
    public class GlobalService : ComponentBase
    {
        #region Kommando, Status
        public enum KommandoTyp
        {
            Suchen, Refresh
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
        public RadzenDataGrid<Object> Grid = null;
        //private Type? EntityType = null;

        public void SetGrid<T>(Object g)
        {
            //EntityType = typeof(T);
            Grid = (RadzenDataGrid<Object>)g;
        }
        #endregion

        #region Statustext
        public string StatusText { get; set; } = "Statustext";
        public event Action OnStatusTextChange;
        private void StatusTextChanged() => OnStatusTextChange?.Invoke();

        public void SMess(String Text)
        {
            StatusText = Text;
            //Ereignis zum Anzeigen woanders auslösen:
            StatusTextChanged();
        }

        //Statuszeile mit Object->JSON
        public void SMess(object value)
        {
            SMess(JsonSerializer.Serialize(value));
        }


        //Statuszeile und Protlist
        public void SMessL(String Text, EventConsole console)
        {
            SMess(Text);
            if (console != null)
                console.Log(Text);
        }

        //Logfile und Statuszeile

        #endregion

    }
}