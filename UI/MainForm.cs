using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PatentApplicationManager.Entities;
using PatentApplicationManager.DAL;
using PatentApplicationManager.Utils;
using PatentApplicationManager.UI;

namespace PatentApplicationManager
{
    /// <summary>
    /// Главная форма приложения для управления патентными заявками
    /// </summary>
    /// <remarks>
    /// Предоставляет интерфейс для просмотра, добавления, редактирования и удаления заявок,
    /// а также генерации отчетов в PDF-формате.
    /// </remarks>
    public partial class MainForm: Form
    {
        /// <summary>
        /// Репозиторий для работы с данными патентных заявок
        /// </summary>
        private PatentRepository _repository = new PatentRepository(); // Создаем экземпляр репозитория

        /// <summary>
        /// Инициализирует новый экземпляр главной формы
        /// </summary>
        /// <remarks>
        /// Устанавливает начальное состояние формы (развернутое на весь экран)
        /// и загружает список патентных заявок.
        /// </remarks>
        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
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
        /// <remarks>
        /// Очищает существующие столбцы, создает кнопки действий (редактировать/удалить)
        /// и настраивает привязку данных к столбцам таблицы.
        /// </remarks>
        public void LoadPatentApplications()
        {
            try
            {
                List<PatentApplication> applications = _repository.GetAllPatentApplications();

                // Очистка и настройка DataGridView
                dgvPatentApplications.DataSource = null;
                dgvPatentApplications.Rows.Clear();
                dgvPatentApplications.Columns.Clear();

                // Добавление столбца для кнопки "Редактировать"
                if (dgvPatentApplications.Columns["colEdit"] == null)
                {
                    DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn
                    {
                        Name = "colEdit",
                        HeaderText = "Редактировать",
                        Text = "Редактировать",
                        UseColumnTextForButtonValue = true
                    };
                    dgvPatentApplications.Columns.Add(editColumn);
                }

                // Добавление столбца для кнопки "Удалить"
                if (dgvPatentApplications.Columns["colDelete"] == null)
                {
                    DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn
                    {
                        Name = "colDelete",
                        HeaderText = "Удалить",
                        Text = "Удалить",
                        UseColumnTextForButtonValue = true
                    };
                    dgvPatentApplications.Columns.Add(deleteColumn);
                }

                dgvPatentApplications.AutoGenerateColumns = false;

                // Настройка столбцов для отображения данных
                AddDataColumn("Id", "Id", visible: false);
                AddDataColumn("ApplicationNumber", "Номер заявки");
                AddDataColumn("FilingDate", "Дата подачи");
                AddDataColumn("InventionTitle", "Название изобретения");
                AddDataColumn("ApplicantName", "Заявитель");
                AddDataColumn("PatentAttorneyName", "Патентный поверенный");
                AddDataColumn("Status", "Статус");
                AddDataColumn("DecisionDate", "Дата решения");
                AddDataColumn("PatentNumber", "Номер патента");
                AddDataColumn("ExpirationDate", "Дата истечения");
                AddDataColumn("Notes", "Примечания", fill: true);

                dgvPatentApplications.DataSource = applications;
            }
            catch (Exception ex)
            {
                ShowError($"Произошла ошибка при загрузке данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавляет столбец в DataGridView с указанными параметрами
        /// </summary>
        /// <param name="name">Имя столбца и привязки данных</param>
        /// <param name="headerText">Текст заголовка столбца</param>
        /// <param name="visible">Видимость столбца</param>
        /// <param name="fill">Заполнять ли доступное пространство</param>
        private void AddDataColumn(string name, string headerText, bool visible = true, bool fill = false)
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
    }
}
