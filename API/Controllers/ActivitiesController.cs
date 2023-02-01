using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {





        /*
       this time it's not coming from our API controller directly.

       Our API controller is sending a request via our go between mediator to our application project that's

       being handled inside there and then that returns the list of activities back up via mediator to our

       API controller, which we then return inside of an HTTP response to the client.

       So one of our goals is to make our controllers as thin as possible.

       And does it get any thinner than one line

       */

        // public ActivitiesController(IMediator mediator)
        // {

        // }

        /*  CancellationToken 
        we need to do a step more than what we've done here because our application handler is not where

        the request is currently sitting with.

        Our request has contacted our API controller via the HTTP request and then our API controller was passed

        off this request for the activities via mediator to our handler.

        So we need to pass the cancellation token from the API controller to our handler.
        */
        // [HttpGet]
        // public async Task<ActionResult<List<Activity>>> GetActivities(CancellationToken ct)
        // {
        //     return await Mediator.Send(new List.Query(), ct);
        // }


        //[HttpGet]
        // public async Task<IActionResult> GetActivities()
        // {
        //     return HandleResult(await Mediator.Send(new List.Query()));
        // }

        [HttpGet]
        public async Task<IActionResult> GetActivities([FromQuery] ActivityParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));

        }


        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivity(Guid id)
        {
            //var activity = await Mediator.Send(new Details.Query { Id = id });
            // if (activity == null) return NotFound();
            //return activity;


            // var result = await Mediator.Send(new Details.Query { Id = id });
            // if (result.IsSuccess && result.Value != null)
            //     return Ok(result.Value);
            // if (result.IsSuccess && result.Value == null)
            //     return NotFound();
            // return BadRequest(result.Error);


            // var result = await Mediator.Send(new Details.Query { Id = id });
            // return HandleResult(result);
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));

        }
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
        }
        [Authorize(Policy = "IsActivityHost")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }


    }
}