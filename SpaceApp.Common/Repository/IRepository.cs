using MongoDB.Driver;
using SpaceApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SpaceApp.Common.Repository
{
    public interface IRepository<TModel> where TModel : MongoDbModel
    {
        TModel New();
        TModel GetById(Guid id);
        TModel GetById(Guid id, QueryOptions<TModel> options);
        IQueryable<TModel> GetAsQueryable();
        void Save(TModel model);
        void InsertAll(IEnumerable<TModel> modelList);
        void Remove(TModel spec);
        void RemoveAll();
        IEnumerable<TModel> Find(IQueryable<TModel> query);
        IEnumerable<TModel> Find(IQueryable<TModel> query, QueryOptions<TModel> options);
        TModel FindOne(IQueryable<TModel> query);
        IEnumerable<TModel> FindAll();
        IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate);
        IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate, QueryOptions<TModel> options);
        void FindAndModify(Expression<Func<TModel, bool>> predicate, UpdateDefinition<TModel> update);
        long Count(Expression<Func<TModel, bool>> predicate);
        long CountAll();
        int Count(IQueryable<TModel> query);
        IAggregateFluent<TModel> Aggregate();
    }

    public class QueryOptions<T>
    {
        public QueryOptions()
        {
            Fields = new List<Expression<Func<T, object>>>();
        }

        private static readonly QueryOptions<T> __empty = new EmptyQueryOptions<T>();

        public static QueryOptions<T> Empty
        {
            get { return __empty; }
        }

        public int? Skip { get; set; }

        public int? Take { get; set; }

        public IEnumerable<Expression<Func<T, object>>> Fields { get; set; }

        public string SortAscending { get; set; }

        public string SortDescending { get; set; }
    }

    internal sealed class EmptyQueryOptions<T> : QueryOptions<T>
    {
    }

    public static class QueryOptionsExtension
    {
        public static QueryOptions<T> Include<T>(this QueryOptions<T> options, params Expression<Func<T, object>>[] fields)
        {
            var result = new List<Expression<Func<T, object>>>();

            foreach (var field in fields)
            {
                result.Add(field);
            }
            options.Fields = result;
            return options;
        }

        public static QueryOptions<T> Skip<T>(this QueryOptions<T> options, int skip)
        {
            options.Skip = skip;
            return options;
        }

        public static QueryOptions<T> Take<T>(this QueryOptions<T> options, int take)
        {
            options.Take = take;
            return options;
        }
    }
}
