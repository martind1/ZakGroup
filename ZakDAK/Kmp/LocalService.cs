using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using Radzen.Blazor;
using System.Security.Policy;
using ZakDAK.Data;
using ZakDAK.Entities.DPE;

namespace ZakDAK.Kmp
{

    //Der lokale Navigator
    public class LocalService<TItem>
    {
        //todo: nach NLnk - NavigatorLink - LinkService
        //Idee: hier auch individueller MaxRecordCount
        private ColumnList _columnlist;
        public ColumnList Columnlist { get => _columnlist ??= LoadColumnlist(); set => _columnlist = value; }
        private string _keyfields;
        public string KeyFields { get => _keyfields ??= LoadKeyFields(); set => _keyfields = value; }
        private FltrList _fltrlist;
        public FltrList Fltrlist { get => _fltrlist ??= LoadFltrlist(); set => _fltrlist = value; }
        public FltrList References { get; set; }  //nicht durch User änderbar

        public string OrderBy { get; set; }  //von Radzen.LoadDataArgs


        DpeData data;
        private FLTR _fltrRec;  //Datensatz aus Tabelle 'FLTR'
        public FLTR FltrRec { get => _fltrRec ??= data.GetFltr(FormKurz, Abfrage); }


        private string _abfrage;
        public string Abfrage { get => _abfrage; set => SetAbfrage(value); }
        public void SetAbfrage(string value)
        {
            if ((_abfrage ?? string.Empty) != (value ?? string.Empty)) { 
                _abfrage = value;
                _fltrRec = null; //neu Laden von Abfrage
                _columnlist = null; //neu Laden von Abfrage
                _keyfields = null; //neu Laden von Abfrage
                _fltrlist = null; //neu Laden von AbfrageLoadFltrlist();
            }
        }
        private string _formKurz;
        public string FormKurz { get => _formKurz; set => SetFormKurz(value); }
        public void SetFormKurz(string value)
        {
            if ((_formKurz ?? string.Empty) != (value ?? string.Empty))
            {
                _formKurz = value;
                _fltrRec = null; //neu Laden von Abfrage
                _columnlist = null; //neu Laden von Abfrage
                _keyfields = null; //neu Laden von Abfrage
                _fltrlist = null; //neu Laden von AbfrageLoadFltrlist();
            }
        }

        RadzenDataGrid<TItem> grid;

        public LocalService()
        {
            
        }

        public LocalService(RadzenDataGrid<TItem> grid, DpeData data, string formKurz, string abfrage): this()
        {
            //todo: an GNav weiterleiten
            this.grid = grid;
            this.data = data;

            this._formKurz = formKurz;
            //this.Abfrage = abfrage;  //mit SetAbfrage
            this._abfrage = abfrage;  //ohne SetAbfrage

            Type myType = typeof(TItem);

            //References = LoadReferences(); * in Seite
        }

#region ColumnList

        public ColumnList LoadColumnlist()
        {
            //Test: statische Liste
            //todo: von Abfrage laden
            //string cl = 
//@"Quittung:5=HOFL_KTRL
//sta:0=sta
//edt:0=edt
//Lieferart:10=lityp
//Ein:5=ETm
//Fahrzeug:11=fahr_knz
//Beförderer:23=anl_na1
//Sorte Bez.:21=srte_bez
//Tara:8=tagew
//erz_na1:13=erz_na1
//erz_na2:14=erz_na2
//erz_str:15=erz_str";
            //return new ColumnList(cl);
            var Result = FltrRec.Columnlist;
            return Result;
        }

        #endregion

        #region KeyFields

        //KeyFields von Abfrage laden und nach Columnlist.Sortorder übertragenb
        public string LoadKeyFields()
        {
            //todo: von Abfrage laden
            //todo: Groß/Kleinschreibung prüfen, anpassen oder Fehler wenn nicht gefunden
            //string kf = "edt;ETm desc";
            string kf = FltrRec.KEYFIELDS;

            string[] keyfields = kf.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            IDictionary<string, string> fields = new Dictionary<string, string>();

            foreach (var keyfield in keyfields)
            {
                var key = keyfield.Split(' ');
                fields.Add(key[0], key.Length >= 2 ? key[1] : "asc");
            }

            //nach Columnlist.Sortorder übertragen:
            foreach (var key in this.Columnlist.Columns.Keys)
            {
                if (fields.TryGetValue(key, out string keyvalue))
                    Columnlist.Columns[key].SortOrder = keyvalue.ToLower() == "asc" ? Radzen.SortOrder.Ascending : Radzen.SortOrder.Descending;
                else
                    Columnlist.Columns[key].SortOrder = null;
            }

            return kf;
        }

        #endregion

        #region FltrList und References

        public FltrList LoadFltrlist()
        {
            //Test: statische Liste
            //todo: von Abfrage laden: LookUp FLTR[formKurz, Abfrage].FltrList
            //string fl = "lityp=B;A\r\nlort_nr=57";
            //return new FltrList(fl);
            var Result = FltrRec.Fltrlist;
            return Result;
        }

        public FltrList LoadReferences()
        {
            //wird nicht aufgerufen
            //In Razor Seite: lnav.References = new FltrList("lityp=<>X");
            string re = "lityp=<>X";
            return new FltrList(re);
        }

        //SQL Where Caluse generieren: Für Radzen Query
        private string _filter;
        public string Filter { get => _filter; set => _filter = value; }   
        public string GetFilter()
        {
            IList<string> FL = DposUtils.GetFieldlist(typeof(TItem));

            //Generate SQL: anhand FltrList und References:
            _filter = "(lityp=\"B\" or lityp=\"A\") and (lort_nr=\"57\") and (sta=\"H\")";
            return _filter;
        }

        #endregion

    }
}
