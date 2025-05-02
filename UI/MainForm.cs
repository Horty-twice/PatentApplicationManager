using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatentApplicationManager.Entities;
using PatentApplicationManager.DAL;
using PatentApplicationManager.Utils;
using PatentApplicationManager.UI;
using System.Linq;
using System.IO;
using System.Data.SQLite;

namespace PatentApplicationManager
{
    /// <summary>
    /// Главная форма приложения для управления патентными заявками.
    /// Предоставляет интерфейс для просмотра, добавления, редактирования и удаления заявок,
    /// а также генерации отчетов в PDF-формате.
    /// </summary>
    public partial class MainForm: Form
    {
        /// <summary>
        /// Репозиторий для работы с данными патентных заявок
        /// </summary>
        private PatentRepository _repository = new PatentRepository(); // Создаем экземпляр репозитория

        /// <summary>
        /// Поле для хранения состояния сортировки
        /// </summary>
        private Dictionary<string, bool> _sortDirections = new Dictionary<string, bool>();

        /// <summary>
        /// Подсказка для элемента txtSearch
        /// </summary>
        private string _placeholderText = "Найти заявку...";

        /// <summary>
        /// Флаг, указывающий активна ли в данный момент подсказка в поле поиска
        /// </summary>
        private bool _isPlaceholderActive = true;

        /// <summary>
        /// Устанавливает подсказку в поле поиска
        /// </summary>
        private void SetPlaceholder()
        {
            _isPlaceholderActive = true;
            txtSearch.Text = _placeholderText;
        }

        /// <summary>
        /// Удаляет подсказку из поля поиска
        /// </summary>
        private void RemovePlaceholder()
        {
            _isPlaceholderActive = false;
            txtSearch.Text = "";
        }

        /// <summary>
        /// Обработчик очистки подсказки для элемента txtSearch
        /// </summary>
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (_isPlaceholderActive)
            {
                RemovePlaceholder();
            }
        }

        /// <summary>
        /// Обработчик вывода подсказки для элемента txtSearch
        /// </summary>
        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                SetPlaceholder();
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр главной формы
        /// Устанавливает начальное состояние формы (развернутое на весь экран)
        /// и загружает список патентных заявок.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Устанавливаем подсказку и загружаем данные
            SetPlaceholder();
            LoadPatentApplications();

            // Отключаем Tab для всех элементов
            foreach (Control control in this.Controls)
            {
                control.TabStop = false;
            }

