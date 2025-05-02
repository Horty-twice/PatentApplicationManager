using System;

namespace PatentApplicationManager.Entities
{
    /// <summary>
    /// Класс, представляющий патентную заявку.
    /// Содержит все основные свойства, необходимые для учета патентных заявок,
    /// включая информацию о заявителе, датах подачи и решения, статус и примечания.
    /// </summary>
    public class PatentApplication
    {
        /// <summary>
        /// Уникальный идентификатор заявки в системе
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Регистрационный номер заявки
        /// </summary>
        public string ApplicationNumber { get; set; }

        /// <summary>
        /// Дата подачи заявки
        /// </summary>
        public DateTime FilingDate { get; set; }

        /// <summary>
        /// Название изобретения
        /// </summary>
        public string InventionTitle { get; set; }

        /// <summary>
        /// Полное имя заявителя
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>
        /// Имя патентного поверенного
        /// </summary>
        public string PatentAttorneyName { get; set; }

        /// <summary>
        /// Текущий статус заявки
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Дата принятия решения по заявке
        /// </summary>
        public DateTime? DecisionDate { get; set; }

        /// <summary>
        /// Номер выданного патента
        /// </summary>
        public string PatentNumber { get; set; }

        /// <summary>
        /// Дата истечения срока действия патента
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Дополнительные примечания к заявке
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PatentApplication"/>
        /// </summary>
        public PatentApplication()
        {
            FilingDate = DateTime.Now;
            Status = "Подана";
        }
    }
}