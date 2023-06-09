using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
namespace NetCoreStudy.First.Utility
{
    public static class DynamicExpressionExtension
    {
        /// <summary>
        /// get IQueryable by fiedName and field value
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="quryable"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <returns>new queryable result</returns>
        public static IQueryable<TModel> CreateExp<TModel>(this IQueryable<TModel> quryable,  string propName, object value) where TModel : class
        {
            //取model的类型
            Type modelType = typeof(TModel);
            //取 成员表达式
            PropertyInfo member = modelType.GetProperty(propName);
            //取成员的类型
            Type propType = member.PropertyType;
            //生成 常量exp表达式
            ConstantExpression propValue = Constant(System.Convert.ChangeType(value, propType), propType);
            //生成 成员exp表达式
            ParameterExpression b1 = Parameter(
                                         modelType,
                                         "b1"
                                     );
            Expression<Func<TModel, bool>> lamda;

            if (propType.IsPrimitive)
            {
                //生成原始类型 等于 lamda表达式
                lamda = Lambda<Func<TModel, bool>>(
                           Equal(
                               MakeMemberAccess(b1,
                                   member
                               ),
                               propValue
                           ),
                           b1
                       );
            }
            else
            {

                //生成非原始 等于 lamda表达式
                lamda = Lambda<Func<TModel, bool>>(
                            MakeBinary(ExpressionType.Equal,
                                MakeMemberAccess(b1,
                                    member
                                ),
                                propValue,false,
                                typeof(string).GetMethod("op_Equality")
                            ),
                            b1
                        );

            }


            quryable= quryable.Where(lamda);


            return quryable;
        }

        public static IQueryable<object[]> MySelect<TModel>(this IQueryable<TModel> quryable,params string[] propNameList)
        {
            ParameterExpression p = Parameter(type: typeof(TModel));

            List<Expression> propExprList = new List<Expression>();
            foreach (var propName in propNameList)
            {
                Expression propExpr = Convert(
                                             MakeMemberAccess(expression: p, member: typeof(TModel).GetProperty(propName)),
                                                              type: typeof(object
                                                             )
                                       );

                propExprList.Add(propExpr);
            }

            Expression[] initializers = propExprList.ToArray();
            NewArrayExpression newArrayExp = Expression.NewArrayInit(type: typeof(object), initializers: initializers );
            Expression<Func<TModel, object[]>> selectExpression = Expression.Lambda<Func<TModel,object[]>>(newArrayExp, p);
            IQueryable<object[]> query = quryable.Select(selectExpression);
            return query;
        }
    }
}
