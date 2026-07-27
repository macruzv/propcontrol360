using propcontrol360.Models;

namespace propcontrol360.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int ReservedOrSoldProperties { get; set; }
        public int TotalClients { get; set; }
        public int TotalAgents { get; set; }
        public decimal TotalSalesVolume { get; set; }
        public decimal TotalCommissionsPaid { get; set; }

        public List<Property> RecentProperties { get; set; } = new List<Property>();
        public List<Client> RecentLeads { get; set; } = new List<Client>();
        public List<Agent> TopAgents { get; set; } = new List<Agent>();
        public List<Contract> ActiveContracts { get; set; } = new List<Contract>();
    }
}
