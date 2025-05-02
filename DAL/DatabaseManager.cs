using System;
using System.Data.SQLite;
using System.IO;

namespace PatentApplicationManager.DAL
{
    /// <summary>
    /// Предоставляет методы для управления файлом базы данных
    /// </summary>
    public class DatabaseManager
    {
        private readonly string _dbPath;

        /// <summary>
        /// Инициализирует новый экземпляр менеджера БД
        /// </summary>
        /// <param name="dbPath">Путь к файлу базы данных (по умолчанию patents.db)</param>
        public DatabaseManager(string dbPath = "patents.db")
        {
            _dbPath = dbPath;
        }

        /// <summary>
        /// Проверяет существование файла базы данных
        /// </summary>
        public bool DatabaseExists()
        {
            return File.Exists(_dbPath);
        }

        /// <summary>
        /// Создает новую базу данных с таблицей PatentApplications
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает при ошибках создания БД</exception>
        public void CreateDatabase()
        {
            try
            {
                if (File.Exists(_dbPath))
                    File.Delete(_dbPath);

                using (var connection = new SQLiteConnection($"Data Source={_dbPath}"))
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
                    new SQLiteCommand(createTableQuery, connection).ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка создания БД: {ex.Message}");
            }
        }

        /// <summary>
        /// Удаляет файл базы данных
        /// </summary>
        public void DeleteDatabase()
        {
            if (File.Exists(_dbPath))
                File.Delete(_dbPath);
        }

        /// <summary>
        /// Создает резервную копию базы данных
        /// </summary>
        /// <param name="backupPath">Путь для сохранения резервной копии</param>
        /// <exception cref="FileNotFoundException">Возникает если исходная БД не найдена</exception>
        public void BackupDatabase(string backupPath)
        {
            if (!File.Exists(_dbPath))
                throw new FileNotFoundException("Исходная БД не найдена");

            File.Copy(_dbPath, backupPath, overwrite: true);
        }

        /// <summary>
        /// Восстанавливает базу данных из резервной копии
        /// </summary>
        /// <param name="backupPath">Путь к файлу резервной копии</param>
        /// <exception cref="FileNotFoundException">Возникает если файл копии не найден</exception>
        public void RestoreDatabase(string backupPath)
        {
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Файл резервной копии не найден");

            if (File.Exists(_dbPath))
                File.Delete(_dbPath);

            File.Copy(backupPath, _dbPath);
        }
    }
}
