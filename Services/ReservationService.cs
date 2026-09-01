using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class ReservationService : GenericService<Reservation> , IReservationService 
    {
        private readonly LibraryDbContext _context;
        public ReservationService(LibraryDbContext context) : base(context) 
        {
            _context = context;
        }
    }
}
