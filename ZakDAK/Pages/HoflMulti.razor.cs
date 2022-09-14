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

namespace ZakDAK.Pages
{
    public partial class HoflMulti
    {
        RadzenDataGrid<VORF> vorfGrid;
        IList<VORF> vorf_tbl;
        IList<VORF> selectedVORF;
        LocalService<VORF> lnav;

        //private string _abfrage = "Standard";
        [Parameter]
        //public string Abfrage { get => String.IsNullOrEmpty(_abfrage) ? "Standard" : _abfrage; set => _abfrage = value; }
        public string Abfrage { get; set; } = "Standard";
        public string formKurz = "HTTP";

        [Inject]
        private GlobalService GNav { get; set; }
        [Inject]
        private ProtService Prot { get; set; }

        public HoflMulti()
        {
            //GNav ist hier noch null!
            //vorfGrid ist hier noch null!
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
            lnav = new LocalService<VORF>(vorfGrid, Data, formKurz, Abfrage)
            {
                References = new FltrList("lityp=<>X\r\nsta=H")
            };
        }

        #region Konfiguration

        public int PollSeconds { get; set; } = 120;

        #endregion

        #region Demo Inline Insert, Edit, Delete

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Prot.SMessL($"Init. Abfrage={Abfrage}");  //hier noch keine console!

                //12.09.22 Filter jetzt per LNav
//                var doReload = false;
//                var column = vorfGrid.ColumnsCollection.Where(c => c.Property == "sta").FirstOrDefault();
//#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
//                //if (column != null)
//                {
//                    column.FilterValue = "Z";
//                    column.FilterOperator = FilterOperator.NotEquals;
//                    doReload = true;
//                }
//                column = vorfGrid.ColumnsCollection.Where(c => c.Property == "edt").FirstOrDefault();
//                //if (column != null)
//                {
//                    column.FilterValue = new DateTime(2018, 1, 1);
//                    column.FilterOperator = FilterOperator.GreaterThanOrEquals;
//                    doReload = true;
//                }
//#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
//                if (doReload)
//                  vorfGrid.Reload();
            }
            return base.OnAfterRenderAsync(firstRender);
        }

        async Task EditRow(VORF vorf)
        {
            await vorfGrid.EditRow(vorf);
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

        async Task SaveRow(VORF vorf)
        {
            if (vorf == vorfToInsert)
            {
                vorfToInsert = null;
            }

            await vorfGrid.UpdateRow(vorf);
        }

        void CancelEdit(VORF vorf)
        {
            if (vorf == vorfToInsert)
            {
                vorfToInsert = null;
            }

            vorfGrid.CancelEditRow(vorf);
            // For production
            var vorfEntry = Data.VorfEntry(vorf);
            if (vorfEntry.State == EntityState.Modified)
            {
                vorfEntry.CurrentValues.SetValues(vorfEntry.OriginalValues);
                vorfEntry.State = EntityState.Unchanged;
            }
        }

        async Task DeleteRow(VORF vorf)
        {
            if (vorf == vorfToInsert)
            {
                vorfToInsert = null;
            }

            if (vorf_tbl.Contains(vorf))
            {
                Data.VorfRemove(vorf);
                // For demo purposes only
                vorf_tbl.Remove(vorf);
                // For production
                //dbContext.SaveChanges();
                await vorfGrid.Reload();
            }
            else
            {
                vorfGrid.CancelEditRow(vorf);
            }
        }

        VORF vorfToInsert;
        async Task InsertRow()
        {
            vorfToInsert = new VORF();
            await vorfGrid.InsertRow(vorfToInsert);
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
        protected int count;
        protected bool isLoading = false;
        protected int pagesize;  //LNav bzw GNax MaxRecordCount

        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;
            await Task.Yield();

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

            args.Filter = lnav.GetFilter();  //SQL generieren
            vorf_tbl = Data.VorfQuery(args).ToList();
            //Idee ohne Data Service: vorf_tbl = lnav.queryList();
            count = Data.VorfQueryCount(args);

            Prot.SMessL($"Loaded. Count: {count}");



            isLoading = false;
        }

        #endregion

        #region Kommando von GlobalNavigator über GlobalService nach hier

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
            await vorfGrid.Reload();
            //await grid.FirstPage(true);
        }

        #endregion

        void OnSort(DataGridColumnSortEventArgs<VORF> args)
        {
            //
            //bringt EAccess string json = JsonConvert.SerializeObject(args);
            //bringt auch EAccess string json = System.Text.Json.JsonSerializer.Serialize(args);
            Prot.SMessL($"OnSort:{args.Column.Property} {args.SortOrder}");
            var colList = vorfGrid.ColumnsCollection.Where(c => c.SortOrder != null);
            foreach (var col in colList)
            {
                Prot.SMessL($"SortColumn Fld:{col.Property} SortProp:{col.SortProperty} a/d:{col.SortOrder}");

            }
        }

        void Poll()
        {
            _ = Reset();
        }

    }
}