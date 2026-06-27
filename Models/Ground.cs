namespace IndoorManagementAPI.Models
{
    public class Ground
    {
        public int GroundId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Cricket or Futsal
        public string Description { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
    }
}
