using System;
using System.Windows.Forms;
using PatentApplicationManager.Entities;
using PatentApplicationManager.DAL;
using System.Text.RegularExpressions;

namespace PatentApplicationManager.UI
{
    /// <summary>
    /// Форма для добавления новой патентной заявки
    /// </summary>
    /// <remarks>
    /// Предоставляет интерфейс для ввода всех необходимых данных о патентной заявке
    /// и выполняет валидацию введенных данных перед сохранением.
    /// </remarks>
    public partial class AddApplicationForm: Form
    {
        /// <summary>
        /// Репозиторий для работы с данными патентных заявок в базе данных
        /// </summary>
        /// <remarks>
        /// Предоставляет методы для выполнения CRUD-операций с патентными заявками.
        /// Использует SQLite в качестве хранилища данных.
        /// Инициализируется при создании формы и используется для сохранения изменений.
        /// </remarks>
        private PatentRepository _repository = new PatentRepository();

        /// <summary>
        /// Инициализирует новый экземпляр формы добавления заявки
        /// </summary>
        public AddApplicationForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Проверяет корректность введенных данных
        /// </summary>
        /// <returns>
        /// true - если все данные валидны, 
        /// false - если найдены ошибки (сообщение об ошибке отображается в lblError)
        /// </returns>
        /// <remarks>
        /// Выполняет комплексную проверку:
        /// 1. Обязательность полей
        /// 2. Формат данных (номера заявки и патента)
        /// 3. Длину текстовых полей
        /// 4. Логику дат (дата решения не может быть раньше подачи и т.д.)
        /// 5. Соответствие статуса и номера патента
        /// </remarks>
        private bool ValidateInput()
        {
            lblError.Text = ""; // Сбрасываем текст ошибки

            if (string.IsNullOrWhiteSpace(txtApplicationNumber.Text))
            {
                lblError.Text = "Пожалуйста, введите номер заявки.";
                return false;
            }

            // Проверка формата номера заявки (например, только цифры и символы "/")
            if (!Regex.IsMatch(txtApplicationNumber.Text, "^[0-9/]+$"))
            {
                lblError.Text = "Номер заявки должен содержать только цифры и символы '/'.";
                return false;
            }

            if (dtpFilingDate.Value > DateTime.Now)
            {
                lblError.Text = "Дата подачи не может быть в будущем.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtInventionTitle.Text))
            {
                lblError.Text = "Пожалуйста, введите название изобретения.";
                return false;
            }

            if (txtInventionTitle.Text.Length > 200)
            {
                lblError.Text = "Название изобретения не должно превышать 200 символов.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApplicantName.Text))
            {
                lblError.Text = "Пожалуйста, введите имя заявителя.";
                return false;
            }

            if (txtApplicantName.Text.Length > 100)
            {
                lblError.Text = "Имя заявителя не должно превышать 100 символов.";
                return false;
            }

            if (txtPatentAttorneyName.Text.Length > 100)
            {
                lblError.Text = "Имя патентного поверенного не должно превышать 100 символов.";
                return false;
            }

            if (txtPatentNumber.Text.Length > 50)
            {
                lblError.Text = "Номер патента не должен превышать 50 символов.";
                return false;
            }

            if (txtNotes.Text.Length > 500)
            {
                lblError.Text = "Примечания не должны превышать 500 символов.";
                return false;
            }

            // Проверка формата номера патента (например, только цифры)
            if (!string.IsNullOrWhiteSpace(txtPatentNumber.Text) && !Regex.IsMatch(txtPatentNumber.Text, "^[0-9]+$"))
            {
                lblError.Text = "Номер патента должен содержать только цифры.";
                return false;
            }

            // Проверка логики дат
            if (dtpDecisionDate.Checked && dtpDecisionDate.Value < dtpFilingDate.Value)
            {
                lblError.Text = "Дата решения не может быть раньше даты подачи.";
                return false;
            }

            if (dtpExpirationDate.Checked && dtpExpirationDate.Value < dtpFilingDate.Value)
            {
                lblError.Text = "Дата истечения срока действия не может быть раньше даты подачи.";
                return false;
            }

            // Проверка приблизительного срока действия патента (20 лет)
            if (dtpExpirationDate.Checked)
            {
                DateTime expectedExpirationDate = dtpFilingDate.Value.AddYears(20);
                TimeSpan difference = dtpExpirationDate.Value - expectedExpirationDate;

                // Допуск погрешности в несколько дней (например, 5 дней)
                if (Math.Abs(difference.TotalDays) > 5)
                {
                    lblError.Text = "Дата истечения срока действия должна быть приблизительно через 20 лет после даты подачи.";
                    return false;
                }
            }

            // Проверка обязательности и пустоты номера патента в зависимости от статуса
            if (cbStatus.Text == "Патент выдан")
            {
                if (string.IsNullOrWhiteSpace(txtPatentNumber.Text))
                {
                    lblError.Text = "При статусе 'Патент выдан' необходимо указать номер патента.";
                    return false;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtPatentNumber.Text))
                {
                    lblError.Text = "При статусе, отличном от 'Патент выдан', номер патента должен быть пустым.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return; // Останавливаем выполнение, если валидация не прошла
            }

            /// <summary>
            /// Создает объект PatentApplication на основе данных формы
            /// </summary>
            /// <returns>Заполненный объект патентной заявки</returns>
            PatentApplication application = new PatentApplication
            {
                ApplicationNumber = txtApplicationNumber.Text,
                FilingDate = dtpFilingDate.Value,
                InventionTitle = txtInventionTitle.Text,
                ApplicantName = txtApplicantName.Text,
                PatentAttorneyName = txtPatentAttorneyName.Text,
                Status = cbStatus.Text,
                DecisionDate = dtpDecisionDate.Checked ? dtpDecisionDate.Value : (DateTime?)null,
                PatentNumber = txtPatentNumber.Text,
                ExpirationDate = dtpExpirationDate.Checked ? dtpExpirationDate.Value : (DateTime?)null,
                Notes = txtNotes.Text
            };

            try
            {
                _repository.AddPatentApplication(application);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Произошла ошибка при сохранении данных: {ex.Message}"; // Используем lblError
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Отмена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
