namespace backend_projetdev.Models
{
    public class Formateur : Personne
    {
        public decimal Salaire { get; set; }
        public string Domaine { get; set; }
        public string Description { get; set; }
        public ICollection<Formation> Formations { get; set; }
    }
}
