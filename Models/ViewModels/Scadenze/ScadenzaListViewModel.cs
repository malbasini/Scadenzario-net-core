using Scadenzario.Models.InputModels.Scadenze;

namespace Scadenzario.Models.ViewModels.Scadenze
{
    public class ScadenzaListViewModel:IPaginationInfo
    {
        public ListViewModel<ScadenzaViewModel>? Scadenze {get;set;}
        public ScadenzaListInputModel Input {get;set;}

        #region Implementazione combo anni scadenze
        
        public required List<int> Anni { get; init; }
        public required int AnnoSelezionato { get; init; }
        //public required List<Scadenza> Scadenze { get; init; }
        
        #endregion
        
        
        #region Implementazione IPaginationInfo
         
        int IPaginationInfo.CurrentPage => Input.Page;

        int IPaginationInfo.TotalResults => Scadenze.TotalCount;

        int IPaginationInfo.ResultsPerPage => Input.Limit;

        string IPaginationInfo.Search => Input.Search;

        string IPaginationInfo.OrderBy => Input.OrderBy;

        bool IPaginationInfo.Ascending => Input.Ascending;
        
        #endregion
    }
}