using Microsoft.EntityFrameworkCore;
using Models;
using WebApi.Data;

namespace GenericRepositories
{
    public class StudenteRepository : IStudenteRepository
    {
        private readonly ScuolaDbContext _context;

        public StudenteRepository(ScuolaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentiTest>> GetAllStudentiAsync()
        {
            List<StudentiTest> studente = await _context.Studente.ToListAsync();
            if (studente is null)
                return [];
            return studente;
        }

        public async Task<StudentiTest?> GetStudenteByIdAsync(int id)
        {
            return await _context.Studente.FindAsync(id);
        }

        public async Task<StudentiTest> CreateStudentiAsync(StudentiTest studente)
        {
            _context.Studente.Add(studente);
            await _context.SaveChangesAsync();
            return studente;
        }

        public async Task<StudentiTest> UpdateStudentiAsync(StudentiTest studente)
        {
            _context.Studente.Update(studente);
            await _context.SaveChangesAsync();
            return studente;
        }

        public async Task<bool> DeleteStudenteByIdAsync(int id)
        {
            StudentiTest? studente = await _context.Studente.FindAsync(id);

            if (studente == null)
                return false;

            _context.Studente.Remove(studente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
