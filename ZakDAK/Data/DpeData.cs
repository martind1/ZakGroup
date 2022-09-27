﻿using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Radzen;
using System.Data.Entity;
using System.Linq.Dynamic.Core;
using ZakDAK.Connection.DPE;
using ZakDAK.Entities.DPE;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Query = Radzen.Query;

namespace ZakDAK.Data
{
    /// <summary>
    /// Datenservice für QUVA
    /// </summary>
    public partial class DpeData
    {
        public DpeContext Ctx { get; set; }
        private readonly NavigationManager navigationManager;

        public DpeData(DpeContext ctx, NavigationManager navigationManager)
        {
            Ctx = ctx;
            this.navigationManager = navigationManager;
        }

        // von CRMDemoBlazor / dynamic-linq:
        public IQueryable QueryableFromQuery(Query query, IQueryable items)
        {
            if (query != null)
            {
                // Relationen laden (Eager loading)
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p);
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
            return items;
        }

        public Query QueryFromLoadDataArgs(LoadDataArgs args)
        {
            //dynamic-linq, siehe d:\Blazor\Repos\radzenhq\radzen-blazor\Radzen.Blazor\Common.cs
            //für LoadData Event
            var query = new Query
            {
                Skip = args.Skip,
                Top = args.Top,
                Filter = args.Filter,
                OrderBy = args.OrderBy
            };
            return query;
        }
        
        #region Vorf

        public IQueryable<VORF> VorfQuery(Query query)
        {
            var items = Ctx.VORF_Tbl.AsQueryable();
            if (query != null)
            {
                items = (IQueryable<VORF>)QueryableFromQuery(query, items);
            }
            return items;
        }

        public int VorfQueryCount(Query query)
        {
            var items = Ctx.VORF_Tbl.AsQueryable();
            //items = items.Include(i => i.Contact);
            //items = items.Include(i => i.OpportunityStatus);
            if (query != null)
            {
                query.Skip = null;
                query.Top = null;
                items = (IQueryable<VORF>)QueryableFromQuery(query, items);
            }
            return items.Count();
        }


        public void VorfUpdate(VORF vorf)
        {
            Ctx.Update<VORF>(vorf);
            Ctx.SaveChanges();
        }

        public EntityEntry<VORF> VorfEntry(VORF vorf)
        {
            return Ctx.Entry<VORF>(vorf);
        }

        public void VorfRemove(VORF vorf)
        {
            Ctx.Remove<VORF>(vorf);
            Ctx.SaveChanges();
        }

        public void VorfAdd(VORF vorf)
        {
            Ctx.Add<VORF>(vorf);
            Ctx.SaveChanges();
        }

        #endregion

        #region FLTR

        public FLTR GetFltr(string formkurz, string abfrage)
        {
            //var items = Ctx.FLTR_Tbl.AsQueryable();
            var query = new Query()
            {
                Filter = "FORM = @0 and NAME = @1",
                FilterParameters = new object[] { formkurz, abfrage }
            };
            var items = (IQueryable<FLTR>)QueryableFromQuery(query, Ctx.FLTR_Tbl);
            var fltr = items.FirstOrDefault();
            return fltr;  //null bei EOF
        }

        #endregion

    }
}