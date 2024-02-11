using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAzureFileService
    {
        Task<string> SaveFileAsync(string fileName, byte[] bytes, string folderName);
        Task<byte[]> GetFileAsync(string fileUrl);
        Task DeleteFileAsync(string fileUrl);
    }
}
