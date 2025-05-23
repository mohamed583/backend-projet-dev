using System.ComponentModel.DataAnnotations;

namespace backend_projetdev.DTOs
{
        public class ManageDepartementDTO
        {
            [Required]
            public string Nom { get; set; }
        }

    
}
