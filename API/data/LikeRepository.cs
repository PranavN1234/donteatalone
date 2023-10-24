using API.data;
using API.Entities;
using API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikeRepository : ILikesService
{   
    private readonly DataContext _context;

    public LikeRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UserLike> GetUserLike(int sourceId, int targetId)
    {
        return await _context.Likes.FindAsync(sourceId, targetId);
    }

    public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
    {
        var users  = _context.Users.OrderBy(u=>u.UserName).AsQueryable();

        var likes = _context.Likes.AsQueryable();

        if(likesParams.Predicate == "liked"){
            likes = likes.Where(like=>like.SourceId == likesParams.UserId);

            users = likes.Select(like=>like.TargetUser);
        }
        if(likesParams.Predicate == "likedBy"){
            likes = likes.Where(like=>like.TargetId == likesParams.UserId);

            users = likes.Select(like=>like.SourceUser);
        }

        var likedUsers =  users.Select(user=>new LikeDTO{
            UserName = user.UserName,
            Knownas = user.Knownas,
            Age = user.DateofBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<Appuser> GetUserWithLikes(int userId)
    {
        return await _context.Users
        .Include(x => x.LikedUsers)
        .FirstOrDefaultAsync(x => x.Id == userId);

    }
}
