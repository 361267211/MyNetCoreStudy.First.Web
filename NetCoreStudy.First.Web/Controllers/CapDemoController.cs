using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.EFCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers
{
    [Route("[controller]/[Action]")]

    [ApiController]
    public class CapDemoController : ControllerBase
    {
        private readonly TestDbContext _dbContext;
        private readonly ICapPublisher _publisher;

        public CapDemoController(ICapPublisher publisher, TestDbContext dbContext)
        {
            _publisher = publisher;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var art = new Article { Title="Ti夺冠", Content = "ti11中国RNG夺冠" };
            _dbContext.Articles.Add(art);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync<Article>("Order.Created", art);  //发布Order.Created事件
            return "订单创建成功啦";
        }

        [NonAction]
        [CapSubscribe("Order.Created")]    //监听Order.Created事件
        public async Task OrderCreatedEventHand(Article article)
        {

            var art = new Article {/* Title = "Ti爆出假赛内幕",*/ Content = "ti11中国RNG夺冠成迷" };


            _dbContext.Add(art);
            await _dbContext.SaveChangesAsync();

            Console.WriteLine(art);
        }
    }
}
