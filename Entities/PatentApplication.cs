using System;

namespace PatentApplicationManager.Entities
{
    /// <summary>
    /// Класс, представляющий патентную заявку
    /// </summary>
    /// <remarks>
    /// Содержит все основные свойства, необходимые для учета патентных заявок,
    /// включая информацию о заявителе, датах подачи и решения, статус и примечания.
    /// </remarks>
    public class PatentApplication
    {
        /// <summary>
        /// Уникальный идентификатор заявки в системе
        /// </summary>
        /// <value>
        /// Целое число, генерируемое автоматически при сохранении в базу данных
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Регистрационный номер заявки
        /// </summary>
        /// <value>
        /// Строка в формате, установленном патентным ведомством
        /// </value>
        public string ApplicationNumber { get; set; }

        /// <summary>
        /// Дата подачи заявки
        /// </summary>
        /// <value>
        /// Устанавливается автоматически в текущую дату при создании объекта
        /// </value>
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
        /// <value>
        /// По умолчанию устанавливается "Подана" при создании объекта.
        /// Возможные значения: "Подана", "На рассмотрении", "Одобрена", "Отклонена"
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Дата принятия решения по заявке
        /// </summary>
        /// <value>
        /// Может быть null, если решение еще не принято.
        /// Должна быть позже даты подачи.
        /// </value>
        public DateTime? DecisionDate { get; set; }

        /// <summary>
        /// Номер выданного патента
        /// </summary>
        /// <value>
        /// Заполняется только при положительном решении.
        /// Формат зависит от патентного ведомства.
        /// </value>
        public string PatentNumber { get; set; }

        /// <summary>
        /// Дата истечения срока действия патента
        /// </summary>
        /// <value>
        /// Может быть null, если патент еще не выдан.
        /// Обычно устанавливается в 20 лет от даты подачи.
        /// </value>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Дополнительные примечания к заявке
        /// </summary>
        /// <value>
        /// Произвольный текст с дополнительной информацией
        /// </value>
        public string Notes { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PatentApplication"/>
        /// </summary>
        /// <remarks>
        /// Устанавливает значения по умолчанию:
        /// - <see cref="FilingDate"/> = текущая дата
        /// - <see cref="Status"/> = "Подана"
        /// </remarks>
        public PatentApplication()
        {
            FilingDate = DateTime.Now;
            Status = "Подана";
        }
    }
}