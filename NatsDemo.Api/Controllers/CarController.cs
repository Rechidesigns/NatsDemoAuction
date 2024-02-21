using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatsDemo.Core.Data;
using NatsDemo.Core.Model;

namespace NatsDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly NatsDemoDbContext _context;

        public CarController(NatsDemoDbContext context)
        {
            _context = context;
        }

        // Add a new car
        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            car.Id = Guid.NewGuid().ToString();
            _context.Carss.Add(car);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        // Update an existing car
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(Guid id, [FromBody] Car updatedCar)
        {
            var car = await _context.Carss.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.Make = updatedCar.Make;
            car.Model = updatedCar.Model;
            car.Year = updatedCar.Year;
            // Update other fields as necessary

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Get details of a specific car
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(Guid id)
        {
            var car = await _context.Carss.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return Ok(car);
        }

        // Delete a car
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            var car = await _context.Carss.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Carss.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // List all cars
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _context.Carss.ToListAsync();
            return Ok(cars);
        }
    }
}
