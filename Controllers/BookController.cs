using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILoanService _loanService;
        private readonly UserManager<User> _userManager;
        public BookController(IBookService bookService, ILoanService loanService, UserManager<User> userManager)
        {
            _bookService = bookService;
            _loanService = loanService;
            _userManager = userManager;
        }
        // GET: BookController
        public IActionResult Index()
        {
            var books = _bookService.GetBookList();
            return View(books);
        }

        // GET: BookController/Details/5
        public IActionResult Details(int id)
        {
            var book = _bookService.GetBookDetails(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // GET: BookController/Create
        public IActionResult Create()
        {
            return View();
        }




        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _bookService.Create(book);
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Edit/5
        public IActionResult Edit(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isValid = _bookService.Update(book);
                    if (!isValid)
                    {
                        return View(book);
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool isValid = _bookService.Delete(id);
                if (!isValid)
                {
                    var book = _bookService.GetById(id);
                    return View(book);
                }
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View();
            }
        }
    }
}

