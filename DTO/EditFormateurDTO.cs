using System.ComponentModel.DataAnnotations;

namespace backend_projetdev.DTOs
{
    public class EditFormateurDTO
    {
        public string Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        public string Domaine { get; set; }

        [Required]
        public decimal Salaire { get; set; }

        [Required]
        public string Description { get; set; }

    }

}
