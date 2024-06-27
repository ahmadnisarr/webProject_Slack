using Dapper;
using Microsoft.Data.SqlClient;

namespace WebProject.Models
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
    {
        private readonly string _connectionString;

        public GenericRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int GetId(string userId)
        {
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "userId";
                var query = $"SELECT Id FROM {tableName} WHERE {primaryKey} = @UserId;";

                return connection.QuerySingleOrDefault<int>(query,new { userId=userId });
                
                //using (var command = new SqlCommand(query, connection))
                //{
                //    command.Parameters.AddWithValue("@UserId", userId);
                //    var reader = command.ExecuteReader();
                //    if (reader.Read())
                //    {
                //        // Read the Id value from the reader
                //        id = reader.GetInt32(0); // Assuming Id is stored as the first column in the result set
                //    }
                //}
            }
        }

        public void Add(TEntity entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id");

                var columnNames = string.Join(",", properties.Select(p => p.Name));
                var parameterNames = string.Join(",", properties.Select(p => "@" + p.Name));

                var query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames});";
                connection.Execute(query, entity);

                //using (var command = new SqlCommand(query, connection))
                //{
                //    foreach (var property in properties)
                //    {
                //        command.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                //    }

                //    command.ExecuteNonQuery();
                //}
            }
        }

        public TEntity GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "Id";

                var query = $"SELECT * FROM {tableName} WHERE {primaryKey} = @Id;";

                return connection.QuerySingleOrDefault<TEntity>(query, new { Id = id });

                //using (var command = new SqlCommand(query, connection))
                //{
                //    command.Parameters.AddWithValue("@Id", id);
                //    var reader = command.ExecuteReader();
                //    if (reader.Read())
                //    {
                //        return MapReaderToObject(reader);
                //    }
                //    return default;
                //}
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;

                var query = $"SELECT * FROM {tableName};";

                return connection.Query<TEntity>(query).ToList();
                //using (var command = new SqlCommand(query, connection))
                //{
                //    var reader = command.ExecuteReader();
                //    var entities = new List<TEntity>();
                //    while (reader.Read())
                //    {
                //        entities.Add(MapReaderToObject(reader));
                //    }
                //    return entities;
                //}
            }
        }

        public void Update(TEntity entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "Id";

                var properties = typeof(TEntity).GetProperties().Where(p => p.Name != primaryKey);

                var setClause = string.Join(",", properties.Select(p => $"{p.Name} = @{p.Name}"));
                var query = $"UPDATE {tableName} SET {setClause} WHERE {primaryKey} = @{primaryKey};";

                connection.Execute(query,entity);

                //using (var command = new SqlCommand(query, connection))
                //{
                //    foreach (var property in properties)
                //    {
                //        command.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                //    }
                //    command.Parameters.AddWithValue("@" + primaryKey, typeof(TEntity).GetProperty(primaryKey).GetValue(entity));

                //    command.ExecuteNonQuery();
                //}
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                var primaryKey = "Id";

                var query = $"DELETE FROM {tableName} WHERE {primaryKey} = @Id;";

                connection.Execute(query, new { Id = id });

                //using (var command = new SqlCommand(query, connection))
                //{
                //    command.Parameters.AddWithValue("@Id", id);
                //    command.ExecuteNonQuery();
                //}
            }
        }

        private TEntity MapReaderToObject(SqlDataReader reader)
        {
            var entity = Activator.CreateInstance<TEntity>();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                    property.SetValue(entity, reader[property.Name]);
               
            }
            return entity;
        }


    }
}
