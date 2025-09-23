using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions.Application;
using Scadenzario.Areas.Identity.Data;
using Scadenzario.Controllers;
using Scadenzario.Models.Entities;
using Scadenzario.Models.Entity;
using Scadenzario.Models.InputModels;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Options;
using Scadenzario.Models.Services.Applications.Beneficiari;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.Services.Infrastructure;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Beneficiari;
using Scadenzario.Models.ViewModels.Scadenze;
using Scadenze.Models.Exceptions.Application;

namespace Scadenzario.Models.Services.Application.Scadenze
{
    public class EfCoreScadenzeService : IScadenzeService
    {
        private readonly ILogger<MailKitEmailSender> _logger;
        private readonly ScadenzarioIdentityDbContext _dbContext;
        private readonly IPaymentGatewayStripe paymentGatewayStripe;
        private readonly IPaymentGatewayPayPal paymentGatewayPayPal;
        private readonly LinkGenerator linkGenerator;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOptionsMonitor<SmtpOptions> _options;
        private readonly IConfiguration _configuration;

        
        public EfCoreScadenzeService(
            IHttpContextAccessor httpContextAccessor,
            ScadenzarioIdentityDbContext dbContext,
            LinkGenerator linkGenerator,
            IPaymentGatewayPayPal paymentGatewayPayPal,
            IPaymentGatewayStripe paymentGatewayStripe,
            IOptionsMonitor<SmtpOptions> options,
            IConfiguration configuration,
            ILogger<MailKitEmailSender> logger
            )
        {
            _logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            this._dbContext = dbContext;
            this.linkGenerator = linkGenerator;
            this.paymentGatewayPayPal = paymentGatewayPayPal;
            this.paymentGatewayStripe = paymentGatewayStripe;
            _options = options;
            _configuration = configuration;

        }
        public async Task<ListViewModel<ScadenzaViewModel>?> GetScadenzeAsync(ScadenzaListInputModel model,  int anno, CancellationToken ct = default)
        {
            string IdUser = string.Empty;
            try                                                                                                     
            { 
                IdUser = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;        
            }                                                                                                       
            catch (NullReferenceException)                                                                          
            {                                                                                                       
                throw new UserUnknownException();                                                                   
            }

            IQueryable<Scadenza> baseQuery = _dbContext.Scadenze;

            baseQuery = (model.OrderBy, model.Ascending) switch
            {
                ("Denominazione", true) => baseQuery.OrderBy(s => s.Denominazione),
                ("Denominazione", false) => baseQuery.OrderByDescending(s => s.Denominazione),
                ("DataScadenza", true) => baseQuery.OrderBy(s => s.DataScadenza),
                ("DataScadenza", false) => baseQuery.OrderByDescending(s => s.DataScadenza),
                ("Importo", true) => baseQuery.OrderBy(s => s.Importo),
                ("Importo", false) => baseQuery.OrderByDescending(s => s.Importo),
                _ => baseQuery
            };
            if (IsDate(model.Search))
            {
                DateTime data = Convert.ToDateTime(model.Search);
                IQueryable<ScadenzaViewModel> queryLinq = baseQuery
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(Scadenze => Scadenze.IDUser == IdUser)
                    .Where(scadenze => scadenze.DataScadenza == data)
                    .Where(s => s.DataScadenza != null && s.DataScadenza!.Year == anno)
                    .OrderByDescending(s => s.DataScadenza) // ordina come preferisci
                    .ThenByDescending(s => s.IDScadenza)
                    .Select(scadenze => ScadenzaViewModel.FromEntity(scadenze));
                List<ScadenzaViewModel> scadenza = await queryLinq
                    .Skip(model.Offset)
                    .Take(model.Limit).ToListAsync();
                int totalCount =  queryLinq.Count();
                ListViewModel<ScadenzaViewModel> results = new ListViewModel<ScadenzaViewModel>
                {
                     Results=scadenza.ToList(),
                     TotalCount=totalCount
                };
                return results;
            }
            else
            {
               IQueryable<ScadenzaViewModel> queryLinq = baseQuery
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(Scadenze => Scadenze.IDUser == IdUser)
                    .Where(scadenze => scadenze.Denominazione.Contains(model.Search))
                    .Where(s => s.DataScadenza != null && s.DataScadenza!.Year == anno)
                    .OrderByDescending(s => s.DataScadenza) // ordina come preferisci
                    .ThenByDescending(s => s.IDScadenza)
                    .Select(scadenze => ScadenzaViewModel.FromEntity(scadenze)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                     
                List<ScadenzaViewModel> scadenza = await queryLinq
                    .Skip(model.Offset)
                    .Take(model.Limit).ToListAsync();
                int totalCount =  queryLinq.Count();
                ListViewModel<ScadenzaViewModel> results = new ListViewModel<ScadenzaViewModel>
                {
                     Results=scadenza,
                     TotalCount=totalCount
                };
                return results;
            }
        }

        public async Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id)
        {
            IQueryable<ScadenzaEditInputModel> queryLinq = _dbContext.Scadenze
                .AsNoTracking()
                .Where(s => s.IDScadenza == id)
                .Include(r=>r.Ricevute)
                .Select(s => ScadenzaEditInputModel.FromEntity(s)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            ScadenzaEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                _logger.LogWarning("Scadenza {id} not found", id);
                throw new ScadenzaNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<ScadenzaDetailViewModel> GetScadenzaAsync(int id)
        {
            string? IdUser = string.Empty;                                                                          
            try                                                                                                    
            {                                                                                                      
                IdUser = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;       
            }                                                                                                      
            catch (NullReferenceException)                                                                         
            {                                                                                                      
                throw new UserUnknownException();                                                                  
            }                                                                                         
            IQueryable<ScadenzaDetailViewModel> queryLinq = _dbContext.Scadenze
                .AsNoTracking()
                .Where(s => s.IDScadenza == id)
                .Where(z=> z.IDUser == IdUser)
                .Include(scadenza=>scadenza.Ricevute)
                .Select(s => ScadenzaDetailViewModel.FromEntity(s)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            ScadenzaDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            //.SingleAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto o contiene più di un elemento, solleva un'eccezione

            if (viewModel == null)
            {
                _logger.LogWarning("Scadenza {id} not found", id);
                throw new ScadenzaNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<ScadenzaDetailViewModelInfo> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel)
        {
            string denominazione = GetBeneficiarioById(inputModel.IdBeneficiario);
            int idBeneficiario = inputModel.IdBeneficiario;
            decimal importo = inputModel.Importo;
            DateTime dataScadenza = inputModel.DataScadenza;
            string? UserId = string.Empty;
            try
            {
                UserId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (NullReferenceException)
            {
                throw new UserUnknownException();
            }
            var scadenza = new Scadenza(dataScadenza,importo,denominazione);
            scadenza.IDUser = UserId;
            scadenza.IDBeneficiario = idBeneficiario;
            scadenza.Status="DA PAGARE";
            _dbContext.Add(scadenza);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exc)
            {
                throw new DbUpdateException(exc.Message);
            }
            return ScadenzaDetailViewModelInfo.FromEntity(scadenza);
        }

        public async Task<ScadenzaDetailViewModelInfo> EditScadenzaAsync(ScadenzaEditInputModel inputModel)
        {
            Scadenza scadenza = await _dbContext.Scadenze.FindAsync(inputModel.IdScadenza);
            
            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IdScadenza);
            }

            scadenza.Denominazione = inputModel.Denominazione;
            scadenza.IDBeneficiario = inputModel.IdBeneficiario;
            scadenza.IDUser = inputModel.IdUser;
            scadenza.DataScadenza = inputModel.DataScadenza;
            scadenza.Importo = inputModel.Importo;
            scadenza.DataPagamento = (DateTime)inputModel.DataPagamento;
            scadenza.GiorniRitardo = inputModel.GiorniRitardo;
            scadenza.Sollecito = inputModel.Sollecito;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OptimisticConcurrencyException();
            }
            catch (DbUpdateException exc)
            {
                throw new DbUpdateException(exc.Message);
            }
            return ScadenzaDetailViewModelInfo.FromEntity(scadenza);
        }
        public async Task<string> DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel)
        {
            Scadenza? scadenza = await _dbContext.Scadenze.FindAsync(inputModel.IdScadenza);
            if (scadenza.Status == "pagata".ToUpper())
                return "La scadenza risulta pagata. Impossibile eliminarla!";
            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IdScadenza);
            }
            _dbContext.Remove(scadenza);
            await _dbContext.SaveChangesAsync();
            this.InfoAdmin(inputModel.IdScadenza,scadenza.DataScadenza.Date,scadenza.Denominazione);
            return String.Empty;
        }
        private void InfoAdmin(int id, DateTime dataScadenza, string denominazione)
        {
            MailKitEmailSender emailSender = new MailKitEmailSender(_options, _logger, _configuration);
            emailSender.SendEmailAsync("admin@example.com", "Eliminazione Scadenza",
                "La scadenza con identificativo " + id + ",data scadenza " + dataScadenza + " e denominazione " + denominazione + " è stata eliminata!");
        }
        public List<SelectListItem> GetBeneficiari()
        {
            string IdUser;
            try
            {
                IdUser = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (NullReferenceException)
            {
                throw new UserUnknownException();
            }
            List<Beneficiario> beneficiari = new List<Beneficiario>();
            beneficiari = (from b in _dbContext.Beneficiari.Where(z => z.IdUser == IdUser) select b).ToList();
            var beneficiario = beneficiari.Select(b => new SelectListItem
            {
                Text = b.Denominazione,
                Value = b.IdBeneficiario.ToString()
            }).ToList();
            return beneficiario;
        }

        public string GetBeneficiarioById(int id)
        {
            _logger.LogInformation("Ricevuto identificativo beneficiario {id}", id);
            string Beneficiario = _dbContext.Beneficiari
                .Where(t => t.IdBeneficiario == id)
                .Select(t => t.Denominazione).Single();
            return Beneficiario;
        }
        //Calcolo giorni ritardo o giorni mancanti al pagamento
        public int DateDiff(DateTime inizio, DateTime fine)
        {
            int giorni = 0;
            giorni = (inizio.Date - fine.Date).Days;
            return giorni;
        }
        public bool IsDate(string date)
        {
            try
            {
                string[] formats = { "dd/MM/yyyy" };
                DateTime parsedDateTime;
                return DateTime.TryParseExact(date, formats, new CultureInfo("it-IT"), DateTimeStyles.None, out parsedDateTime);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncStripe(int id, string token)
        {
            return paymentGatewayStripe.CapturePaymentAsyncStripe(token);
        }

        public Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncPayPal(int id, string token)
        {
            return paymentGatewayPayPal.CapturePaymentAsyncPayPal(token);
        }

        public async Task<string> GetPaymentUrlAsyncPayPal(int scadenzaId)
        {
            ScadenzaDetailViewModel viewModel = await GetScadenzaAsync(scadenzaId);
            ScadenzaPayInputModel inputModel = null!;
            if (httpContextAccessor.HttpContext != null)
            {
                inputModel = new()
                {
                    IdScadenza = scadenzaId,
                    UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Description = viewModel.Denominazione,
                    Price = viewModel.Importo,
                    ReturnUrl = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: nameof(ScadenzeController.SubscribePayPal),
                        controller: "Scadenze",
                        values: new { id = scadenzaId }),
                    CancelUrl = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: nameof(ScadenzeController.Detail),
                        controller: "Scadenze",
                        values: new { id = scadenzaId })
                };
            }

            Debug.Assert(inputModel != null, nameof(inputModel) + " != null");
            return await paymentGatewayPayPal.GetPaymentUrlAsyncPayPal(inputModel);
        }

        public async Task<string> GetPaymentUrlAsyncStripe(int scadenzaId)
        {
            ScadenzaDetailViewModel viewModel = await GetScadenzaAsync(scadenzaId);
            ScadenzaPayInputModel inputModel = null!;
            if (httpContextAccessor.HttpContext != null)
            {
                inputModel = new()
                {
                    IdScadenza = scadenzaId,
                    UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Description = viewModel.Denominazione,
                    Price = viewModel.Importo,
                    ReturnUrl = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: nameof(ScadenzeController.SubscribeStripe),
                        controller: "Scadenze",
                        values: new { id = scadenzaId }),
                    CancelUrl = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: nameof(ScadenzeController.Detail),
                        controller: "Scadenze",
                        values: new { id = scadenzaId })
                };
            }

            Debug.Assert(inputModel != null, nameof(inputModel) + " != null");
            return await paymentGatewayStripe.GetPaymentUrlAsyncStripe(inputModel);
        }

        public async Task SubscribeScadenzaAsync(ScadenzaSubscribeInputModel inputModel)
        {
            if (inputModel.UserId != null)
            {
                Entities.Subscription subscription = new(inputModel.UserId, inputModel.IdScadenza)
                {
                    PaymentDate = inputModel.PaymentDate,
                    PaymentType = inputModel.PaymentType,
                    Paid = inputModel.Paid,
                    TransactionId = inputModel.TransactionId,
                };

                _dbContext.Subscriptions.Add(subscription);
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new Exception();
            }
            
            Scadenza scadenza = await _dbContext.Scadenze.FindAsync(inputModel.IdScadenza);
            
            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IdScadenza);
            }
            scadenza.DataPagamento=DateTime.Now;
            scadenza.Status = "PAGATA";
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OptimisticConcurrencyException();
            }
            catch (DbUpdateException exc)
            {
                throw new DbUpdateException(exc.Message);
            }
        }
    }
}