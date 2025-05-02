using PatentApplicationManager.DAL;
using PatentApplicationManager.Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PatentApplicationManager.UI
{
    /// <summary>
    /// Форма для редактирования существующей патентной заявки.
    /// Позволяет изменять все данные патентной заявки с выполнением валидации.
    /// Наследует базовую функциональность валидации от AddApplicationForm.
    /// </summary>
    public partial class EditApplicationForm: Form
    {
        /// <summary>
        /// Репозиторий для работы с данными патентных заявок
        /// Обеспечивает доступ к базе данных для операций обновления информации о заявках.
        /// Инициализируется при создании формы.
        /// </summary>
        private PatentRepository _repository = new PatentRepository();

        /// <summary>
        /// Редактируемая патентная заявка.
        /// Содержит исходные данные заявки перед редактированием.
        /// Все изменения формы применяются к этому объекту перед сохранением.
        /// </summary>
        private PatentApplication _application;

        /// <summary>
        /// Инициализирует новый экземпляр формы редактирования
        /// </summary>
        /// <param name="application">Объект патентной заявки для редактирования</param>
        /// <exception cref="ArgumentNullException">Если application равен null</exception>
        public EditApplicationForm(PatentApplication application)
        {
            InitializeComponent();

            _application = application; // Сохраняем заявку, которую нужно отредактировать

            // Заполняем форму данными из заявки
            txtApplicationNumber.Text = _application.ApplicationNumber;
            dtpFilingDate.Value = _application.FilingDate;
            txtInventionTitle.Text = _application.InventionTitle;
            txtApplicantName.Text = _application.ApplicantName;
            txtPatentAttorneyName.Text = _application.PatentAttorneyName;
            cbStatus.Text = _application.Status;

            // Обработка Nullable<DateTime> для DecisionDate
            if (_application.DecisionDate.HasValue)
            {
                dtpDecisionDate.Value = _application.DecisionDate.Value;
                dtpDecisionDate.Checked = true;
            }
            else
            {
                dtpDecisionDate.Checked = false;
            }

            // Обработка Nullable<DateTime> для ExpirationDate
            if (_application.ExpirationDate.HasValue)
            {
                dtpExpirationDate.Value = _application.ExpirationDate.Value;
                dtpExpirationDate.Checked = true;
            }
            else
            {
                dtpExpirationDate.Checked = false;
            }

            txtPatentNumber.Text = _application.PatentNumber;
            txtNotes.Text = _application.Notes;

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
        /// Проверяет корректность введенных данных
        /// </summary>
        /// <returns>
        /// true - если все данные валидны,
        /// false - если найдены ошибки (сообщение об ошибке отображается в lblError)
        /// </returns>
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

            // 2. Обновление объекта PatentApplication
            _application.ApplicationNumber = txtApplicationNumber.Text;
            _application.FilingDate = dtpFilingDate.Value;
            _application.InventionTitle = txtInventionTitle.Text;
            _application.ApplicantName = txtApplicantName.Text;
            _application.PatentAttorneyName = txtPatentAttorneyName.Text;
            _application.Status = cbStatus.Text;
            _application.DecisionDate = dtpDecisionDate.Checked ? dtpDecisionDate.Value : (DateTime?)null;
            _application.ExpirationDate = dtpExpirationDate.Checked ? dtpExpirationDate.Value : (DateTime?)null;
            _application.PatentNumber = txtPatentNumber.Text;
            _application.Notes = txtNotes.Text;

            try
            {
                // 3. Сохранение изменений в базу данных
                _repository.UpdatePatentApplication(_application);

                // 4. Закрытие формы
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Произошла ошибка при сохранении данных: {ex.Message}";
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
