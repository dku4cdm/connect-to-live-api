using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SpaceApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SpaceApp.Common.Repository
{
    public class MongoDBRepository<TModel> : IRepository<TModel> where TModel : MongoDbModel
    {
        protected MongoClient Mongo;
        protected string DatabaseName;
        protected string CollectionName;

        protected IMongoDatabase Database => Mongo.GetDatabase(DatabaseName);

        public IMongoCollection<TModel> Collection => Database.GetCollection<TModel>(CollectionName);


        public MongoDBRepository(string connectionString)
        {
            Mongo = new MongoClient(connectionString);
            DatabaseName = MongoUrl.Create(connectionString).DatabaseName;
            CollectionName = typeof(TModel).Name;
            if (!BsonClassMap.IsClassMapRegistered(typeof(TModel)))
            {
                BsonClassMap.RegisterClassMap<TModel>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        /// <summary>
        /// Creates new instance of model type
        /// </summary>
        /// <returns></returns>
        public virtual TModel New()
        {
            TModel instance = Activator.CreateInstance<TModel>();
            instance.Created = DateTime.UtcNow;

            return instance;
        }

        public virtual TModel GetById(Guid id)
        {
            var builder = Builders<TModel>.Filter;
            var filter = builder.Eq(x => x.Id, id);

            return Collection.FindSync(filter).SingleOrDefault();
        }

        public virtual TModel GetById(Guid id, QueryOptions<TModel> options)
        {
            var builder = Builders<TModel>.Filter;
            var filter = builder.Eq(x => x.Id, id);

            return this.FindAll(filter, options.Take(1)).FirstOrDefault();
        }

        public virtual IQueryable<TModel> GetAsQueryable()
        {
            return this.Collection.AsQueryable();
        }

        public virtual void Save(TModel model)
        {
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                model.Created = DateTime.UtcNow;
            }

            model.Updated = DateTime.UtcNow;
            var filter = Builders<TModel>.Filter.Eq(x => x.Id, model.Id);
            this.Collection.ReplaceOne(filter, model, new UpdateOptions { IsUpsert = true });
        }

        public virtual void InsertAll(IEnumerable<TModel> modelList)
        {
            this.Collection.InsertMany(modelList);
        }

        public void Remove(TModel spec)
        {
            if (spec == null)
                return;
            var builder = Builders<TModel>.Filter;
            var filter = builder.Eq(x => x.Id, spec.Id);

            Collection.DeleteOne(filter);
        }

        public void RemoveAll()
        {
            var filter = new BsonDocument();
            Collection.DeleteMany(filter);
        }

        public virtual IEnumerable<TModel> FindAll()
        {
            return Find(x => true);
        }

        public virtual IEnumerable<TModel> Find(IQueryable<TModel> query)
        {
            return query.AsEnumerable();
        }

        public virtual IEnumerable<TModel> Find(IQueryable<TModel> query, QueryOptions<TModel> options)
        {
            return query.Skip(options.Skip.Value).Take(options.Take.Value);
        }

        public virtual TModel FindOne(IQueryable<TModel> query)
        {
            return query.FirstOrDefault();
        }


        public virtual int Count(IQueryable<TModel> query)
        {
            return query.Count();
        }

        public virtual IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate)
        {
            return this.Collection.Find(predicate).ToEnumerable();
        }

        public virtual IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate, QueryOptions<TModel> options)
        {
            var filters = new FilterDefinitionBuilder<TModel>().Empty;
            if (predicate != null)
                filters = new FilterDefinitionBuilder<TModel>().Where(predicate);

            return FindAll(filters, options);
        }


        public virtual long Count(Expression<Func<TModel, bool>> predicate)
        {
            var cursor = predicate == null ? Collection.Find<TModel>(Builders<TModel>.Filter.Empty) : Collection.Find<TModel>(predicate);
            return cursor.Count();
        }


        public virtual long CountAll()
        {
            return Count((Expression<Func<TModel, bool>>)null);
        }

        private IEnumerable<TModel> FindAll(FilterDefinition<TModel> filters, QueryOptions<TModel> options)
        {
            ProjectionDefinition<TModel> projection = null;
            foreach (var field in options.Fields)
            {
                if (projection == null)
                    projection = new ProjectionDefinitionBuilder<TModel>().Include(field);
                else
                    projection = projection.Include(field);
            }

            SortDefinition<TModel> sortDefinition = null;
            var builder = new SortDefinitionBuilder<TModel>();
            if (options.SortAscending != null)
            {
                sortDefinition = builder.Ascending(options.SortAscending);
            }
            if (options.SortDescending != null)
                sortDefinition = builder.Descending(options.SortDescending);

            IFindFluent<TModel, TModel> result = null;
            if (projection == null)
            {
                result = this.Collection.Find(filters);
            }
            else
            {
                result = this.Collection.Find(filters).Project<TModel>(projection);
            }

            if (options.Skip.HasValue)
                result.Skip(options.Skip.Value);
            if (options.Take.HasValue)
                result.Limit(options.Take);

            if (sortDefinition != null)
                result.Sort(sortDefinition);

            return result.ToEnumerable();
        }

        public virtual IAggregateFluent<TModel> Aggregate()
        {
            return this.Collection.Aggregate<TModel>();
        }

        public virtual void FindAndModify(Expression<Func<TModel, bool>> query, UpdateDefinition<TModel> update)
        {
            FilterDefinition<TModel> filter = new FilterDefinitionBuilder<TModel>().Where(query);

            Collection.UpdateMany(filter, update);
        }
    }
}
