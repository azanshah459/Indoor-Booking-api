namespace IndoorManagementAPI.DTOs.Ground
{
    public class GroundResponseDto
    {
        public int GroundId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
    }
}
