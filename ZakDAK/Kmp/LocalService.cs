using NuGet.Protocol;
using Radzen.Blazor;
using System.Security.Policy;
using ZakDAK.Entities.DPE;

namespace ZakDAK.Kmp
{

    //Der lokale Navigator
    public class LocalService<TItem>
    {
        //todo: nach NLnk - NavigatorLink - LinkService
        public ColumnList Columnlist { get; set; }
        public FltrList Fltrlist { get; set; }
        public FltrList References { get; set; }  //nicht durch User änderbar
        public string abfrage = "";
        public string formKurz = "";

        RadzenDataGrid<TItem> grid;

        public LocalService()
        {
            
        }

        public LocalService(RadzenDataGrid<TItem> grid, string formKurz, string abfrage): this()
        {
            this.grid = grid;
            this.formKurz = formKurz;
            this.abfrage = abfrage;

            Columnlist = GetColumnlist();
        }

#region ColumnList, References

        public ColumnList GetColumnlist()
        {
            //Test: statische Liste
            //Später: LookUp FLTR[formKurz, abfrage].ColumnList
            string cl =
@"Quittung:5=HOFL_KTRL
sta:0=sta
edt:0=edt
Lieferart:10=lityp
EIN:5=ETm
Fahrzeug:11=fahr_knz
Beförderer:23=anl_na1
Sorte Bez.:21=srte_bez
Tara:8=tagew
erz_na1:13=erz_na1
erz_na2:14=erz_na2
erz_str:15=erz_str";
            return new ColumnList(cl);
        }

        #endregion

        //Idee: hier auch individueller MaxRecordCount

#region FltrList und References



#endregion

    }
}
