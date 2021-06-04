using System.Threading.Tasks;
using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<BasketCheckoutConsumer> _logger;

        public BasketCheckoutConsumer(IMapper mapper, IMediator mediator, ILogger<BasketCheckoutConsumer> logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var checkoutCommand = _mapper.Map<CheckoutOrderCommand>(context.Message);
            var result = await _mediator.Send(checkoutCommand);
            
            _logger.LogInformation($"BasketCheckoutEvent consumed successfully. Created Order Id - {result}");
        }
    }
}
