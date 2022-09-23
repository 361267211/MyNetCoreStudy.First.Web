using System;

namespace NetCoreStudy.First.EFCore
{
    public class Book
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime PubTime { get; set; }
        public double Price { get; set; }
        public double Birthday { get; set; }
    }
}