            // Отключаем обработку Tab на уровне формы
            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Tab)
                {
                    e.Handled = true; // Блокируем Tab
                }
            };
        }

        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadPatentApplications(); // Перемещаем вызов LoadPatentApplications в конец конструктора
        }

        /// <summary>
        /// Загружает список патентных заявок в DataGridView
        /// </summary>
        public void LoadPatentApplications()
        {
            try
            {
                List<PatentApplication> applications = _repository.GetAllPatentApplications();

                // Очистка и настройка DataGridView
                dgvPatentApplications.DataSource = null;
                dgvPatentApplications.Rows.Clear();
                dgvPatentApplications.Columns.Clear();

                // Добавление столбцов для кнопок действий
                DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn
                {
                    Name = "colEdit",
                    HeaderText = "Редактировать",
                    Text = "Редактировать",
                    UseColumnTextForButtonValue = true
                };
                dgvPatentApplications.Columns.Add(editColumn);

                DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn
                {
                    Name = "colDelete",
                    HeaderText = "Удалить",
                    Text = "Удалить",
                    UseColumnTextForButtonValue = true
                };
                dgvPatentApplications.Columns.Add(deleteColumn);

                dgvPatentApplications.AutoGenerateColumns = false;

                // Настройка столбцов для отображения данных
                AddDataColumn("Id", "Id", visible: false);
                AddDataColumn("ApplicationNumber", "Номер заявки", width: 80);
                AddDataColumn("FilingDate", "Дата подачи", width: 120);
                AddDataColumn("InventionTitle", "Название изобретения", width: 160); // Увеличено с ~150
                AddDataColumn("ApplicantName", "Заявитель", width: 120);
                AddDataColumn("PatentAttorneyName", "Патентный поверенный", width: 180); // Увеличено с ~100
                AddDataColumn("Status", "Статус", width: 100);
                AddDataColumn("DecisionDate", "Дата решения", width: 120);
                AddDataColumn("PatentNumber", "Номер патента", width: 120);
                AddDataColumn("ExpirationDate", "Дата истечения", width: 120);
                AddDataColumn("Notes", "Примечания", fill: true);

                dgvPatentApplications.DataSource = applications;
            }
            catch (Exception ex)
            {
                ShowError($"Произошла ошибка при загрузке данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик клика по заголовку колонки DataGridView
        /// </summary>
        private void DgvPatentApplications_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1) return; // Клик не по заголовку

            string columnName = dgvPatentApplications.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(columnName)) return; // Для кнопок нет DataPropertyName

            // Определяем направление сортировки
            bool sortAscending = !_sortDirections.ContainsKey(columnName) || !_sortDirections[columnName];
            _sortDirections[columnName] = sortAscending;

            // Сортируем данные
            SortData(columnName, sortAscending, e.ColumnIndex);
        }

        /// <summary>
        /// Сортирует данные в указанной колонке
        /// </summary>
        /// <param name="columnName">Имя колонки для сортировки</param>
        /// <param name="ascending">Направление сортировки (true - по возрастанию)</param>
        /// <param name="columnIndex">Индекс колонки для отображения значка сортировки</param>
        private void SortData(string columnName, bool ascending, int columnIndex)
        {
            if (dgvPatentApplications.DataSource is List<PatentApplication> applications)
            {
                List<PatentApplication> sortedList = null;

                switch (columnName)
                {
                    case "ApplicationNumber":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.ApplicationNumber).ToList() :
                            applications.OrderByDescending(a => a.ApplicationNumber).ToList();
                        break;

                    case "FilingDate":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.FilingDate).ToList() :
                            applications.OrderByDescending(a => a.FilingDate).ToList();
                        break;

                    case "InventionTitle":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.InventionTitle).ToList() :
                            applications.OrderByDescending(a => a.InventionTitle).ToList();
                        break;

                    case "ApplicantName":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.ApplicantName).ToList() :
                            applications.OrderByDescending(a => a.ApplicantName).ToList();
                        break;

                    case "PatentAttorneyName":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.PatentAttorneyName).ToList() :
                            applications.OrderByDescending(a => a.PatentAttorneyName).ToList();
                        break;

                    case "Status":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.Status).ToList() :
                            applications.OrderByDescending(a => a.Status).ToList();
                        break;

                    case "DecisionDate":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.DecisionDate ?? DateTime.MaxValue).ToList() :
                            applications.OrderByDescending(a => a.DecisionDate ?? DateTime.MinValue).ToList();
                        break;

                    case "PatentNumber":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.PatentNumber).ToList() :
                            applications.OrderByDescending(a => a.PatentNumber).ToList();
                        break;

                    case "ExpirationDate":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.ExpirationDate ?? DateTime.MaxValue).ToList() :
                            applications.OrderByDescending(a => a.ExpirationDate ?? DateTime.MinValue).ToList();
                        break;

                    case "Notes":
                        sortedList = ascending ?
                            applications.OrderBy(a => a.Notes).ToList() :
                            applications.OrderByDescending(a => a.Notes).ToList();
                        break;
                }

                if (sortedList != null)
                {
                    dgvPatentApplications.DataSource = sortedList;

                    // Обновляем визуальное отображение направления сортировки
                    foreach (DataGridViewColumn column in dgvPatentApplications.Columns)
                    {
                        column.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }

                    dgvPatentApplications.Columns[columnIndex].HeaderCell.SortGlyphDirection =
                        ascending ? SortOrder.Ascending : SortOrder.Descending;
                }
            }
        }

        /// <summary>
        /// Обработчик изменения текста в поле поиска
        /// </summary>
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Если текст - это подсказка, просто загружаем все заявки
            if (_isPlaceholderActive || string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadPatentApplications();
                return;
            }

            string searchTerm = txtSearch.Text.Trim();

            var filtered = _repository.GetAllPatentApplications()
                        .Where(app =>
                    (app.ApplicationNumber?.Contains(searchTerm) ?? false) ||
                    (app.InventionTitle?.Contains(searchTerm) ?? false) ||
                    (app.ApplicantName?.Contains(searchTerm) ?? false) ||
                    (app.PatentNumber?.Contains(searchTerm) ?? false) ||
                    (app.Notes?.Contains(searchTerm) ?? false))
                .ToList();

            dgvPatentApplications.DataSource = filtered;
        }


        /// <summary>
        /// Добавляет столбец в DataGridView с указанными параметрами
        /// </summary>
        /// <param name="name">Имя столбца и привязки данных</param>
        /// <param name="headerText">Текст заголовка столбца</param>
        /// <param name="visible">Видимость столбца</param>
        /// <param name="fill">Заполнять ли доступное пространство</param>
        /// <param name="width">Ширина столбца в пикселях (если не задано, используется авторазмер)</param>
        private void AddDataColumn(string name, string headerText, bool visible = true, bool fill = false, int? width = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                Visible = visible
            };

            if (fill)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else if (width.HasValue)
            {
                column.Width = width.Value;
            }

            dgvPatentApplications.Columns.Add(column);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить заявку"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        private void btnAddApplication_Click(object sender, EventArgs e)
        {
            using (var addApplicationForm = new AddApplicationForm())
            {
                if (addApplicationForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPatentApplications();
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия на ячейки DataGridView
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события, содержащие информацию о нажатой ячейке</param>
        private void dgvPatentApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Обработка кнопки "Редактировать"
            if (IsEditButtonClicked(e))
            {
                EditSelectedApplication(e.RowIndex);
            }
            // Обработка кнопки "Удалить"
            else if (IsDeleteButtonClicked(e))
            {
                DeleteSelectedApplication(e.RowIndex);
            }
        }

        /// <summary>
        /// Проверяет, была ли нажата кнопка "Редактировать"
        /// </summary>
        /// <param name="e">Данные события CellClick</param>
        /// <returns>True если нажата кнопка редактирования</returns>
        private bool IsEditButtonClicked(DataGridViewCellEventArgs e)
        {
            return dgvPatentApplications.Columns["colEdit"] != null &&
                   e.ColumnIndex == dgvPatentApplications.Columns["colEdit"].Index;
        }

        /// <summary>
        /// Проверяет, была ли нажата кнопка "Удалить"
        /// </summary>
        /// <param name="e">Данные события CellClick</param>
        /// <returns>True если нажата кнопка удаления</returns>
        private bool IsDeleteButtonClicked(DataGridViewCellEventArgs e)
        {
            return dgvPatentApplications.Columns["colDelete"] != null &&
                   e.ColumnIndex == dgvPatentApplications.Columns["colDelete"].Index;
        }

        /// <summary>
        /// Открывает форму редактирования для выбранной заявки
        /// </summary>
        /// <param name="rowIndex">Индекс строки с заявкой</param>
        private void EditSelectedApplication(int rowIndex)
        {
            int applicationId = GetApplicationId(rowIndex);
            var application = _repository.GetPatentApplicationById(applicationId);

            if (application != null)
            {
                using (var editApplicationForm = new EditApplicationForm(application))
                {
                    if (editApplicationForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadPatentApplications();
                    }
                }
            }
            else
            {
                ShowError($"Заявка с ID {applicationId} не найдена.");
            }
        }

        /// <summary>
        /// Удаляет выбранную заявку после подтверждения
        /// </summary>
        /// <param name="rowIndex">Индекс строки с заявкой</param>
        private void DeleteSelectedApplication(int rowIndex)
        {
            int applicationId = GetApplicationId(rowIndex);

            if (MessageBox.Show("Вы уверены, что хотите удалить эту заявку?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _repository.DeletePatentApplication(applicationId);
                    LoadPatentApplications();
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка при удалении заявки: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Получает ID заявки из указанной строки DataGridView
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <returns>ID заявки</returns>
        private int GetApplicationId(int rowIndex)
        {
            return Convert.ToInt32(dgvPatentApplications.Rows[rowIndex].Cells["Id"].Value);
        }

        /// <summary>
        /// Отображает сообщение об ошибке в интерфейсе
        /// </summary>
        /// <param name="message">Текст сообщения об ошибке</param>
        private void ShowError(string message)
        {
            lblError.Text = message;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сформировать отчет"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        [Obsolete]
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Сохранить отчет как PDF",
                    FileName = $"PatentReport_{DateTime.Now:yyyyMMdd}.pdf"
                })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var applications = _repository.GetAllPatentApplications();
                        PdfReportGenerator.GenerateReport(applications, saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сформирован!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при генерации отчета: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик кнопки создания новой БД
        /// </summary>
        private void btnCreateDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                var dbManager = new DatabaseManager();

                if (dbManager.DatabaseExists()) // Теперь работает!
                {
                    string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                    Directory.CreateDirectory(backupDir);
                    string backupPath = Path.Combine(backupDir, $"patents_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

                    dbManager.BackupDatabase(backupPath);
                    MessageBox.Show($"Резервная копия сохранена как:\n{backupPath}", "Бэкап создан");
                }

                dbManager.CreateDatabase();
                LoadPatentApplications();
                MessageBox.Show("Новая БД успешно создана", "Успех");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик кнопки удаления БД
        /// </summary>
        private void btnDeleteDatabase_Click(object sender, EventArgs e)
        {
            // Инициализация менеджера БД
            var dbManager = new DatabaseManager();

            try
            {
                // Проверка существования БД через DatabaseManager
                if (!dbManager.DatabaseExists())
                {
                    ShowError("База данных не обнаружена. Удаление невозможно.");
                    return;
                }

                // Этап 1: Предложение создать бэкап
                var backupDialogResult = MessageBox.Show(
                    "Хотите создать резервную копию перед удалением?",
                    "Резервное копирование",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button3);

                // Обработка выбора пользователя
                switch (backupDialogResult)
                {
                    case DialogResult.Yes:
                        try
                        {
                            string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                            Directory.CreateDirectory(backupDir);
                            string backupPath = Path.Combine(backupDir, $"patents_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

                            dbManager.BackupDatabase(backupPath);
                            MessageBox.Show($"Резервная копия сохранена в:\n{backupPath}",
                                          "Бэкап создан",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                        }
                        catch (Exception backupEx)
                        {
                            ShowError($"Ошибка создания резервной копии: {backupEx.Message}");
                            return;
                        }
                        break;

                    case DialogResult.Cancel:
                        return; // Пользователь отменил операцию
                }

                // Этап 2: Финальное подтверждение
                var confirmationResult = MessageBox.Show(
                    "ВНИМАНИЕ! Все данные будут безвозвратно удалены.\nПродолжить?",
                    "Последнее предупреждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (confirmationResult == DialogResult.Yes)
                {
                    dbManager.DeleteDatabase();
                    LoadPatentApplications(); // Обновление интерфейса

                    MessageBox.Show("База данных успешно удалена",
                                  "Операция завершена",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Критическая ошибка: {ex.Message}\n\nРекомендуется проверить:\n1. Закрыты ли все подключения к БД\n2. Доступ к файлу БД");
            }
        }

        /// <summary>
        /// Обработчик кнопки поиска заявок
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadPatentApplications();
                return;
            }

            var filtered = _repository.GetAllPatentApplications()
                .Where(app =>
                    (app.ApplicationNumber?.Contains(searchTerm) ?? false) ||
                    (app.InventionTitle?.Contains(searchTerm) ?? false) ||
                    (app.ApplicantName?.Contains(searchTerm) ?? false) ||
                    (app.PatentNumber?.Contains(searchTerm) ?? false) || // Добавили поиск по номеру патента
                    (app.Notes?.Contains(searchTerm) ?? false))          // И по примечаниям
                .ToList();

            dgvPatentApplications.DataSource = filtered;
        }

        /// <summary>
        /// Экспорт текущей БД в выбранный файл
        /// </summary>
        private void btnExportDatabase_Click_1(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "SQLite Database (*.db)|*.db|CSV File (*.csv)|*.csv|JSON File (*.json)|*.json";
                saveDialog.FileName = $"patents_export_{DateTime.Now:yyyyMMdd}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string ext = Path.GetExtension(saveDialog.FileName).ToLower();

                        switch (ext)
                        {
                            case ".db":
                                new DatabaseManager().BackupDatabase(saveDialog.FileName);
                                break;

                            case ".csv":
                                ExportToCsv(saveDialog.FileName);
                                break;

                            case ".json":
                                ExportToJson(saveDialog.FileName);
                                break;
                        }

                        MessageBox.Show($"Данные экспортированы в:\n{saveDialog.FileName}",
                                      "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Ошибка экспорта: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Экспорт в CSV
        /// </summary>
        private void ExportToCsv(string filePath)
        {
            var apps = _repository.GetAllPatentApplications();
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Номер;Дата подачи;Название;Статус;Заявитель");
                foreach (var app in apps)
                {
                    writer.WriteLine($"\"{app.ApplicationNumber}\";{app.FilingDate:yyyy-MM-dd};" +
                                   $"\"{app.InventionTitle}\";\"{app.Status}\";\"{app.ApplicantName}\"");
                }
            }
        }

        /// <summary>
        /// Экспорт в JSON
        /// </summary>
        private void ExportToJson(string filePath)
        {
            var apps = _repository.GetAllPatentApplications();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(apps, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Импорт БД"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        /// <remarks>
        /// Выполняет восстановление базы данных из резервной копии с полной проверкой:
        /// 1. Проверяет существование выбранного файла
        /// 2. Валидирует структуру SQLite базы
        /// 3. Проверяет наличие нужной таблицы
        /// 4. Создает резервную копию текущей БД
        /// 5. Выполняет восстановление с обработкой ошибок
        /// 6. Логирует все операции
        /// </remarks>
        private void BtnImportDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Выбор файла для импорта
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
                    openFileDialog.Title = "Выберите файл резервной копии БД";

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    string importFilePath = openFileDialog.FileName;

                    // 2. Проверка существования файла
                    if (!File.Exists(importFilePath))
                    {
                        ShowError("Выбранный файл не существует!");
                        return;
                    }

                    // 3. Проверка размера файла (минимум 1 КБ)
                    FileInfo fileInfo = new FileInfo(importFilePath);
                    if (fileInfo.Length < 1024)
                    {
                        ShowError("Файл слишком мал для базы данных!");
                        return;
                    }

                    // 4. Проверка, что файл является валидной SQLite БД
                    try
                    {
                        using (var testConnection = new SQLiteConnection($"Data Source={importFilePath};Version=3;FailIfMissing=True;"))
                        {
                            testConnection.Open();

                            // Проверка наличия нужной таблицы
                            var checkTableCmd = new SQLiteCommand(
                                "SELECT count(*) FROM sqlite_master " +
                                "WHERE type='table' AND name='PatentApplications'",
                                testConnection);

                            object result = checkTableCmd.ExecuteScalar();
                            if (result == null || Convert.ToInt32(result) == 0)
                            {
                                ShowError("В выбранной базе отсутствует таблица PatentApplications!");
                                return;
                            }

                            // Дополнительная проверка структуры таблицы
                            var checkColumnsCmd = new SQLiteCommand(
                                "PRAGMA table_info('PatentApplications')",
                                testConnection);

                            using (var reader = checkColumnsCmd.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    ShowError("Не удалось проверить структуру таблицы!");
                                    return;
                                }
                            }
                        }
                    }
                    catch (SQLiteException sqlEx)
                    {
                        ShowError($"Ошибка проверки базы данных: {sqlEx.Message}");
                        return;
                    }

                    // 5. Подтверждение операции
                    if (MessageBox.Show(
                        "ВНИМАНИЕ! Текущая база данных будет заменена.\nПродолжить?",
                        "Подтверждение импорта",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    }

                    var dbManager = new DatabaseManager();
                    string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                    Directory.CreateDirectory(backupDir);

                    // 6. Создание резервной копии текущей БД (если существует)
                    string backupPath = string.Empty;
                    if (dbManager.DatabaseExists())
                    {
                        backupPath = Path.Combine(backupDir, $"patents_pre_import_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                        try
                        {
                            dbManager.BackupDatabase(backupPath);
                            LogOperation($"Создана резервная копия перед импортом: {backupPath}");
                        }
                        catch (Exception backupEx)
                        {
                            ShowError($"Не удалось создать резервную копию: {backupEx.Message}");
                            return;
                        }
                    }

                    // 7. Восстановление из выбранного файла
                    try
                    {
                        dbManager.RestoreDatabase(importFilePath);
                        LogOperation($"Успешный импорт БД из: {importFilePath}");

                        // 8. Обновление интерфейса
                        LoadPatentApplications();

                        // 9. Уведомление пользователя
                        string message = "База данных успешно восстановлена из резервной копии.";
                        if (!string.IsNullOrEmpty(backupPath))
                        {
                            message += $"\nТекущая БД сохранена как: {backupPath}";
                        }

                        MessageBox.Show(message,
                            "Импорт завершен",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception importEx)
                    {
                        // Восстановление из резервной копии при ошибке
                        if (!string.IsNullOrEmpty(backupPath) && File.Exists(backupPath))
                        {
                            try
                            {
                                dbManager.RestoreDatabase(backupPath);
                                LogOperation($"Восстановление из резервной копии после ошибки импорта: {backupPath}");
                            }
                            catch { /* ignore */ }
                        }

                        ShowError($"Ошибка при импорте: {importEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperation($"Критическая ошибка при импорте: {ex}");
                ShowError($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Логирование операций с базой данных
        /// </summary>
        /// <param name="message">Сообщение для логирования</param>
        private void LogOperation(string message)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database_operations.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
            }
            catch { /* Гарантированно не выбросит исключение */ }
        }
    }
}
