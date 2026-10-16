using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository fineRepository;
        private readonly IRentRepository rentRepository;

        public FineService(IFineRepository fineRepository, IRentRepository rentRepository)
        {
            this.fineRepository = fineRepository;
            this.rentRepository = rentRepository;
        }

        public Fine AddFine(CreateFineRequest request)
        {
            var rent = rentRepository.GetRentById(request.RentId)
                ?? throw new KeyNotFoundException("Rent not found.");

            var fine = new Fine(request.Motive, request.Amount, request.Date, rent);
            fineRepository.AddFine(fine);
            fineRepository.SaveChanges();
            return fine;
        }

        public IReadOnlyList<Fine> GetAllFines() => fineRepository.GetAllFines();

        public Fine? GetFineById(Guid id) => fineRepository.GetFineById(id);

        public bool PayFine(Guid id)
        {
            var fine = fineRepository.GetFineById(id);
            if (fine == null)
            {
                return false;
            }

            try
            {
                fine.PayFine();
                fineRepository.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
