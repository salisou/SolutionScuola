using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using WebApi.Data;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentiController : ControllerBase
    {
        private readonly ScuolaDbContext _context;

        public StudentiController(ScuolaDbContext context)
        {
            _context = context;
        }

        // GET: api/Studenti
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentiTest>>> GetAllStudentiAsync()
        {
            try
            {
                List<StudentiTest> ListaStudenti = await _context.Studente.ToListAsync();
                if (ListaStudenti is null)
                    NotFound("⚠️Attenzione la tabella è vuota!");
                return Ok(ListaStudenti);
            }
            catch (Exception ex)
            {
                NotFound("Errore di caricamento della Tabella Studente");
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<StudentiTest>> GetStudenteByIdAsync(int id)
        {
            try
            {
                var st = await _context.Studente.FindAsync(id);
                if (st is null) NotFound("Questo studente non esite");
                return Ok(st);
            }
            catch (Exception ex)
            {
                return BadRequest($"Attenzione Verifica lo stato del Serve {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentiAsync(StudentiTest studente)
        {

            // Aggiunta dello studente al database
            _context.Studente.Add(studente);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetStudenteByIdAsync), new
            {
                id = studente.StudenteId
            }, studente);
        }

        [HttpPut]
        public async Task<ActionResult<StudentiTest>> UpdateStudentiAsync(StudentiTest studente)
        {
            _context.Studente.Update(studente);
            await _context.SaveChangesAsync();
            return Ok(studente);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudenteByIdAsync(int id)
        {
            StudentiTest? studente = await _context.Studente.FindAsync(id);

            if (studente == null)
                return NotFound("Id non valido");

            _context.Studente.Remove(studente);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
