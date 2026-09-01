using LibraryManagement._ٰViewModels;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IUserService _userService;
        private readonly IBookCopyService _bookCopyService;
        public LoanController(ILoanService loanService , IUserService userService , IBookCopyService bookCopyService)
        {
            _loanService = loanService;
            _userService = userService;
            _bookCopyService = bookCopyService;
        }

        // GET: LoanController
        public IActionResult Index()
        {
            var loans = _loanService.GetAll();

            if (loans == null)
            {
                return NotFound();
            }

            return View(loans);
        }

        // GET: LoanController/Details/5
        public IActionResult Details(int id)
        {
            var loan = _loanService.GetById(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        public IActionResult Borrow()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow(int userId, int bookCopyId)
        {
            if (ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var loan = _loanService.Borrow(userId, bookCopyId);
                return View(loan);

            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }

        }
    }
}
