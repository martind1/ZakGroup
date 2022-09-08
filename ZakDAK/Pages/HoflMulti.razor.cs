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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ZakDAK.Pages
{
    public partial class HoflMulti
    {
        RadzenDataGrid<VORF> vorfGrid;
        IList<VORF> vorf_tbl;
        //IEnumerable<Customer> customers;
        //IEnumerable<Employee> employees;

        [Inject]
        DpeData Data { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            //customers = dbContext.Customers.ToList();
            //employees = dbContext.Employees.ToList();
            // For demo purposes only
            //orders = dbContext.Orders.Include("Customer").Include("Employee").ToList();
            // For production
            //orders = dbContext.Orders.Include("Customer").Include("Employee");
            //loaddata - vorf_tbl = Data.FrzgQuery(null).ToList();
            GNav.SetGrid<VORF>(vorfGrid);
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var doReload = false;
                var column = vorfGrid.ColumnsCollection.Where(c => c.Property == "sta").FirstOrDefault();
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                //if (column != null)
                {
                    column.FilterValue = "Z";
                    column.FilterOperator = FilterOperator.NotEquals;
                    doReload = true;
                }
                column = vorfGrid.ColumnsCollection.Where(c => c.Property == "edt").FirstOrDefault();
                //if (column != null)
                {
                    column.FilterValue = new DateTime(2018, 1, 1);
                    column.FilterOperator = FilterOperator.GreaterThanOrEquals;
                    doReload = true;
                }
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
                if (doReload)
                  vorfGrid.Reload();
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

            Data.FrzgUpdate(vorf);
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
            var vorfEntry = Data.FrzgEntry(vorf);
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
                Data.FrzgRemove(vorf);
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
            Data.FrzgAdd(vorf);
            // For demo purposes only
            //vorf.Customer = dbContext.Customers.Find(vorf.CustomerID);
            //vorf.Employee = dbContext.Employees.Find(vorf.EmployeeID);
            // For production
            //dbContext.SaveChanges();
        }

        #region von Sped:
        protected int count;
        protected bool isLoading = false;
        //private String oldArgs = "-1";
        protected int pagesize = 5000;  //LNav 20

        [Inject]
        private GlobalService GNav { get; set; }

        protected async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;
            await Task.Yield();

            GNav.SMessL($"Skip: {args.Skip}, Top: {args.Top}");
            vorf_tbl = Data.FrzgQuery(args).ToList();
            // if (oldArgs != args.Filter)
            {
                //oldArgs = args.Filter;
                count = Data.FrzgQueryCount(args);
                GNav.SMessL($"Loaded. Count: {count}");
                //pagesize = count;
            }

            //if (args.Top > 20)
            isLoading = false;
        }

        void OnFilter(DataGridColumnFilterEventArgs<VORF> args)
        {
            GNav.SMessL($"Filter:{args}");
        }

        #endregion
    }
}