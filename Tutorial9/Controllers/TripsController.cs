using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial9.Context;
using Tutorial9.DTOs;
using Tutorial9.Models;

namespace Tutorial9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly TripsContext _context;
        
        public TripsController(TripsContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            var total = _context.Trips.Count();
            var totalPages = (int)Math.Ceiling((double)total / pageSize);
            
            var trips = _context.Trips
                .OrderByDescending(trip => trip.DateFrom)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            // Create a PaginationTripsDto object
            var result = new PaginationTripsDto
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Trips = trips
            };

            return Ok(result);
        }
        
        [HttpPost("{idTrip:int}/clients")]
        public IActionResult AddClientToTrip(int idTrip, ClientDto clientDto)
        {
            if (_context.Clients.Any(c => c.Pesel == clientDto.Pesel))
            {
                return BadRequest("Client with such PESEL already exists");
            }
            
            var tripClients = _context.Clients.Where(
                c => _context.ClientTrips.Any(ct => ct.IdClient == c.IdClient));
            
            if (tripClients.Any(c => c.Pesel == clientDto.Pesel))
            {
                return BadRequest("Client with such PESEL already added to trip");
            }

            
            var trip = _context.Trips.Find(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            if (trip.DateFrom < DateTime.Now)
            {
                return BadRequest("Trip already started");
            }
            
            var client = new Client
            {
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                Email = clientDto.Email,
                Telephone = clientDto.Telephone,
                Pesel = clientDto.Pesel
            };
            
            _context.Clients.Add(client);
            _context.SaveChanges();
            
            _context.ClientTrips.Add(new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = trip.IdTrip,
                PaymentDate = clientDto.PaymentDate,
                RegisteredAt = DateTime.Now
            });
            _context.SaveChanges();
            return Ok("Client added to trip");
        }
        
        
    }
}
