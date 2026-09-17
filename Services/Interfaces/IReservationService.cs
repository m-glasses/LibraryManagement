using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IReservationService : IGenericService<Reservation>
    {
        Reservation Reserve(int userId, int bookId);
        void CanReserveBook(int userId, int bookId);
        Reservation? GetDetailsById(int reservationId, int userId);
        List<Reservation> GetUserReservations(int userId);
        void CancelReservation(int reservationId, int userId);
    }
}
