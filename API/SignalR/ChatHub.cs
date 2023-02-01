using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;
namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            var comment = await _mediator.Send(command);

            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);
            //.SendAsync("ReceiveComment", comment.Value); it will call one method call "ReceiveComment" from Client Side
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext.Request.Query["activityId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });


            /* Clients.Caller means who do we want to send this list of comment to . the only user we need to send this to is the caller, the person makeing this
               request to connect to this Hub
            */
            await Clients.Caller.SendAsync("LoadComments", result.Value);
             //.SendAsync("LoadComments", result.Value); it will call one method call "LoadComments" from Client Side
        }
    }
}