using backend_projetdev.Models;
using System.ComponentModel.DataAnnotations;

namespace backend_projetdev.DTOs
{
    public class ManagePosteDTO
    {
        [Required]
        public string Nom { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public StatutPoste StatutPoste { get; set; }
    }

}
