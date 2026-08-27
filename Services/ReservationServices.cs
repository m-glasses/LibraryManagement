using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class ReservationServices : GenericService<Reservation> , IReservationService 
    {
        private readonly LibraryDbContext _context;
        public ReservationServices(LibraryDbContext context) : base(context) 
        {
            _context = context;
        }
    }
}
