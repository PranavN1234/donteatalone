using API.Entities;

namespace API;

public interface ILikesService
{
    Task<UserLike> GetUserLike(int sourceId, int targetId);

    Task<Appuser> GetUserWithLikes(int userId);

    Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
}
