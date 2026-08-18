using propcontrol360.Models;

namespace propcontrol360.ViewModels
{
    public class LandingPageViewModel
    {
        public List<Property> FeaturedProperties { get; set; } = new List<Property>();
        public List<Property> AllProperties { get; set; } = new List<Property>();
        public List<Property> IndividualProperties { get; set; } = new List<Property>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Agent> ActiveAgents { get; set; } = new List<Agent>();

        // Filtros de búsqueda
        public string? SelectedCategory { get; set; }
        public string? SearchTerm { get; set; }
        public decimal? MaxPrice { get; set; }

        // Estadísticas rápidas para la Landing
        public int TotalTerrenosLotes { get; set; }
        public int TotalCasas { get; set; }
        public int TotalApartamentos { get; set; }
        public int TotalBloques { get; set; }
    }
}
