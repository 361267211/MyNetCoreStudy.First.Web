using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.EFCore.Entity;
using NetCoreStudy.First.Web.MediatR;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers
{
    [Route("[controller]/[Action]")]

    [ApiController]
    public class TestController : ControllerBase
    {
        private IMediator _mediator;
        private TestDbContext _dbContext;

        public TestController(IMediator mediator, TestDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task AddOrder()
        {
            var order = new Order { TotalAmount = 4, Details = new List<OrderDetail>() };
            Merchan merchan = new Merchan { Name = "苹果手机8", Price = 4399 };
            order.AddDetail(merchan, 8);
            _dbContext.Add(order);
            await _dbContext.SaveChangesAsync();
        }


        [HttpGet]
        public async Task RabbitMQTest()
        {
            var connFactory = new ConnectionFactory();
            connFactory.HostName = "42.193.20.184";
            connFactory.DispatchConsumersAsync = true;
            var connection = connFactory.CreateConnection();
            string exchangeName = "exchange1";
            string queueName = "queue1";
            string routingKey = "key1";
         

            for (int i = 0; i < 5; i++)
            {
                string msg = DateTime.Now.TimeOfDay.ToString();
                using (var channel=connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
                    byte[] body = Encoding.UTF8.GetBytes(msg);
                    channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, mandatory: true, basicProperties: properties, body: body);




                }
                Console.WriteLine("发布了消息"+msg);
                Thread.Sleep(2000);
            }
        }
    }
}
