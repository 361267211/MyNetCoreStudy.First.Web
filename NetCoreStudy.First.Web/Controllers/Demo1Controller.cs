using ExpressionTreeToString;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.EFCore.Entity;
using NetCoreStudy.First.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;

namespace NetCoreStudy.First.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class Demo1Controller : ControllerBase
    {
        private readonly TestDbContext _dbContext;
        public Demo1Controller(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<object> GetTest()
        {
            var prices = _dbContext.Books.Where(e => true).OrderBy(e => e.Price).Select(e => e.Price);
            var books = _dbContext.Books.FromSqlInterpolated($@"select * from T_Books a where a.Price>77");
            var query = books.Where(b => b.Id > 1);


            var querysStr = query.ToQueryString();


            Trace.WriteLine(querysStr);

            var list = query.ToList();
            return prices;
        }

        [HttpGet]
        public async Task AddArtWithComment()
        {
            var prices = _dbContext.Articles.Add(
                new Article
                {
                    Content = "zzq赚钱了",
                    Title = "财经zzq",
                    Comments = new List<Comment>
                    {
                        new Comment{ Message="羡慕"},
                        new Comment{ Message="嫉妒"},
                    }
                }
                );

            await _dbContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<List<Article>> GetArtWithComment()
        {

            List<Article> artList = await _dbContext.Articles.Include(a => a.Comments).ToListAsync();
            return artList;
        }

        [HttpGet]
        public async Task AddOrgUnit()
        {

            var orgUnit = _dbContext.OrgUnits.Add(
                          new OrgUnit
                          {
                              Name = "微软总部",
                              Children = new List<OrgUnit>
                              {
                                  new OrgUnit{Name="微软亚洲" ,Children=new List<OrgUnit>{
                                                              new OrgUnit{Name="微软中国"},
                                                              new OrgUnit{Name="微软韩国"},
                                                              new OrgUnit{Name="微软日本"},
                                  } },
                                  new OrgUnit{Name="微软非洲",Children=new List<OrgUnit>{
                                                              new OrgUnit{Name="微软刚果"},
                                                              new OrgUnit{Name="微软埃及"},
                                  } },
                                  new OrgUnit{Name="微软美洲",Children=new List<OrgUnit>{
                                                              new OrgUnit{Name="微软加拿大"},
                                                              new OrgUnit{Name="微软巴西"},
                                  }},
                                  new OrgUnit{Name="微软澳洲",Children=new List<OrgUnit>{
                                                              new OrgUnit{Name="微软澳大利亚"},
                                                              new OrgUnit{Name="微软新西兰"},
                                  }},
                              }
                          }
                          );

            await _dbContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task AddTest()
        {
            _dbContext.Books.Add(new Book
            {
                Price = 500,
                Title = "三国",
                PubTime = new DateTime(1800, 5, 22)
            });
            _dbContext.Books.Add(new Book
            {
                Price = 89,
                Title = "水浒",
                PubTime = new DateTime(1900, 8, 13)
            });
            _dbContext.Books.Add(new Book
            {
                Price = 69,
                Title = "红楼梦",
                PubTime = new DateTime(1850, 9, 9)
            });
            _dbContext.Books.Add(new Book
            {
                Price = 45,
                Title = "华尔街之狼",
                PubTime = new DateTime(2000, 10, 11)
            });
            _dbContext.Books.Add(new Book
            {
                Price = 88,
                Title = "牛津英语",
                PubTime = new DateTime(2016, 1, 9)
            });

            await _dbContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task AddStudentWithTeacher()
        {
            List<Student> stuList = new List<Student>
            {
                new Student{Name="李易峰"},
                new Student{Name="吴亦凡"},
                new Student{Name="吴秀波"},
                new Student{Name="房祖名"},
            };

            List<Teacher> teacherList = new List<Teacher>
            {
                new Teacher{Name="Tom"},
                new Teacher{Name="Jerry"},
                new Teacher{Name="Zack"},
            };

            foreach (var teacher in teacherList)
            {
                foreach (var student in stuList)
                {
                    teacher.Students.Add(student);
                }

            }

            _dbContext.Students.AddRange(stuList);
            _dbContext.Teachers.AddRange(teacherList);

            await _dbContext.SaveChangesAsync();
        }


        [HttpGet]
        public async Task UpdateArticle()
        {
            var artList = _dbContext.Articles.Take(3).ToList();
            artList[1].Title = "[紧急]" + artList[1].Title;
            Article detachedArt = new Article { Title = "未追踪的对象" };
            Article istTrackedArt = new Article { Title = "新插入的对象", Content = "新插入" };
            _dbContext.Remove(artList[2]);
            _dbContext.Add(istTrackedArt);


            //unChanged
            Trace.WriteLine(_dbContext.Entry(artList[0]).State);

            //修改
            Trace.WriteLine(_dbContext.Entry(artList[1]).State);

            //删除
            Trace.WriteLine(_dbContext.Entry(artList[2]).State);

            //插入
            Trace.WriteLine(_dbContext.Entry(istTrackedArt).State);

            //未追踪
            Trace.WriteLine(_dbContext.Entry(detachedArt).State);

            await _dbContext.SaveChangesAsync();

            //unChanged-->unChanged
            Trace.WriteLine(_dbContext.Entry(artList[0]).State);

            //修改-->unChanged
            Trace.WriteLine(_dbContext.Entry(artList[1]).State);

            //删除-->Detached
            Trace.WriteLine(_dbContext.Entry(artList[2]).State);

            //插入-->unChanged
            Trace.WriteLine(_dbContext.Entry(istTrackedArt).State);

            //未追踪-->Detached
            Trace.WriteLine(_dbContext.Entry(detachedArt).State);


            _dbContext.Entry(istTrackedArt).State = EntityState.Detached;
            //不查询的情况下该值，以istTrackedArt 为模板
            Article article = new Article { Id = istTrackedArt.Id, Title = "没查询就update" };
            _dbContext.Entry(article).Property(a => a.Title).IsModified = true;
            await _dbContext.SaveChangesAsync();

        }

        /// <summary>
        /// 并发情况下的乐观锁控制机制
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task UpdatePrice(long id)
        {

            var book = _dbContext.Books.First(e => e.Id == id);
            Thread.Sleep(5000);
            book.Price = book.Price + 500;
            try
            {
                await _dbContext.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.First();
                var dbValues = await entry.GetDatabaseValuesAsync();
                string dbPrice = dbValues.GetValue<string>(nameof(Book.Price));
                Console.WriteLine($"有其他人和您同时并发修改了本书的价格,修改后的价格{dbPrice}");
                // throw new DbUpdateConcurrencyException($"有其他人和您同时并发修改了本书的价格,修改后的价格{dbPrice}");
            }



        }

        [HttpGet]
        public async Task DebugExpression(long id)
        {
            Expression<Func<Book, bool>> exp1 = b1 => b1.Price > 5;
            var exp = _dbContext.Books.CreateExp<Book>(nameof(Book.Price), 77);

            var obj = new { Title = "", Price = 11 };
            var obj2 = RetObject();
            var query = _dbContext.Books.CreateExp<Book>(nameof(Book.Title), "三国").MySelect("Title", "Price");

            var myQuery = _dbContext.Books.Select(b => obj);
            var myQuery1 = _dbContext.Books.Select(b =>
           new { b.Title, b.Price }

            );

            var sqlstr = myQuery.ToQueryString();

            var list = myQuery.ToList();

            /*Expression<Func<Book, bool>> exp1 = b1 => b1.Price > 5;

            Console.WriteLine(exp1.ToString("Object notation", "C#"));
            Console.WriteLine(exp1.ToString("Factory methods", "C#"));


            var param = Expression.Parameter(type: typeof(Book), name: "b1");

            //常量表达式:5
            ConstantExpression contantRight = Expression.Constant(value: 5.0, type: typeof(double));
            //成员表达式:b1.price
            MemberExpression memberLeft = Expression.MakeMemberAccess(expression: param, member: typeof(Book).GetProperty("Price"));
            //等式表达式：>
            BinaryExpression binary = Expression.MakeBinary(binaryType: ExpressionType.GreaterThan, left: memberLeft, right: contantRight);

            Expression<Func<Book, bool>> lamda = Expression.Lambda<Func<Book, bool>>(binary, param);


             

            var b1 = Parameter(
                typeof(Book),
                "b1"
            );

            //
            Lambda(
                GreaterThan(
                    MakeMemberAccess(b1,
                        typeof(Book).GetProperty("Price")
                    ),
                    Constant(5.0)
                ),
                b1
            );
*/
            // Expression<Func<Book, Book, double>> exp2 = (b1, b2) => b1.Price + b2.Price;
        }

        private Object RetObject()
        {


            Type t = BuildDynamicTypeWithProperties();

            var obj = Activator.CreateInstance(t);

            return obj;
        }

        public static Type BuildDynamicTypeWithProperties()
        {

            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            // To generate a persistable assembly, specify AssemblyBuilderAccess.RunAndSave.
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName,
                                                            AssemblyBuilderAccess.Run);
            // Generate a persistable single-module assembly.
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name);

            TypeBuilder myTypeBuilder = myModBuilder.DefineType("CustomerData",
                                                            TypeAttributes.Public);

            //字段名
            FieldBuilder customerNameBldr = myTypeBuilder.DefineField("customerName",
                                                            typeof(string),
                                                            FieldAttributes.Public);

            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameterless property,
            // use an array with no elements: new Type[] {})
            //属性名 prop
            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty("CustomerName",
                                                             PropertyAttributes.None,
                                                             typeof(string),
                                                             null);

            // The property set and property get methods require a special
            // set of attributes.

            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder custNameGetPropMthdBldr =
                myTypeBuilder.DefineMethod("get_CustomerName",
                                           getSetAttr,
                                           typeof(string),
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder custNameSetPropMthdBldr =
                myTypeBuilder.DefineMethod("set_CustomerName",
                                           getSetAttr,
                                           null,
                                           new Type[] { typeof(string) });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to
            // their corresponding behaviors, "get" and "set" respectively.
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);

            //自写字段赋值逻辑
            ConstructorBuilder ctorBuilder =  myTypeBuilder.DefineConstructor(attributes: MethodAttributes.Public,
                                            callingConvention: CallingConventions.Standard,
                                            parameterTypes: new Type[0]);

            var ctorIL = ctorBuilder.GetILGenerator();
            //调用构造函数
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, typeof(System.Object).GetConstructor(new Type[0]));

            //Ldfld类，等待Stfld处被使用，
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldstr, "zzq");
            ctorIL.Emit(OpCodes.Stfld, customerNameBldr);

            //定义一个字符串
            ctorIL.Emit(OpCodes.Ldstr, "生成的第一个程序");
            //调用一个函数
            ctorIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));

            ctorIL.Emit(OpCodes.Ret);


            Type retval = myTypeBuilder.CreateType();

            // Save the assembly so it can be examined with Ildasm.exe,
            // or referenced by a test program.
            //   myAsmBuilder.Save(myAsmName.Name + ".dll");




            return retval;
        }

    }
}
