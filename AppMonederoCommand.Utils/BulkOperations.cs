using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Utils
{
    public static class BulkOperations
    {
        private static int batchSize = 1000;
        public static async Task<bool> InsertarEnLotes<TEntity>(this DbContext context, IEnumerable<TEntity> elementos)
            where TEntity : class
        {
            var contador = 0;

            foreach (var elemento in elementos)
            {
                context.Set<TEntity>().Add(elemento);

                if (++contador % batchSize == 0)
                {
                    await context.SaveChangesAsync();
                    await DesconectarEntidadesAsync(context);
                }
            }

            await context.SaveChangesAsync(); // Guardar los registros restantes

            return contador == elementos.Count();
        }

        public static async Task<bool> ActualizarEnLotes<TEntity>(this DbContext context, IEnumerable<TEntity> elementos)
            where TEntity : class
        {
            var contador = 0;

            foreach (var elemento in elementos)
            {
                context.Set<TEntity>().Update(elemento);

                if (++contador % batchSize == 0)
                {
                    await context.SaveChangesAsync();
                    await DesconectarEntidadesAsync(context);
                }
            }

            await context.SaveChangesAsync(); // Guardar los registros restantes

            return contador == elementos.Count();
        }

        public static async Task DesconectarEntidadesAsync(DbContext context)
        {
            var entries = context.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }

}
