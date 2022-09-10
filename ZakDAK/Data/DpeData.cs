using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Radzen;
using System.Data.Entity;
using System.Linq.Dynamic.Core;
using ZakDAK.Connection.DPE;
using ZakDAK.Entities.DPE;

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
            this.Ctx = ctx;
            this.navigationManager = navigationManager;
        }

        // von CRMDemoBlazor:
        public IQueryable QueryableFromQuery(Query query, IQueryable items)
        {
            if (query != null)
            {
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
            //siehe d:\Blazor\Repos\radzenhq\radzen-blazor\Radzen.Blazor\Common.cs
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

        public IQueryable<VORF> VorfQuery(LoadDataArgs args)
        {
            var query = QueryFromLoadDataArgs(args);
            return VorfQuery(query);
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

        public int VorfQueryCount(LoadDataArgs args)
        {
            var query = QueryFromLoadDataArgs(args);
            return VorfQueryCount(query);
        }


        public void VorfUpdate(VORF frzg)
        {
            Ctx.Update<VORF>(frzg);
            Ctx.SaveChanges();
        }

        public EntityEntry<VORF> VorfEntry(VORF frzg)
        {
            return Ctx.Entry<VORF>(frzg);
        }

        public void VorfRemove(VORF frzg)
        {
            Ctx.Remove<VORF>(frzg);
            Ctx.SaveChanges();
        }

        public void VorfAdd(VORF frzg)
        {
            Ctx.Add<VORF>(frzg);
            Ctx.SaveChanges();
        }

        #endregion

    }
}