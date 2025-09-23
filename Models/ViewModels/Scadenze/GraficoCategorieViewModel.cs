namespace Scadenzario.Models.ViewModels.Scadenze;

public sealed class GraficoCategorieViewModel
{
    public required List<string?> Labels { get; init; }
    public required List<decimal> Values { get; init; }
    public DateTime? Dal { get; init; }
    public DateTime? Al  { get; init; }
    
    public required List<int> Anni { get; init; }
    
    public required int AnnoSelezionato { get; init; }
   
    
}