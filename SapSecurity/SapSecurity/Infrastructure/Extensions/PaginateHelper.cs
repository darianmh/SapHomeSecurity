using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SapSecurity.ViewModel;

namespace SapSecurity.Infrastructure.Extensions
{
    public static class PaginateHelper
    {

        public static async Task<PaginatedViewModel<T>> PaginateAsync<T>(this IQueryable<T> query, int pageSize, int pageIndex) =>
            new PaginatedViewModel<T>()
            {
                Count = query.Count(),
                Data = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync()
            };
    }
}
