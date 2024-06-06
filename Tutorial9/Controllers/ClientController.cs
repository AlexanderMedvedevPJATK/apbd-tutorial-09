using Microsoft.AspNetCore.Mvc;
using Tutorial9.Context;

namespace Tutorial9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly TripsContext _context;
        
        public ClientController(TripsContext context)
        {
            _context = context;
        }
        
        [HttpDelete("{id:int}")]
        public IActionResult DeleteClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound("Client not found");
            }
            if (_context.ClientTrips.Any(ct => ct.IdClient == client.IdClient))
            {
                return BadRequest("Client has trips");
            }
            _context.Clients.Remove(client);
            _context.SaveChanges();
            return Ok("Client removed");
        }
    }
}
