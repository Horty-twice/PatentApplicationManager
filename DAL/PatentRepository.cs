using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using PatentApplicationManager.Entities;

namespace PatentApplicationManager.DAL
{
    /// <summary>
    /// Репозиторий для работы с патентными заявками в базе данных SQLite
    /// </summary>
    public class PatentRepository
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Имя файла базы данных
        /// </summary>
        private const string _databaseFileName = "patents.db";

        /// <summary>
        /// Путь к файлу лога ошибок
        /// </summary>
        private readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PatentRepository"/>
        /// </summary>
        /// <remarks>
        /// Проверяет существование файла БД и создает его при необходимости
        /// </remarks>
        public PatentRepository()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _databaseFileName);
            _connectionString = $"Data Source={databasePath};Version=3;";

            if (!File.Exists(databasePath))
            {
                CreateDatabase();
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PatentRepository"/> с указанием пути к БД
        /// </summary>
        /// <param name="dbPath">Путь к файлу базы данных. Если null, используется значение по умолчанию.</param>
        /// <remarks>
        /// Проверяет существование файла БД и создает его при необходимости
        /// </remarks>
        public PatentRepository(string dbPath)
        {
            string databasePath = dbPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _databaseFileName);
            _connectionString = $"Data Source={databasePath};Version=3;";

            if (!File.Exists(databasePath))
            {
                CreateDatabase();
            }
        }

        /// <summary>
        /// Фильтрует заявки по указанному статусу
        /// </summary>
        /// <param name="status">Статус для фильтрации</param>
        /// <returns>Отфильтрованный список заявок</returns>
        public List<PatentApplication> FilterByStatus(string status)
        {
            return GetAllPatentApplications()
                .Where(app => app.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Сортирует заявки по дате подачи
        /// </summary>
        /// <param name="ascending">True - по возрастанию, False - по убыванию</param>
        /// <returns>Отсортированный список заявок</returns>
        public List<PatentApplication> SortByFilingDate(bool ascending = true)
        {
            return ascending
                ? GetAllPatentApplications().OrderBy(app => app.FilingDate).ToList()
                : GetAllPatentApplications().OrderByDescending(app => app.FilingDate).ToList();
        }

        /// <summary>
        /// Создает новую базу данных SQLite с таблицей для хранения патентных заявок
        /// </summary>
        /// <exception cref="SQLiteException">Возникает при ошибках выполнения SQL-запроса</exception>
        /// <exception cref="IOException">Возникает при проблемах с доступом к файлу БД</exception>
        private void CreateDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                    CREATE TABLE PatentApplications (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ApplicationNumber TEXT,
                        FilingDate DATETIME,
                        InventionTitle TEXT,
                        ApplicantName TEXT,
                        PatentAttorneyName TEXT,
                        Status TEXT,
                        DecisionDate DATETIME,
                        PatentNumber TEXT,
                        ExpirationDate DATETIME,
                        Notes TEXT
                    )";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании базы данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавляет новую патентную заявку в базу данных
        /// </summary>
        /// <param name="application">Объект патентной заявки для добавления</param>
        /// <returns>ID добавленной заявки или 0 при ошибке</returns>
        /// <exception cref="SQLiteException">Возникает при ошибках выполнения SQL-запроса</exception>
        public int AddPatentApplication(PatentApplication application)
        {
            int newId = 0;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SQLiteCommand(@"
                    INSERT INTO PatentApplications 
                    (ApplicationNumber, FilingDate, InventionTitle, ApplicantName, 
                     PatentAttorneyName, Status, DecisionDate, PatentNumber, 
                     ExpirationDate, Notes)
                    VALUES 
                    (@ApplicationNumber, @FilingDate, @InventionTitle, @ApplicantName, 
                     @PatentAttorneyName, @Status, @DecisionDate, @PatentNumber, 
                     @ExpirationDate, @Notes);
                    SELECT last_insert_rowid();", connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationNumber", application.ApplicationNumber ?? "");
                        command.Parameters.AddWithValue("@FilingDate", application.FilingDate);
                        command.Parameters.AddWithValue("@InventionTitle", application.InventionTitle ?? "");
                        command.Parameters.AddWithValue("@ApplicantName", application.ApplicantName ?? "");
                        command.Parameters.AddWithValue("@PatentAttorneyName", application.PatentAttorneyName ?? "");
                        command.Parameters.AddWithValue("@Status", application.Status ?? "");
                        command.Parameters.AddWithValue("@DecisionDate", application.DecisionDate.HasValue ? (object)application.DecisionDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@PatentNumber", application.PatentNumber ?? "");
                        command.Parameters.AddWithValue("@ExpirationDate", application.ExpirationDate.HasValue ? (object)application.ExpirationDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Notes", application.Notes ?? "");

                        newId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении заявки: {ex.Message}");
            }
            return newId;
        }

        /// <summary>
        /// Получает список всех патентных заявок из базы данных
        /// </summary>
        /// <returns>Список объектов <see cref="PatentApplication"/> или пустой список при ошибке</returns>
        /// <exception cref="SQLiteException">Возникает при ошибках выполнения SQL-запроса</exception>
        public List<PatentApplication> GetAllPatentApplications()
        {
            List<PatentApplication> applications = new List<PatentApplication>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM PatentApplications";

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PatentApplication application = new PatentApplication
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ApplicationNumber = reader["ApplicationNumber"].ToString(),
                                FilingDate = Convert.ToDateTime(reader["FilingDate"]),
                                InventionTitle = reader["InventionTitle"].ToString(),
                                ApplicantName = reader["ApplicantName"].ToString(),
                                PatentAttorneyName = reader["PatentAttorneyName"].ToString(),
                                Status = reader["Status"].ToString(),
                                DecisionDate = reader["DecisionDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DecisionDate"]),
                                PatentNumber = reader["PatentNumber"].ToString(),
                                ExpirationDate = reader["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ExpirationDate"]),
                                Notes = reader["Notes"].ToString()
                            };
                            applications.Add(application);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении заявок: {ex.Message}");
            }
            return applications;
        }

        /// <summary>
        /// Получает патентную заявку по указанному ID
        /// </summary>
        /// <param name="id">ID искомой заявки</param>
        /// <returns>Объект <see cref="PatentApplication"/> или null, если заявка не найдена</returns>
        /// <exception cref="SQLiteException">Возникает при ошибках выполнения SQL-запроса</exception>
        public PatentApplication GetPatentApplicationById(int id)
        {
            PatentApplication application = null;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string selectQuery = @"
                    SELECT * FROM PatentApplications
                    WHERE Id = @Id";

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                application = new PatentApplication
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    ApplicationNumber = reader["ApplicationNumber"].ToString(),
                                    FilingDate = Convert.ToDateTime(reader["FilingDate"]),
                                    InventionTitle = reader["InventionTitle"].ToString(),
                                    ApplicantName = reader["ApplicantName"].ToString(),
                                    PatentAttorneyName = reader["PatentAttorneyName"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    DecisionDate = reader["DecisionDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DecisionDate"]),
                                    PatentNumber = reader["PatentNumber"].ToString(),
                                    ExpirationDate = reader["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ExpirationDate"]),
                                    Notes = reader["Notes"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении заявки по ID: {ex.Message}");
            }
            return application;
        }

        /// <summary>
        /// Обновляет данные патентной заявки в базе данных
        /// </summary>
        /// <param name="application">Объект <see cref="PatentApplication"/> с обновленными данными</param>
        /// <exception cref="SQLiteException">Возникает при ошибках выполнения SQL-запроса</exception>
        /// <exception cref="ArgumentException">Возникает если ID заявки не указан</exception>
        public void UpdatePatentApplication(PatentApplication application)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SQLiteCommand(@"
                    UPDATE PatentApplications
                    SET ApplicationNumber = @ApplicationNumber,
                        FilingDate = @FilingDate,
                        InventionTitle = @InventionTitle,
                        ApplicantName = @ApplicantName,
                        PatentAttorneyName = @PatentAttorneyName,
                        Status = @Status,
                        DecisionDate = @DecisionDate,
                        PatentNumber = @PatentNumber,
                        ExpirationDate = @ExpirationDate,
                        Notes = @Notes
                    WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationNumber", application.ApplicationNumber ?? "");
                        command.Parameters.AddWithValue("@FilingDate", application.FilingDate);
                        command.Parameters.AddWithValue("@InventionTitle", application.InventionTitle ?? "");
                        command.Parameters.AddWithValue("@ApplicantName", application.ApplicantName ?? "");
                        command.Parameters.AddWithValue("@PatentAttorneyName", application.PatentAttorneyName ?? "");
                        command.Parameters.AddWithValue("@Status", application.Status ?? "");
                        command.Parameters.AddWithValue("@DecisionDate", application.DecisionDate.HasValue ? (object)application.DecisionDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@PatentNumber", application.PatentNumber ?? "");
                        command.Parameters.AddWithValue("@ExpirationDate", application.ExpirationDate.HasValue ? (object)application.ExpirationDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Notes", application.Notes ?? "");
                        command.Parameters.AddWithValue("@Id", application.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении заявки: {ex.Message}");
            }
        }

        /// <summary>
        /// Удаляет патентную заявку по указанному ID
        /// </summary>
        /// <param name="id">ID заявки для удаления</param>
        /// <exception cref="SQLiteException">Возникает при ошибках выполнения SQL-запроса</exception>
        public void DeletePatentApplication(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string deleteQuery = @"
                    DELETE FROM PatentApplications
                    WHERE Id = @Id";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Ошибка при удалении заявки с ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Записывает сообщение об ошибке в лог-файл
        /// </summary>
        /// <param name="message">Сообщение для записи в лог</param>
        private void LogError(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при логировании: {ex.Message}");
            }
        }
    }
}