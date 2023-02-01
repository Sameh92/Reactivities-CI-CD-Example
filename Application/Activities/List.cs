using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{

    /*because this is logic inside our application project there is no way that our code on its own could get this information to our 
    API controller */
    public class List
    {
        // we tell our request this IRequest what object is going to be returning from this query and it's going to be a list of activites
        // public class Query : IRequest<Result<List<ActivityDto>>>
        //  {

        /*if we needed to send any data from the API such as an id of an activity or other information then
        then we will put them down as properties inside this class */
        //}
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public ActivityParams Params { get; set; }
        }
        // first parameter A Query class the second parameter is what we return(return Type) from Handler class
        //public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly ILogger<List> _logger;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, ILogger<List> logger, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _logger = logger;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }
            /* we pass our query which froms a request that we pass to our handler and then it returns the data that we specify 
             we are looking for insid this IRequest interface and eventuall we return the list of activites */

            // public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //return await _context.Activities.ToListAsync();
                // return Result<List<Activity>>.Success(await _context.Activities.ToListAsync());

                //     var activitiesWithInclue = await _context.Activities.Include(a => a.Attendees).ThenInclude(u => u.AppUser)
                //   .ToListAsync(cancellationToken);

                //     var activitesToReturn = _mapper.Map<List<ActivityDto>>(activitiesWithInclue);
                //     return Result<List<ActivityDto>>.Success(activitesToReturn);


                // var activities = await _context.Activities
                //     .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                //     .ToListAsync(cancellationToken);


                //var activities = await _context.Activities
                //    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                //  .ToListAsync();

                var query = _context.Activities
                .Where(x => x.Date >= request.Params.StartDate)
                .OrderBy(d => d.Date)
                .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                .AsQueryable();

                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }

                //return Result<List<ActivityDto>>.Success(activities);
                return Result<PagedList<ActivityDto>>
                .Success(await PagedList<ActivityDto>.CreateAsync(query,
                request.Params.PageNumber, request.Params.PageSize));


            }

            // to apply cancellationToken
            // public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
            // {
            //     try
            //     {
            //         for (var i = 0; i < 10; i++)
            //         {
            //             cancellationToken.ThrowIfCancellationRequested();

            //             //if we continue 
            //             await Task.Delay(1000, cancellationToken);
            //             _logger.LogInformation($"Task{i} has completed");
            //         }

            //     }
            //     catch (Exception)
            //     {
            //         _logger.LogInformation($"Task was cancelled");
            //     }
            //     return await _context.Activities.ToListAsync();
            // }
        }
    }
}