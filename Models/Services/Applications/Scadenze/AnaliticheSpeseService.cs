using Scadenzario.Areas.Identity.Data;
using Scadenzario.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Scadenzario.Models.Services.Applications.Scadenze;

public class AnaliticheSpeseService
{
    private readonly ScadenzarioIdentityDbContext _ctx;

    public AnaliticheSpeseService(ScadenzarioIdentityDbContext ctx) => _ctx = ctx;
    
    public async Task<List<int>> GetAnniDisponibiliAsync(CancellationToken ct = default)
    {
        var anni = await _ctx.Scadenze
            .Where(s => true)
            .Select(s => s.DataScadenza.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);

        var current = DateTime.UtcNow.Year;
        if (!anni.Contains(current)) anni.Insert(0, current);
        return anni;
    }
    public async Task<List<CategoriaTotaleDto>> GetTotaliPerCategoriaAnnoAsync(
        int anno, 
        DateTime? dal, 
        DateTime? al,
        string? filter, 
        CancellationToken ct = default, 
        DateTime? data = null)
    {
        var q = _ctx.Scadenze.AsNoTracking().AsQueryable();
        
        if (dal is not null)
            q = q.Where(s => s.DataScadenza >= dal.Value.Date);

        if (al is not null)
            q = q.Where(s => s.DataScadenza <= al.Value.Date);
        
        if (data is not null)
            q = q.Where(s => s.DataScadenza == data.Value.Date);

        if (filter is not null)
            q = q.Where(s=> s.Denominazione.Contains(filter));
        
        // Se DataScadenza è nullable DateTime?
        var rows = await q
            .Where(s => s.DataScadenza.Year == anno)
            .Where(s=> s.Status=="PAGATA")
            .GroupBy(s => s.Denominazione)                    // <-- deve essere una colonna mappata!
            .Select(g => new
            {
                Categoria = g.Key,                            // niente ToString qui
                Totale    = g.Sum(e => e.Importo)             // se Importo è decimal? usa (e.Importo ?? 0m)
            })
            .OrderBy(x => x.Categoria)                        // ok sull’anonimo
            .ToListAsync(ct);

        // Mapping in memoria: qui puoi fare coalesce/stringa “di fallback”
        return rows.Select(r => new CategoriaTotaleDto(
            Categoria: r.Categoria ?? "Senza categoria",
            Totale: r.Totale
        )).ToList();
    }
}