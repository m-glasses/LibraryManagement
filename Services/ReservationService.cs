using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class ReservationService : GenericService<Reservation> , IReservationService 
    {
        private readonly LibraryDbContext _context;
        public ReservationService(LibraryDbContext context) : base(context) 
        {
            _context = context;
        }


        public Reservation Reserve(int userId, int bookId)
        {
            CanReserveBook(userId, bookId);

            var settings = _context.LibrarySettings.SingleOrDefault();

            if (settings is null)
            {
                throw new InvalidOperationException("Library settings not found");
            }

            var reservedAt = DateTime.Now;

            var reservation = new Reservation
            {
                UserId = userId,
                BookId = bookId,
                ReservedAt = reservedAt,
                ExpiresAt = reservedAt.AddDays(settings.ReservationDurationDays),
                ReservationStatus = ReservationStatus.Active
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return reservation;
        }

        public void CanReserveBook(int userId, int bookId)
        {
            var userExists = _context.Users.Any(user => user.Id == userId);

            if (!userExists)
            {
                throw new InvalidOperationException("User not found");
            }

            var bookExists = _context.Books.Any(book => book.Id == bookId);

            if (!bookExists)
            {
                throw new InvalidOperationException("Book not found");
            }

            var hasAvailableCopy = _context.BookCopies.Any(bookCopy =>
                bookCopy.BookId == bookId &&
                !bookCopy.Loans.Any(loan => loan.LoanStatus == LoanStatus.Active));

            if (hasAvailableCopy)
            {
                throw new InvalidOperationException("The book is currently available");
            }

            var hasActiveLoan = _context.Loans.Any(loan =>
                loan.UserId == userId &&
                loan.BookCopy.BookId == bookId &&
                loan.LoanStatus == LoanStatus.Active);

            if (hasActiveLoan)
            {
                throw new InvalidOperationException(
                    "You already have this book on loan");
            }

            var hasActiveReservation = _context.Reservations.Any(reservation =>
                reservation.UserId == userId &&
                reservation.BookId == bookId &&
                reservation.ReservationStatus == ReservationStatus.Active);

            if (hasActiveReservation)
            {
                throw new InvalidOperationException(
                    "You already have an active reservation for this book");
            }
        }

        public Reservation? GetDetailsById(int reservationId, int userId)
        {
            return _context.Reservations
                .AsNoTracking()
                .Include(reservation => reservation.Book)
                .SingleOrDefault(reservation =>
                    reservation.Id == reservationId &&
                    reservation.UserId == userId);
        }

        public List<Reservation> GetUserReservations(int userId)
        {
            return _context.Reservations
                .AsNoTracking()
                .Where(reservation => reservation.UserId == userId)
                .Include(reservation => reservation.Book)
                .ToList();
        }

        public void CancelReservation(int reservationId, int userId)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(reservation =>
                    reservation.Id == reservationId &&
                    reservation.UserId == userId &&
                    reservation.ReservationStatus == ReservationStatus.Active);

            if (reservation is null)
            {
                throw new InvalidOperationException(
                    "Active reservation not found.");
            }

            reservation.ReservationStatus = ReservationStatus.Cancelled;

            _context.SaveChanges();
        }
    }
}
