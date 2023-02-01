using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;
using Application.Profiles;

namespace Application.Core
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        {
            string currentUsername = null;
            CreateMap<Activity, Activity>();

            CreateMap<Activity, ActivityDto>()
             .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees
             .FirstOrDefault(x => x.IsHost).AppUser.UserName));

            //CreateMap<ActivityAttendee, Profiles.Profile>()
            CreateMap<ActivityAttendee, AttendeeDto>()
             .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
             .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
             .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
            // .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
               .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
                .ForMember(d => d.Following, // we want to know if the currently logged in user inside that follower's collection as a follower of this particular user
                                             //that means we need ti get access to our currently logged in user information from our token 
                                             // so wen need to do is get our currentUserName acrross to our configuration when we are using the projection 
                    o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));

            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(d => d.Image, s => s.MapFrom(o => o.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
                // we want to know if the currently logged in user inside that follower's collection as a follower of this particular user
                //that means we need ti get access to our currently logged in user information from our token 
                // so wen need to do is get our currentUserName acrross to our configuration when we are using the projection 
                .ForMember(d => d.Following,
                    o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));

            CreateMap<Comment, CommentDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
            .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));

            CreateMap<ActivityAttendee, UserActivityDto>()
               .ForMember(d => d.Id, o => o.MapFrom(s => s.Activity.Id))
               .ForMember(d => d.Date, o => o.MapFrom(s => s.Activity.Date))
               .ForMember(d => d.Title, o => o.MapFrom(s => s.Activity.Title))
               .ForMember(d => d.Category, o => o.MapFrom(s => s.Activity.Category))
               .ForMember(d => d.HostUsername, o => o.MapFrom(s =>
                   s.Activity.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
        }
    }
}