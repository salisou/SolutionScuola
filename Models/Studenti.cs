using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Models
{
    [Table("Studente")]
    public class StudentiTest
    {
        [Key, NotNull]
        public int StudenteId { get; set; }

        [MaxLength(50), NotNull]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(50), NotNull]
        public string Cognome { get; set; } = string.Empty;

        [NotNull]
        public int Eta { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }
    }
}
