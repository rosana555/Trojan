using System.Collections.Generic;
using System.Threading.Tasks;
using Trojan.Core.Models;

namespace Trojan.Core.Interfaces
{
    public interface IAlbumService
    {
        Task<List<Album>> GetAlbumsAsync();

        Task<Album> CreateAlbumAsync(string title);

        Task DeleteAlbumAsync(int albumId);

        Task AddImageToAlbumAsync(int albumId, int galleryItemId);

        Task RemoveImageFromAlbumAsync(int albumId, int galleryItemId);

        Task SaveChangesAsync();
    }
}