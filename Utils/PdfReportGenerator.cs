using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PatentApplicationManager.Entities;
using System.Collections.Generic;
using System.Drawing;

namespace PatentApplicationManager.Utils
{
    /// <summary>
    /// Генератор PDF-отчетов для патентных заявок
    /// </summary>
    /// <remarks>
    /// Предоставляет статический метод для создания PDF-документов с таблицей заявок.
    /// Использует библиотеку PDFsharp для работы с PDF.
    /// </remarks>
    public static class PdfReportGenerator
    {
        /// <summary>
        /// Генерирует PDF-отчет с перечнем патентных заявок
        /// </summary>
        /// <param name="applications">Список патентных заявок для отображения в отчете</param>
        /// <param name="filePath">Полный путь для сохранения PDF-файла</param>
        /// <exception cref="System.ArgumentNullException">Возникает если applications или filePath равны null</exception>
        /// <exception cref="System.IO.IOException">Возникает при проблемах записи файла</exception>
        /// <example>
        /// <code>
        /// var applications = repository.GetAllPatentApplications();
        /// PdfReportGenerator.GenerateReport(applications, "C:\\Reports\\patents.pdf");
        /// </code>
        /// </example>
        [System.Obsolete]
        public static void GenerateReport(List<PatentApplication> applications, string filePath)
        {
            // Создаем новый PDF-документ
            var document = new PdfDocument();

            // Добавляем первую страницу
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Настройка шрифтов
            var fontHeader = new XFont("Arial", new XUnit(10, XGraphicsUnit.Point));
            var fontRegular = new XFont("Arial", new XUnit(8, XGraphicsUnit.Point));

            // Начальные координаты для отрисовки
            var margin = new XUnit(20, XGraphicsUnit.Point);
            var x = margin;
            var y = new XUnit(30, XGraphicsUnit.Point);
            var rowHeight = new XUnit(15, XGraphicsUnit.Point);

            // Заголовок отчета
            gfx.DrawString("Отчёт по патентным заявкам", fontHeader, XBrushes.Black, x, y);
            y += new XUnit(25, XGraphicsUnit.Point);

            // Ширина столбцов таблицы
            var columnWidths = new XUnit[]
            {
                new XUnit(80, XGraphicsUnit.Point),   // Колонка "Номер"
                new XUnit(150, XGraphicsUnit.Point),  // Колонка "Название"
                new XUnit(100, XGraphicsUnit.Point),  // Колонка "Заявитель"
                new XUnit(100, XGraphicsUnit.Point),  // Колонка "Поверенный"
                new XUnit(80, XGraphicsUnit.Point)    // Колонка "Статус"
            };

            // Заголовки столбцов
            string[] headers = { "Номер", "Название", "Заявитель", "Поверенный", "Статус" };

            // Отрисовка заголовков таблицы
            var currentX = x;
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black, currentX, y);
                currentX += columnWidths[i];
            }
            y += rowHeight;

            // Разделительная линия между заголовком и данными
            gfx.DrawLine(XPens.Black, x, y, new XUnit(page.Width.Point - margin.Point), y);
            y += new XUnit(10, XGraphicsUnit.Point);

            // Заполнение таблицы данными
            foreach (var app in applications)
            {
                currentX = x;

                // Подготовка данных для текущей строки
                string[] cells =
                {
                    app.ApplicationNumber ?? "-",
                    app.InventionTitle ?? "-",
                    app.ApplicantName ?? "-",
                    app.PatentAttorneyName ?? "-",
                    app.Status ?? "-"
                };

                // Отрисовка данных в строке
                for (int i = 0; i < cells.Length; i++)
                {
                    gfx.DrawString(cells[i], fontRegular, XBrushes.Black, currentX, y);
                    currentX += columnWidths[i];
                }

                y += rowHeight;

                // Проверка на необходимость новой страницы
                if (y.Point > (page.Height.Point - margin.Point))
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = new XUnit(30, XGraphicsUnit.Point);
                }
            }

            // Сохранение PDF-документа
            document.Save(filePath);
        }
    }
}