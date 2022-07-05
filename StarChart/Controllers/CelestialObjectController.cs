﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
            var match = _context.CelestialObjects.FirstOrDefault(c => c.Name == name);
            if (match is null) return NotFound();

            var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == match.Id).ToList();
            match.Satellites = satellites;
            return Ok(match);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var objects = _context.CelestialObjects.ToList();
            foreach(var celestialObject in objects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(objects);
        }
    }
}

