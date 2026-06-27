namespace IndoorManagementAPI.DTOs.Ground
{
    public class GroundRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
    }
}
