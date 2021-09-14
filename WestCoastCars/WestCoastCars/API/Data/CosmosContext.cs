using API.DocumentEntities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class CosmosContext : DbContext
  {
    public DbSet<Valuation> Valuations { get; set; }
    public CosmosContext(DbContextOptions<CosmosContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //Ange namn på den Container/Collection som är skapad i Azure
      //För Cosmos DB databasen
      modelBuilder.HasDefaultContainer("Valuations");

      //Anger att Valuation dokumentet har ett inbäddat dokument(Vehicle)
      //och endast ett dokument av typen Vehicle kan förekomma.
      //Exempel...
      /*
        {
            "id": "8c270a03-57df-4f8f-9149-5ba73893be61",
            "valuationDate": "2021-05-10T14:59:05.7688291",
            "status": "New",
            "valuationType": "Normal",
            "value": 0,
            "vehicle": {
                "registrationNo": "YF13947",
                "make": "VOLVO",
                "model": "V90",
                "mileage": 0,
                "modelYear": "2018"
            }
        }
      */
      modelBuilder.Entity<Valuation>()
      .OwnsOne(c => c.Vehicle);

      base.OnModelCreating(modelBuilder);
    }
  }
}