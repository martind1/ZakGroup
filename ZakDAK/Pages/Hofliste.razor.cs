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
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Radzen.Blazor.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ZakDAK.Pages
{
    public partial class Hofliste
    {
        RadzenDataGrid<V_LADEZETTEL> grid;
        IList<V_LADEZETTEL> tbl;
        IList<V_LADEZETTEL> selectedList;
        LocalService<V_LADEZETTEL> lnav;

        [Parameter]
        public string Abfrage { get; set; } = "Standard";
        public string formKurz = "HTTP";

        [Inject]
        private GlobalService GNav { get; set; }
        [Inject]
        private ProtService Prot { get; set; }
        [Inject]
        DpeData Data { get; set; }

        #region Initialisieren, Starten, LoadData

        protected bool isLoading = false;
        protected int pagesize;  //LNav bzw GNax MaxRecordCount

        readonly bool Fullscreen = true; //hier setzen!
        readonly string gridStyle;

        public Hofliste()
        {
            //GNav ist hier noch null!
            //grid ist hier noch null!
            gridStyle = Fullscreen ?
                "height: calc(100vh - 75px); width: calc(100vw - 50px); " :
                "height: calc(100vh - 170px - 75px); width: calc(100vw - 250px);";
        }

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

            lnav = new LocalService<V_LADEZETTEL>(GNav, Data, formKurz, Abfrage)
            {
                Pagetitle = "ZAK Digitale Annahmekontrolle",
                References = new FltrList("sta=H"),
            };
            GNav.SetLNav(lnav);
            InitLookUp();
        }

        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;
            await Task.Yield();

            lnav.RefreshAbfrage();  //von FLTR neu einlesen (Columnlist, Fltrlist,..)

            //pagesize anpassen falls in GNav/LNav geändert
            pagesize = GNav.MaxRecordCount;
            if (args.Skip == 0 && args.Top != pagesize)
            {
                Prot.SMessL($"Top: {args.Top} => {pagesize}");
                args.Top = pagesize;
            }
            //merken für später
            lnav.OrderBy = args.OrderBy;
            Prot.SMessL($"Skip: {args.Skip}, Top: {args.Top}, pagesize={pagesize}");

            lnav.GenFilter();  //SQL Filter und Parameters generieren
            var query = Data.QueryFromLoadDataArgs(args);
            query.Filter = lnav.Filter;
            query.FilterParameters = lnav.FilterParameters;
            Prot.Prot0SL($"Filter:{query.Filter}");
            Prot.Prot0SL($"Filterparameter:{JsonSerializer.Serialize(query.FilterParameters)}");
            tbl = Data.EntityQuery<V_LADEZETTEL>(query).ToList();
            //Idee ohne Data Service: tbl = lnav.queryList();

            lnav.Recordcount = Data.EntityQueryCount<V_LADEZETTEL>(query);
            Prot.SMessL($"Loaded. Count: {lnav.Recordcount}");

            isLoading = false;
        }

        #endregion

        #region Insert, Edit, Delete

        V_LADEZETTEL editRec;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier console
                LoadBedienerAsync();  //von LocalStorage
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        void EditRow(V_LADEZETTEL vorf)
        {
            selectedList = new List<V_LADEZETTEL>() { vorf };
            editRec = vorf;
            lnav.PageState = GlobalService.PageState.Single;
        }

        void SaveRow(V_LADEZETTEL vorf)
        {
            lnav.PageState = GlobalService.PageState.Multi;
        }

        void CancelEdit(V_LADEZETTEL vorf)
        {
            lnav.PageState = GlobalService.PageState.Multi;
        }

        #endregion

        #region Kommando, Polling, Reset

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
                        //var vorf = tbl.FirstOrDefault();
                        //EditRow(vorf);
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
            if (grid != null)
                await grid.Reload();
            //await grid.FirstPage(true);
        }

        public int PollSeconds { get; set; } = 120;

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

        void OnSort(DataGridColumnSortEventArgs<V_LADEZETTEL> args)
        {
            //bringt EAccess string json = JsonConvert.SerializeObject(args);
            //bringt auch EAccess string json = System.Text.Json.JsonSerializer.Serialize(args);
            Prot.SMessL($"OnSort:{args.Column.Property} {args.SortOrder}");
            var colList = grid.ColumnsCollection.Where(c => c.SortOrder != null);
            foreach (var col in colList)
            {
                Prot.SMessL($"SortColumn Fld:{col.Property} SortProp:{col.SortProperty} a/d:{col.SortOrder}");

            }
        }
        #endregion

        #region Single Form

        //readonly Dictionary<string, Asws> aswOK = new()
        //{
        //    {"J", new Asws(){ Param = "J", Value = "OK", IsTrue = true } },
        //    {"N", new Asws(){ Param = "N", Value = "Nicht OK", IsTrue = false } }
        //};


        public void Submit(V_LADEZETTEL arg)
        {
            //Speichern;
            //Data.EntityUpdate<V_LADEZETTEL>(arg);
            arg.geaendert_von = Bediener;
            Data.VorfUpdate(arg);

            lnav.PageState = GlobalService.PageState.Multi;
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            //Unable to track an instance of type 'V_LADEZETTEL' because it does not have a primary key. 
            //Data.Ctx.Entry(editRec).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        public void Cancel()
        {
            lnav.PageState = GlobalService.PageState.Multi;
            //neu laden (EntityStat geht nicht wg View ohne primary key -> haskey()
            //RunKommando((int)GlobalService.KommandoTyp.Refresh);
            var vorfEntry = Data.EntityEntry<V_LADEZETTEL>(editRec);
            if (vorfEntry.State == EntityState.Modified)
            {
                vorfEntry.CurrentValues.SetValues(vorfEntry.OriginalValues);
                vorfEntry.State = EntityState.Unchanged;
            }
        }

        #endregion

        #region LookUp

        LookupDef<VFUE> LuVFUE;
        RadzenDropDownDataGrid<string> cobVFUE;

        LookupDef<DKAT> LuDKAT;
        RadzenDropDownDataGrid<string> cobDKAT;

        void InitLookUp()
        {
            LuVFUE = new LookupDef<VFUE>(GNav, Data, Prot, "VFUE", "LOOKUP")
            {
                References = new FltrList("VFUE_NR=:VFUE_NR"),
            };

            LuDKAT = new LookupDef<DKAT>(GNav, Data, Prot, "DKAT", "LOOKUP")
            {
                References = new FltrList("DKAT_NR=:DKAT_NR"),
                Virtualization = true,
            };
        }

        #endregion

        #region Bediener       

        [Inject]
        private ProtectedLocalStorage protectedLocalStorage { get; set; }
        [Inject]
        IJSRuntime JS { get; set; }
        public string Bediener { get; set; } = "";

        public async void LoadBedienerAsync()
        {
            Bediener = (await protectedLocalStorage.GetAsync<string>("Bediener")).Value;
            StateHasChanged();
        }

        public async void SaveBedienerAsync()
        {
            await protectedLocalStorage.SetAsync("Bediener", Bediener);
        }


        private async Task BedienerPrompt()
        {
            object[] label = new object[] { "Bediener:", Bediener };
            Bediener = await JS.InvokeAsync<string>("prompt", label);
            SaveBedienerAsync();
        }

        private void Abmelden()
        {
            Bediener = "";
            SaveBedienerAsync();
        }

        #endregion

    }
}