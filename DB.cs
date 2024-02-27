using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace KeyLoger
{
    
    public class SQLiteManager
    {
        private readonly string _connectionString;

        public SQLiteManager(string databaseFilePath)
        {
            _connectionString = $"Data Source={databaseFilePath};Version=3;";
        }

        public void AddItem(string tableName, Dictionary<string, object> data)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var commandText = $"INSERT INTO {tableName} ({string.Join(", ", data.Keys)}) VALUES ({string.Join(", ", new string[data.Count].Select((s, i) => $"@param{i}"))})";
                using (var command = new SqliteCommand(commandText, connection))
                {
                    int i = 0;
                    foreach (var item in data)
                    {
                        command.Parameters.AddWithValue($"@param{i}", item.Value);
                        i++;
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Dictionary<string, object>> ExecuteCustomQuery(string query)
        {
            var resultList = new List<Dictionary<string, object>>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            resultList.Add(row);
                        }
                    }
                }
            }

            return resultList;
        }
    }
}
