﻿using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _context;


        public RaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Race race)
        {
            _context.Add(race);
            return Save();
        }

        public async Task<bool> Delete(int id)
        {
            var race = await GetByIdAsync(id);
            if (race != null)
            {
                _context.Remove(race);
            }
            return Save();
        }

        public async Task<IEnumerable<Race>> GetAll()
        {
            return await _context.Races.ToListAsync();

        }


        public async Task<Race> GetByIdAsync(int id)
        {
            return await _context.Races.Include(item => item.Address).FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<Race> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Races.Include(item => item.Address).AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Race>> GetRacesByCity(string city)
        {
            return await _context.Races.Where(c => c.Address.City.Contains(city)).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Race race)
        {
            _context.Update(race);
            return Save();
        }
    }
}
