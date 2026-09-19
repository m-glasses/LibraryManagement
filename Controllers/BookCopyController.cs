using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class BookCopyController : Controller
    {
        private readonly IBookCopyService _bookCopyService;
        public BookCopyController(IBookCopyService bookCopyService)
        {
            _bookCopyService = bookCopyService;
        }
        // GET: BookCopyController
        public IActionResult Index()
        {
            var bookCopies = _bookCopyService.GetAll();
            return View(bookCopies);
        }

        // GET: BookCopyController/Details/5
        public IActionResult Details(int id)
        {
            var bookCopy = _bookCopyService.GetById(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            return View(bookCopy);
        }

        // GET: BookCopyController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookCopyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookCopy bookCopy)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _bookCopyService.Create(bookCopy);
                    return RedirectToAction(nameof(Index));
                }
                return View(bookCopy);
            }
            catch
            {
                return View();
            }
        }

        // GET: BookCopyController/Edit/5
        public IActionResult Edit(int id)
        {
            var bookCopy = _bookCopyService.GetById(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            return View(bookCopy);
        }

        // POST: BookCopyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BookCopy bookCopy)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isValid = _bookCopyService.Update(bookCopy);
                    if (!isValid)
                    {
                        return View(bookCopy);
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(bookCopy);
            }
            catch
            {
                return View();
            }
        }

        // GET: BookCopyController/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var bookCopy = _bookCopyService.GetById(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            return View(bookCopy);
        }

        // POST: BookCopyController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool isValid = _bookCopyService.Delete(id);
                if (!isValid)
                {
                    var bookCopy = _bookCopyService.GetById(id);
                    return View(bookCopy);
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

