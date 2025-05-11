using System;
using System.Windows.Forms;
using PatentApplicationManager.Entities;
using PatentApplicationManager.DAL;
using System.Text.RegularExpressions;

namespace PatentApplicationManager.UI
{
    /// <summary>
    /// Форма для добавления новой патентной заявки.
    /// Предоставляет интерфейс для ввода всех необходимых данных о патентной заявке
    /// и выполняет валидацию введенных данных перед сохранением.
    /// </summary>
    public partial class AddApplicationForm: Form
    {
        /// <summary>
        /// Репозиторий для работы с данными патентных заявок в базе данных
        /// Предоставляет методы для выполнения CRUD-операций с патентными заявками.
        /// Использует SQLite в качестве хранилища данных.
        /// Инициализируется при создании формы и используется для сохранения изменений.
        /// </summary>
        private PatentRepository _repository = new PatentRepository();

        /// <summary>
        /// Первоначальная настройка
        /// </summary>
        private void UpdateFormFieldsLockState()
        {
            bool isGranted = cbStatus.Text == PatentApplication.STATUS_GRANTED;
            bool isApproved = cbStatus.Text == PatentApplication.STATUS_APPROVED;
            bool isSubmitted = cbStatus.Text == PatentApplication.STATUS_SUBMITTED;
            bool isRejected = cbStatus.Text == PatentApplication.STATUS_REJECTED;

            // Блокировка номеров
            txtApplicationNumber.ReadOnly = true; // Всегда только для чтения

            // Номер патента полностью блокируется при статусе "Патент выдан"
            txtPatentNumber.ReadOnly = isGranted;
            if (isGranted)
            {
                if (string.IsNullOrEmpty(txtPatentNumber.Text) || !int.TryParse(txtPatentNumber.Text, out _))
                {
                    txtPatentNumber.Text = _repository.GetNextPatentNumber();
                }
            }
            else
            {
                txtPatentNumber.Text = "";
            }

            // Даты блокируются в зависимости от статуса
            dtpDecisionDate.Enabled = isApproved || isGranted || isRejected;
            dtpExpirationDate.Enabled = isGranted;

            // Дополнительные ограничения
            if (isSubmitted)
            {
                dtpDecisionDate.Checked = false;
                dtpExpirationDate.Checked = false;
                txtPatentNumber.Text = "";
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр формы добавления заявки
        /// </summary>
        public AddApplicationForm()
        {
            InitializeComponent();

            // Автоматически генерируем номер заявки
            txtApplicationNumber.Text = _repository.GetNextApplicationNumber();
            txtApplicationNumber.ReadOnly = true; // Запрещаем редактирование

            if (cbStatus.Text == PatentApplication.STATUS_GRANTED)
            {
                txtPatentNumber.ReadOnly = true;
            }

            // Подписываемся на изменение статуса
            cbStatus.SelectedIndexChanged += CbStatus_SelectedIndexChanged;

            // Устанавливаем дату истечения патента (текущая дата + 20 лет)
            dtpExpirationDate.Value = DateTime.Now.AddYears(20);

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
            cbStatus.SelectedIndexChanged += (s, e) => UpdateFormFieldsLockState();
            UpdateFormFieldsLockState();
        }

        /// <summary>
        /// Обработчик изменения статуса
        /// </summary>
        private void CbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus.Text == PatentApplication.STATUS_GRANTED)
            {
                txtPatentNumber.ReadOnly = true; // Полная блокировка поля
                if (string.IsNullOrEmpty(txtPatentNumber.Text))
                {
                    txtPatentNumber.Text = _repository.GetNextPatentNumber();
                }
            }
            else
            {
                txtPatentNumber.ReadOnly = false;
                txtPatentNumber.Text = "";
            }
            UpdateFormFieldsLockState(); // Обновляем состояние всех полей
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

            /*if (string.IsNullOrWhiteSpace(txtApplicationNumber.Text))
            {
                lblError.Text = "Пожалуйста, введите номер заявки.";
                return false;
            }*/

            // Проверка формата номера заявки (например, только цифры и символы "/")
            /*if (!Regex.IsMatch(txtApplicationNumber.Text, "^[0-9/]+$"))
            {
                lblError.Text = "Номер заявки должен содержать только цифры и символы '/'.";
                return false;
            }*/

            // Проверка соответствия статуса и номера патента
            var status = cbStatus.Text;

            // Проверка номера патента
            if (status == PatentApplication.STATUS_GRANTED && string.IsNullOrEmpty(txtPatentNumber.Text))
            {
                txtPatentNumber.Text = _repository.GetNextPatentNumber();
            }
            else if (status != PatentApplication.STATUS_GRANTED && !string.IsNullOrEmpty(txtPatentNumber.Text))
            {
                txtPatentNumber.Text = "";
            }

            // Проверка дат
            if (status == PatentApplication.STATUS_SUBMITTED &&
                (dtpDecisionDate.Checked || dtpExpirationDate.Checked))
            {
                lblError.Text = "Для статуса 'Подана' даты решения и истечения недопустимы";
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
            /*if (!string.IsNullOrWhiteSpace(txtPatentNumber.Text) && !Regex.IsMatch(txtPatentNumber.Text, "^[0-9]+$"))
            {
                lblError.Text = "Номер патента должен содержать только цифры.";
                return false;
            }*/

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
