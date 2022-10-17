using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Serilog;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZakDAK;
using ZakDAK.Connection.DPE;
using ZakDAK.Data;
using ZakDAK.Entities.DPE;
using ZakDAK.Kmp;
using ZakDAK.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ZakDAK.Pages
{
    public partial class Hofliste
    {
        RadzenDataGrid<VORF> grid;
        IList<VORF> tbl;
        IList<VORF> selectedList;
        LocalService<VORF> lnav;

        [Parameter]
        public string Abfrage { get; set; } = "Standard";
        public string formKurz = "HTTP";

        [Inject]
        private GlobalService GNav { get; set; }
        [Inject]
        private ProtService Prot { get; set; }

        public Hofliste()
        {
            //GNav ist hier noch null!
            //grid ist hier noch null!
        }

        [Inject]
        DpeData Data { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            GNav.OnDoKommando += RunKommando;
            pagesize = GNav.MaxRecordCount;
            //Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier noch keine console!
        }

        protected override void OnParametersSet()
        {
            Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier console!

            lnav = new LocalService<VORF>(GNav, Data, formKurz, Abfrage)
            {
                Pagetitle = "ZAK Digitale Annahmekontrolle",
                References = new FltrList("sta=H")
            };
        }

        #region Konfiguration

        public int PollSeconds { get; set; } = 120;

        #endregion

        #region Demo Inline Insert, Edit, Delete

        VORF vorfToInsert;
        VORF editRec;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier console
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        //async Task EditRow(VORF vorf)
        //{
        //    await grid.EditRow(vorf);
        //}

        //async Task SaveRow(VORF vorf)
        //{
        //    if (vorf == vorfToInsert)
        //    {
        //        vorfToInsert = null;
        //    }

        //    await grid.UpdateRow(vorf);
        //}

        void EditRow(VORF vorf)
        {
            selectedList = new List<VORF>() { vorf };
            editRec = vorf;
            lnav.PageState = GlobalService.PageState.Single;
        }

        void SaveRow(VORF vorf)
        {
            lnav.PageState = GlobalService.PageState.Multi;
        }

        void CancelEdit(VORF vorf)
        {
            //if (vorf == vorfToInsert)
            //{
            //    vorfToInsert = null;
            //}

            //grid.CancelEditRow(vorf);
            //// For production
            //var vorfEntry = Data.VorfEntry(vorf);
            //if (vorfEntry.State == EntityState.Modified)
            //{
            //    vorfEntry.CurrentValues.SetValues(vorfEntry.OriginalValues);
            //    vorfEntry.State = EntityState.Unchanged;
            //}
            lnav.PageState = GlobalService.PageState.Multi;
        }

        void OnUpdateRow(VORF vorf)
        {
            if (vorf == vorfToInsert)
            {
                vorfToInsert = null;
            }

            Data.VorfUpdate(vorf);
            // For demo purposes only
            //vorf.Customer = dbContext.Customers.Find(vorf.CustomerID);
            //vorf.Employee = dbContext.Employees.Find(vorf.EmployeeID);
            // For production
            //dbContext.SaveChanges();
        }

        async Task DeleteRow(VORF vorf)
        {
            if (vorf == vorfToInsert)
            {
                vorfToInsert = null;
            }

            if (tbl.Contains(vorf))
            {
                Data.VorfRemove(vorf);
                // For demo purposes only
                tbl.Remove(vorf);
                // For production
                //dbContext.SaveChanges();
                await grid.Reload();
            }
            else
            {
                grid.CancelEditRow(vorf);
            }
        }

        async Task InsertRow()
        {
            vorfToInsert = new VORF();
            await grid.InsertRow(vorfToInsert);
        }

        void OnCreateRow(VORF vorf)
        {
            Data.VorfAdd(vorf);
            // For demo purposes only
            //vorf.Customer = dbContext.Customers.Find(vorf.CustomerID);
            //vorf.Employee = dbContext.Employees.Find(vorf.EmployeeID);
            // For production
            //dbContext.SaveChanges();
        }
        #endregion

        #region von Sped:
        protected bool isLoading = false;
        protected int pagesize;  //LNav bzw GNax MaxRecordCount

        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;
            await Task.Yield();

            lnav.RefreshAbfrage();  //von FLTR neu einlesen (Columnlist, Fltrlist,..)

            //pagesize anpassen falls in GNav/LNav ge�ndert
            pagesize = GNav.MaxRecordCount;
            if (args.Skip == 0 && args.Top != pagesize)
            {
                Prot.SMessL($"Top: {args.Top} => {pagesize}");
                args.Top = pagesize;
            }
            //merken f�r sp�ter
            lnav.OrderBy = args.OrderBy;
            Prot.SMessL($"Skip: {args.Skip}, Top: {args.Top}, pagesize={pagesize}");

            lnav.GenFilter();  //SQL Filter und Parameters generieren
            var query = Data.QueryFromLoadDataArgs(args);
            query.Filter = lnav.Filter;
            query.FilterParameters = lnav.FilterParameters;
            Prot.Prot0SL($"Filter:{query.Filter}");
            Prot.Prot0SL($"Filterparameter:{JsonSerializer.Serialize(query.FilterParameters)}");
            tbl = Data.VorfQuery(query).ToList();
            //Idee ohne Data Service: tbl = lnav.queryList();

            lnav.Recordcount = Data.VorfQueryCount(query);
            Prot.SMessL($"Loaded. Count: {lnav.Recordcount}");

            isLoading = false;
        }

        #endregion

        #region Kommando von GlobalNavigator �ber GlobalService nach hier

        public void RunKommando(int KNr)
        {
            switch ((GlobalService.KommandoTyp)KNr)
            {
                case GlobalService.KommandoTyp.Suchen:
                    {
                        //SuchenDialog
                        //if (allowFiltering)
                        //    _ = InsertRow();
                        StateHasChanged();
                        break;
                    }
                case GlobalService.KommandoTyp.Refresh:
                    {
                        _ = Reset();
                        break;
                    }
                case GlobalService.KommandoTyp.Pagesize:
                    {
                        pagesize = GNav.MaxRecordCount;
                        _ = Reset();
                        break;
                    }
            }
        }

        async Task Reset()
        {
            //grid.Reset(true);
            await grid.Reload();
            //await grid.FirstPage(true);
        }

        #endregion

        void OnSort(DataGridColumnSortEventArgs<VORF> args)
        {
            //
            //bringt EAccess string json = JsonConvert.SerializeObject(args);
            //bringt auch EAccess string json = System.Text.Json.JsonSerializer.Serialize(args);
            Prot.SMessL($"OnSort:{args.Column.Property} {args.SortOrder}");
            var colList = grid.ColumnsCollection.Where(c => c.SortOrder != null);
            foreach (var col in colList)
            {
                Prot.SMessL($"SortColumn Fld:{col.Property} SortProp:{col.SortProperty} a/d:{col.SortOrder}");

            }
        }


        /// <summary>
        /// Daten alle 120s refreshen (Tablet Hofliste)
        /// </summary>
        void Poll()
        {
            if (lnav.PageState == GlobalService.PageState.Multi)
            {
                Prot.SMessL("Poll Timer: Reset");
                // _ = Reset();
                RunKommando((int)GlobalService.KommandoTyp.Refresh);
            }
        }

        #region Single Form

        //public bool? CheckAsw[string param] {
        //    get { return param == "J" ? true : param == "N" ? false : null; }
        //}
        // Ziel: bind=@CheckAsw[@vorf.HOFL_OK]
        

        public class Asws
        {
            public string Param { get; set; }
            public string Value { get; set; }
        }

        IEnumerable<Asws> aswOK = new Asws[] {
            new Asws(){ Param = "J", Value = "OK"},
            new Asws(){ Param = "N", Value = "Nicht OK"} };


        public void Submit(VORF arg)
        {
            //Speichern;
            //Data.EntityUpdate<VORF>(arg);
            Data.VorfUpdate(arg);

            lnav.PageState = GlobalService.PageState.Multi;
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            Data.Ctx.Entry(editRec).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        public void Cancel()
        {
            lnav.PageState = GlobalService.PageState.Multi;
            RunKommando((int)GlobalService.KommandoTyp.Refresh);
        }

        #endregion

    }
}