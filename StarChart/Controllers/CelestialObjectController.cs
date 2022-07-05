using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var match = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (match is null) return NotFound();

            var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            match.Satellites = satellites;
            return Ok(match);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var matches = _context.CelestialObjects.Where(c => c.Name == name);
            if (matches is null || matches.Count() == 0) return NotFound();

            foreach (var match in matches)
            {
                match.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == match.Id).ToList();
            }

            return Ok(matches);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var objects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in objects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(objects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject model)
        {
            _context.CelestialObjects.Add(model);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject model)
        {
            var match = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (match is null) return NotFound();

            match.Name = model.Name;
            match.OrbitalPeriod = model.OrbitalPeriod;
            match.OrbitedObjectId = model.OrbitedObjectId;

            _context.CelestialObjects.Update(match);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var match = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (match is null) return NotFound();

            match.Name = name;
            _context.CelestialObjects.Update(match);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var matches = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);
            if (matches is null || matches.Count() == 0) return NotFound();

            _context.CelestialObjects.RemoveRange(matches);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

