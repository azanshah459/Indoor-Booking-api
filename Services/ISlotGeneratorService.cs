namespace IndoorManagementAPI.Services
{
    public interface ISlotGeneratorService
    {
        Task GenerateSlotsForDateAsync(int groundId, DateTime date);
    }
}
