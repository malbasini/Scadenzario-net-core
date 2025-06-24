

namespace Scadenzario.Models.InputModels.Scadenze;

public class ScadenzaPayInputModel
{
    public int IdScadenza { get; set; }
    public string? UserId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}