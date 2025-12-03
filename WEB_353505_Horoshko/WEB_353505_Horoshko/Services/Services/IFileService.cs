namespace WEB_353505_Horoshko.Services.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
