using LibraryManagement.Configuration;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.Services.ZarinPal;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Controllers
{
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly UserManager<User> _userManager;
        private readonly IZarinPalService _zarinPal;
        private readonly IOptions<ZarinPalOptions> _options;
        private readonly IPaymentAttemptService _payment;

        public WalletController(IWalletService walletService, UserManager<User> userManager , IZarinPalService zarin , IOptions<ZarinPalOptions> option , IPaymentAttemptService payment)
        {
            _walletService = walletService;
            _userManager = userManager;
            _zarinPal = zarin;
            _options = option;
            _payment = payment;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var wallet = _walletService.GetByUserId(user.Id);

            if (wallet == null)
            {
                return NotFound();
            }

            var transactions = _walletService.GetTransactions(wallet.Id);

            var walletViewModel = new WalletViewModel
            {
                Balance = wallet.Balance,
                Transactions = transactions.Select(transaction => new WalletTransactionListViewModel
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    CreatedAt = transaction.CreatedAt
                }).ToList()
            };

            return View(walletViewModel);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Deposit(WalletDepositViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                    return Unauthorized();

                var paymentAttempt = _payment.Create(user.Id, model.Amount);

                var callbackUrl = Url.Action("Callback","Wallet",values: null,protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(callbackUrl))
                    throw new InvalidOperationException("Callback URL could not be generated");

                var paymentResult = await _zarinPal.RequestPaymentAsync(model.Amount,callbackUrl);

                if (string.IsNullOrWhiteSpace(paymentResult.Authority))
                    throw new InvalidOperationException("Payment authority was not received from ZarinPal.");

                _payment.SetAuthority(paymentAttempt,paymentResult.Authority);

                var paymentUrl = _zarinPal.GetPaymentUrl(paymentResult.Authority);

                return Redirect(paymentUrl);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] =
                    Helper.ErrorMessageHelper.Translate(ex.Message);

                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Callback(ZarinPalCallbackViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Authority))
                return BadRequest("Authority is missing");

            if (string.IsNullOrWhiteSpace(model.Status))
                return BadRequest("Payment status is missing");

            var paymentAttempt = _payment.GetByAuthority(model.Authority);

            if (paymentAttempt is null)
                return NotFound("Payment attempt not found");

            if (paymentAttempt.Status != PaymentStatus.Pending)
                return BadRequest("Payment has already been processed");

            if (!string.Equals(model.Status, "OK", StringComparison.OrdinalIgnoreCase))
            {
                _payment.SetStatus(paymentAttempt, PaymentStatus.Failed);

                return BadRequest("Payment was not successful.");
            }

            var paymentResult = await _zarinPal.VerifyPaymentAsync(paymentAttempt.Authority!,paymentAttempt.Amount);

            if (paymentResult.Code != 100 && paymentResult.Code != 101)
            {
                _payment.SetStatus(paymentAttempt, PaymentStatus.Failed);

                return BadRequest("Payment verification failed.");
            }

            _payment.CompletePayment(paymentAttempt);

            return Ok(new
            {
                paymentAttempt.Authority,
                paymentResult.Code
            });

        }

    }
}
