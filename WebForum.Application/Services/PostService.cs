using WebForum.Application.Converters;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;

namespace WebForum.Application.Services;

public class PostService(IPostRepository postRepository, IUserRepository userRepository, ITagRepository tagRepository) : IPostService
{
    public async Task<IEnumerable<PostDto>> GetPostsAsync()
    {
        var posts = await postRepository.GetPostsAsync().ConfigureAwait(false);
        return posts.Select(PostConverter.MapToDto);
    }

    public async Task<PostDto> CreatePostAsync(string title, string body, Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createdPost = await postRepository.CreatePostAsync(post).ConfigureAwait(false);
        return PostConverter.MapToDto(createdPost);
    }

    public async Task AddCommentAsync(Guid postId, CreateCommentRequest request, Guid userId)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var user = await userRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Body = request.Body,
            User = user,
            Post = post
        };

        _ = await postRepository.AddCommentAsync(comment).ConfigureAwait(false);
    }

    public async Task AddLikeAsync(Guid postId, Guid userId)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var user = await userRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        if (post.UserId == userId)
        {
            throw new InvalidOperationException("Users cannot like their own posts");
        }

        var existingLike = await postRepository.GetLikeAsync(postId, userId).ConfigureAwait(false);
        if (existingLike != null)
        {
            return;
        }

        var like = new Like
        {
            PostId = postId,
            UserId = userId,
            Post = post,
            User = user
        };

        _ = await postRepository.AddLikeAsync(like).ConfigureAwait(false);

        post.LikeCount++;
        _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
    }

    public async Task RemoveLikeAsync(Guid postId, Guid userId)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var existingLike = await postRepository.GetLikeAsync(postId, userId).ConfigureAwait(false);
        if (existingLike == null)
        {
            return;
        }

        await postRepository.RemoveLikeAsync(postId, userId).ConfigureAwait(false);

        post.LikeCount = Math.Max(0, post.LikeCount - 1);
        _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
    }

    public async Task AddTagToPostAsync(Guid postId, AddTagToPostRequest request)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var existingTag = await tagRepository.GetByNameAsync(request.TagName).ConfigureAwait(false);
        if (existingTag != null)
        {
            if (!post.Tags.Any(t => t.Id == existingTag.Id))
            {
                post.Tags.Add(existingTag);
                _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
            }
        }
        else
        {
            var tagToCreate = new Tag { Name = request.TagName };
            var createdTag = await tagRepository.CreateAsync(tagToCreate).ConfigureAwait(false);
            post.Tags.Add(createdTag);
            _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
        }
    }
}
