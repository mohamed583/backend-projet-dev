using System.ComponentModel.DataAnnotations;

namespace backend_projetdev.DTOs
{
    public class RegisterFormateurDTO
    {
        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Domaine { get; set; }

        [Required]
        public decimal Salaire { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation doivent correspondre.")]
        public string ConfirmPassword { get; set; }
    }

}
