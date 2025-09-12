using System.Data.Entity;
using Scadenzario.Areas.Identity.Data;

namespace Scadenzario.Models.Services.Applications.Scadenze;

using Scadenzario.Data;
using Scadenzario.Models.Dtos;
using Microsoft.EntityFrameworkCore;


public class AnaliticheSpeseService
{
    private readonly ScadenzarioIdentityDbContext _ctx;

    public AnaliticheSpeseService(ScadenzarioIdentityDbContext ctx) => _ctx = ctx;

    public async Task<List<CategoriaTotaleDto>> GetTotaliPerCategoriaAsync(
        DateTime? dal = null,
        DateTime? al  = null,
        CancellationToken ct = default)
    {
        var q = _ctx.Scadenze.AsNoTracking().AsQueryable();

        if (dal is not null)
            q = q.Where(s => s.DataScadenza >= dal.Value.Date);

        if (al is not null)
            q = q.Where(s => s.DataScadenza <= al.Value.Date);
        
        var rows = await q
            .GroupBy(s => s.Denominazione) // string? OK. Se nullable, vedi nota sotto.
            .Select(g => new
            {
                Categoria = g.Key,                      // string (o enum) ma senza ToString()
                Totale    = g.Sum(e => e.Importo)       // se Importo è decimal?
            })
            .OrderBy(x => x.Categoria)
            .ToListAsync(ct);

        // Mapping in memoria al tuo DTO (qui puoi fare coalesce/ToString)
        return rows
            .Select(r => new CategoriaTotaleDto(
                Categoria: r.Categoria ?? "Senza categoria",
                Totale: r.Totale
            ))
            .ToList();
    }
}