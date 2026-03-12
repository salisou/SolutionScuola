using Models;

namespace GenericRepositories
{
    public interface IStudenteRepository
    {
        Task<IEnumerable<StudentiTest>> GetAllStudentiAsync();
        Task<StudentiTest?> GetStudenteByIdAsync(int id);
        Task<StudentiTest> CreateStudentiAsync(StudentiTest studente);
        Task<StudentiTest> UpdateStudentiAsync(StudentiTest studente);
        Task<bool> DeleteStudenteByIdAsync(int id);
    }
}
