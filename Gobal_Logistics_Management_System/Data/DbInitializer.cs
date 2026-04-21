using Global_Logistics_Management_System.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Global_Logistics_Management_System.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // Seed Clients
            if (!await context.Clients.AnyAsync())
            {
                var clients = new List<Client>
                {
                    new Client
                    {
                        Name = "Acme Global Logistics",
                        ContactDetails = "contact@acmelogistics.com | +27 11 555 1234",
                        Region = "Gauteng"
                    },
                    new Client
                    {
                        Name = "TransOcean Freight",
                        ContactDetails = "operations@transocean.co.za | +27 21 555 6789",
                        Region = "Western Cape"
                    },
                    new Client
                    {
                        Name = "EuroCargo Express",
                        ContactDetails = "info@eurocargo.eu | +49 30 555 4321",
                        Region = "Europe"
                    },
                    new Client
                    {
                        Name = "AsiaPac Shipping Ltd",
                        ContactDetails = "apac@asiapac.sg | +65 6123 4567",
                        Region = "Asia-Pacific"
                    },
                    new Client
                    {
                        Name = "Americas Freight Solutions",
                        ContactDetails = "dispatch@americasfreight.com | +1 305 555 9876",
                        Region = "North America"
                    },
                    new Client
                    {
                        Name = "Durban Harbor Carriers",
                        ContactDetails = "durban@harborcarriers.co.za | +27 31 555 3456",
                        Region = "KwaZulu-Natal"
                    },
                    new Client
                    {
                        Name = "Middle East Cargo Group",
                        ContactDetails = "info@mecargo.ae | +971 4 555 7890",
                        Region = "Middle East"
                    },
                    new Client
                    {
                        Name = "Southern Cross Logistics",
                        ContactDetails = "admin@southerncross.com.au | +61 2 555 1122",
                        Region = "Australia"
                    },
                    new Client
                    {
                        Name = "Latin American Shipping Co.",
                        ContactDetails = "ventas@lashipping.com.br | +55 11 555 3344",
                        Region = "South America"
                    },
                    new Client
                    {
                        Name = "TechMove Corporate",
                        ContactDetails = "corporate@techmove.com | +27 10 555 0000",
                        Region = "Gauteng"
                    }
                };

                await context.Clients.AddRangeAsync(clients);
                await context.SaveChangesAsync();
            }

            // a sample contract for testing 
            
            if (!await context.Contracts.AnyAsync())
            {
                var firstClient = await context.Clients.FirstAsync();
                var sampleContract = new Contract
                {
                    ClientId = firstClient.ClientId,
                    StartDate = DateTime.Today.AddDays(-30),
                    EndDate = DateTime.Today.AddDays(335),
                    Status = ContractStatus.Active,
                    ServiceLevel = "Premium",
                    SignedAgreementPath = null
                };
                await context.Contracts.AddAsync(sampleContract);
                await context.SaveChangesAsync();
            }
            
        }
    } 
}

