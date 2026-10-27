using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionVideoClub.Infrastructure.Persistence.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(VideoClubDbContext context, IPasswordHashService hasher)
        {
            if (await context.Movies.AnyAsync())
            {
                return;
            }

            var today = DateTime.Today;

            var vhs = new Format("VHS", 1, 0);
            var dvd = new Format("DVD", 2, 1);
            var bluray = new Format("Blu-ray", 3, 3);
            var uhd4k = new Format("4K UHD", 4, 5);
            context.Formats.AddRange(vhs, dvd, bluray, uhd4k);

            var inception = new Movie("tt1375666", "Inception", "Action, Adventure, Sci-Fi", 148, 13, 2010, null, null);
            var matrix = new Movie("tt0133093", "The Matrix", "Action, Sci-Fi", 136, 16, 1999, null, null);
            var interstellar = new Movie("tt0816692", "Interstellar", "Adventure, Drama, Sci-Fi", 169, 13, 2014, null, null);
            var godfather = new Movie("tt0068646", "The Godfather", "Crime, Drama", 175, 16, 1972, null, null);
            var pulpFiction = new Movie("tt0110912", "Pulp Fiction", "Crime, Drama", 154, 18, 1994, null, null);
            var secretoDeSusOjos = new Movie("tt1305806", "El secreto de sus ojos", "Drama, Mystery, Thriller", 129, 16, 2009, null, null);
            var relatosSalvajes = new Movie("tt3011894", "Relatos salvajes", "Comedy, Drama, Thriller", 122, 16, 2014, null, null);
            // Movie.Clasification exige un valor > 0 y el dominio no tiene un código dedicado para
            // "ATP" (apta para todo público), así que se usa 1 como la clasificación más baja posible.
            var toyStory = new Movie("tt0114709", "Toy Story", "Animation, Adventure", 81, 1, 1995, null, null);

            context.Movies.AddRange(inception, matrix, interstellar, godfather, pulpFiction, secretoDeSusOjos, relatosSalvajes, toyStory);

            var incCopy1 = new Copy("INC-DVD-01", Copy.CopyState.healthy, "Estante A1", true, inception, dvd);
            var incCopy2 = new Copy("INC-BR-01", Copy.CopyState.healthy, "Estante A1", true, inception, bluray);
            var matCopy1 = new Copy("MAT-BR-01", Copy.CopyState.healthy, "Estante A2", true, matrix, bluray);
            var matCopy2 = new Copy("MAT-4K-01", Copy.CopyState.healthy, "Estante A2", true, matrix, uhd4k);
            var intCopy1 = new Copy("INT-4K-01", Copy.CopyState.healthy, "Estante A3", true, interstellar, uhd4k);
            var intCopy2 = new Copy("INT-VHS-01", Copy.CopyState.healthy, "Estante A3", true, interstellar, vhs);
            var godCopy1 = new Copy("GOD-VHS-01", Copy.CopyState.healthy, "Estante B1", true, godfather, vhs);
            var godCopy2 = new Copy("GOD-DVD-01", Copy.CopyState.healthy, "Estante B1", true, godfather, dvd);
            var pulCopy1 = new Copy("PUL-DVD-01", Copy.CopyState.healthy, "Estante B2", true, pulpFiction, dvd);
            var pulCopy2 = new Copy("PUL-BR-01", Copy.CopyState.healthy, "Estante B2", true, pulpFiction, bluray);
            var secCopy1 = new Copy("SEC-BR-01", Copy.CopyState.healthy, "Estante C1", true, secretoDeSusOjos, bluray);
            var secCopy2 = new Copy("SEC-4K-01", Copy.CopyState.healthy, "Estante C1", true, secretoDeSusOjos, uhd4k);
            var relCopy1 = new Copy("REL-4K-01", Copy.CopyState.healthy, "Estante C2", true, relatosSalvajes, uhd4k);
            var relCopy2 = new Copy("REL-VHS-01", Copy.CopyState.damaged, "Estante C2", false, relatosSalvajes, vhs);
            var toyCopy1 = new Copy("TOY-VHS-01", Copy.CopyState.healthy, "Estante D1", true, toyStory, vhs);
            var toyCopy2 = new Copy("TOY-DVD-01", Copy.CopyState.damaged, "Estante D1", false, toyStory, dvd);

            context.Copies.AddRange(
                incCopy1, incCopy2, matCopy1, matCopy2, intCopy1, intCopy2, godCopy1, godCopy2,
                pulCopy1, pulCopy2, secCopy1, secCopy2, relCopy1, relCopy2, toyCopy1, toyCopy2);

            var employee1 = new Employee("Juan", "Pérez", 29111222, "3415551111", "San Luis 1200, Rosario", "juan.perez@videoclub.com", hasher.Hash("Empleado123!"), "Mañana", 450000m);
            var employee2 = new Employee("María", "Gómez", 31222333, "3415552222", "Urquiza 1300, Rosario", "maria.gomez@videoclub.com", hasher.Hash("Empleado123!"), "Tarde", 470000m);
            context.Employees.AddRange(employee1, employee2);

            var admin = await context.Employees.FirstAsync(e => e.Email == "admin@videoclub.com");

            var client1 = new Client("Carlos", "Fernández", 30111222, "3411234567", "Pellegrini 1234, Rosario", "carlos.fernandez@mail.com", hasher.Hash("Cliente123!"));
            var client2 = new Client("Lucía", "Martínez", 32222333, "3412345678", "Córdoba 2345, Rosario", "lucia.martinez@mail.com", hasher.Hash("Cliente123!"));
            var client3 = new Client("Martín", "Gómez", 28333444, "3413456789", "San Martín 3456, Rosario", "martin.gomez@mail.com", hasher.Hash("Cliente123!"));
            var client4 = new Client("Florencia", "López", 35444555, "3414567890", "Entre Ríos 4567, Rosario", "florencia.lopez@mail.com", hasher.Hash("Cliente123!"));
            var client5 = new Client("Diego", "Rodríguez", 27555666, "3415678901", "Mitre 5678, Rosario", "diego.rodriguez@mail.com", hasher.Hash("Cliente123!"));
            context.Clients.AddRange(client1, client2, client3, client4, client5);

            // Rentas activas: la copia queda marcada como no disponible mientras dura el alquiler.
            var rent1 = new Rent(today.AddDays(-1), today.AddDays(5), incCopy1, client1, admin);
            incCopy1.SetAvailability(false);

            var rent2 = new Rent(today.AddDays(-2), today.AddDays(4), matCopy1, client2, employee1);
            matCopy1.SetAvailability(false);

            // Rentas devueltas a tiempo: se devuelven antes de la fecha esperada y la copia vuelve a estar disponible.
            var rent3 = new Rent(today.AddDays(-10), today.AddDays(-7), intCopy1, client3, employee2);
            rent3.ReturnCopy(Rent.RentState.Returned, today.AddDays(-8));
            intCopy1.SetAvailability(true);

            var rent4 = new Rent(today.AddDays(-10), today.AddDays(-7), godCopy1, client4, admin);
            rent4.ReturnCopy(Rent.RentState.Returned, today.AddDays(-8));
            godCopy1.SetAvailability(true);

            // Rentas devueltas tarde: la devolución ocurre después de la fecha esperada (Overdue).
            var rent5 = new Rent(today.AddDays(-10), today.AddDays(-7), pulCopy1, client5, employee1);
            rent5.ReturnCopy(Rent.RentState.Overdue, today.AddDays(-2));
            pulCopy1.SetAvailability(true);

            var rent6 = new Rent(today.AddDays(-12), today.AddDays(-7), secCopy1, client1, employee2);
            rent6.ReturnCopy(Rent.RentState.Overdue, today.AddDays(-1));
            secCopy1.SetAvailability(true);

            context.Rents.AddRange(rent1, rent2, rent3, rent4, rent5, rent6);

            var fine1 = new Fine(Fine.FineMotive.LateReturn, 500m, today.AddDays(-2), rent5);

            var fine2 = new Fine(Fine.FineMotive.LateReturn, 650m, today.AddDays(-1), rent6);
            fine2.PayFine();

            context.Fines.AddRange(fine1, fine2);

            await context.SaveChangesAsync();
        }
    }
}
