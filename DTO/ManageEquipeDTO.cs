using System.ComponentModel.DataAnnotations;

namespace backend_projetdev.DTOs
{
    public class ManageEquipeDTO
    {
        [Required]
        public string Nom { get; set; }

        [Required]
        public int DepartementId { get; set; }
    }
}
