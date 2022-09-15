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
        public ColumnList Columnlist {
            get { 
                _columnlist ??= LoadColumnlist();
                LoadKeyFields();  //hier wg überschreibt Columnlist
                return _columnlist;
            }
            set => _columnlist = value; 
        }
        private string _keyfields;
        public string KeyFields { get => _keyfields ??= LoadKeyFields(); set => _keyfields = value; }
        private FltrList _fltrlist;
        public FltrList Fltrlist { get => _fltrlist ??= LoadFltrlist(); set => _fltrlist = value; }
        public FltrList References { get; set; }  //nicht durch User änderbar

        public string OrderBy { get; set; }  //von Radzen.LoadDataArgs

        //Liste mit Original Feldnamen (Groß/Kleinschreibung)
        public IList<string> EntityFieldlist = DposUtils.GetFieldlist(typeof(TItem));

        protected DpeData data;
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

        public RadzenDataGrid<TItem> grid;

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

            var columnlist = FltrRec.Columnlist;  //von DB

            //Groß/Klein korrigieren:
            foreach (var col in columnlist.Columns)
            {
                col.Fieldname = DposUtils.AdjustFieldname(col.Fieldname, EntityFieldlist);
            }

            //fehlende Entity Felder als invisible ergänzen:
            foreach (var field in EntityFieldlist)
            {
                var col = columnlist.Columns.Where(x => x.Fieldname == field).FirstOrDefault();
                if (col == null)
                {
                    columnlist.AddColumn($"{field}:0={field}");
                }
            }
            return columnlist;
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
                key[0] = DposUtils.AdjustFieldname(key[0], EntityFieldlist);  //Groß/Klein korrigieren
                fields.Add(key[0], key.Length >= 2 ? key[1] : "asc");
            }

            //nach Columnlist.Sortorder übertragen: 
            // beware Columnlist sonst deadlock!
            foreach (var col in _columnlist.Columns)
            {
                if (fields.TryGetValue(col.Fieldname, out string keyvalue))
                    col.SortOrder = keyvalue.ToLower() == "asc" ? Radzen.SortOrder.Ascending : Radzen.SortOrder.Descending;
                else
                    col.SortOrder = null;
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
            //IList<string> FL = DposUtils.GetFieldlist(typeof(TItem));

            //Generate SQL: anhand FltrList und References:
            _filter = "(lityp=\"B\" or lityp=\"A\") and (lort_nr=\"57\") and (sta=\"H\")";
            return _filter;
        }

        #endregion

    }
}
