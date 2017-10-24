using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Puppy.Core.FileUtils;
using Puppy.Web.HtmlUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Puppy.ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            string docxSrcPath = "E:\\SampleConvert - Standard.docx";
            string docxPath = "E:\\SampleConvert.docx";
            string pdfPath = @"E:\SampleConvert.pdf";
            string pdfPassword = "";

            File.Copy(docxSrcPath, docxPath, true);
            Random random = new Random();
            WordHelper.Replace(docxPath, new Dictionary<string, string>
            {
                {"{SystemRunningNo}", random.Next(1000,9999).ToString()},
                {"{CurrentDate}", DateTimeOffset.UtcNow.ToString("dd MMMM yyyy")},
                {"{RequestYearMonth}", DateTimeOffset.UtcNow.ToString("yy/MM")},
                {"{CompanyName}", "Top Nguyen Co."},
                {"{CompanyAddress}", "8 Wilkie Road #03-01 Wilkie Edge Singapore 228095."},
                {"{XbrlType}", "Full XBRL"},
                {"{ContactName}", "Top Nguyen"},
                {"{ContactEmail}", "topnguyen92@gmail.com"},
                {"{DiscountedFee}", "560.00"},
                {"{StandardFee}", "20.00"},
                {"{SubTotalFee}", "580.00"},
                {"{TaxFee}", "40.60"},
                {"{TotalFee}", "620.60"},
                {"{FinancialYearEndDate}", new DateTime(2017,12,31).ToString("dd MMMM yyyy")},
            });

            List<PricingGuidelineItemModel> listItem = new List<PricingGuidelineItemModel>();

            listItem.Add(new PricingGuidelineItemModel
            {
                Name = "Service rendered in preparation of {XbrlType} for Financial Year Ended 31 December 2016",
                Amount = 560
            });

            listItem.Add(new PricingGuidelineItemModel
            {
                Name = "Disbursements (includes printing, photocopying and other incidental costs)",
                Amount = 20
            });

            listItem.Add(new PricingGuidelineItemModel
            {
                Name = "",
                Amount = 580
            });

            listItem.Add(new PricingGuidelineItemModel
            {
                Name = "Add GST @ 7%",
                Amount = (decimal)40.60
            });

            listItem.Add(new PricingGuidelineItemModel
            {
                Name = "Total",
                Amount = (decimal)620.60
            });

            InsertPricingGuidelineTable(docxPath, listItem.ToArray());

            //byte[] htmlBytes = HtmlHelper.FromDocx(docxPath);
            //HtmlHelper.ToPdfFromHtml(Encoding.UTF8.GetString(htmlBytes), pdfPath);
            //PdfHelper.SetPassword(pdfPath, pdfPassword);
        }

        public class PricingGuidelineItemModel
        {
            public string Name { get; set; }

            public decimal Amount { get; set; }
        }

        private static void InsertPricingGuidelineTable(string docxPath, params PricingGuidelineItemModel[] items)
        {
            // {PricingGuideline}

            Table table = new Table();
            TableProperties tableProperty = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 24 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 24 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 24 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 24 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 24 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 24 }
                )
            );
            table.AppendChild(tableProperty);

            var nameColWidth = "5000";
            var amountColWidth = "1500";

            // Header Row
            TableRow headerRow = new TableRow();

            // Name
            TableCell headerNameCell = new TableCell();
            headerNameCell.AppendChild(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = nameColWidth }));
            headerNameCell.AppendChild(new Paragraph(new Run(new Text())));
            headerRow.AppendChild(headerNameCell);

            // Amount
            TableCell headerAmountCell = new TableCell();
            headerRow.AppendChild(headerAmountCell);
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            Paragraph paragraph = new Paragraph(new Run(new Text("S$")));
            paragraphProperties.AppendChild(new Justification { Val = JustificationValues.Center });
            headerAmountCell.AppendChild(paragraphProperties);
            headerAmountCell.AppendChild(paragraph);

            table.AppendChild(headerRow);

            // Empty Row
            TableRow emptyRow = new TableRow();

            // Name
            TableCell emptyNameCell = new TableCell();
            emptyNameCell.AppendChild(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = nameColWidth }));
            emptyNameCell.AppendChild(new Paragraph(new Run(new Text())));
            emptyRow.AppendChild(emptyNameCell);

            // Amount
            TableCell emptyAmountCell = new TableCell();
            emptyAmountCell.AppendChild(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = amountColWidth }));
            emptyAmountCell.AppendChild(new Paragraph(new Run(new Text())));
            emptyRow.AppendChild(emptyAmountCell);

            table.AppendChild(emptyRow);

            foreach (var item in items)
            {
                TableRow row = new TableRow();

                // Name
                TableCell nameCell = new TableCell();
                nameCell.AppendChild(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = nameColWidth }));
                nameCell.AppendChild(new Paragraph(new Run(new Text(item.Name))));
                row.AppendChild(nameCell);

                // Amount
                TableCell amountCell = new TableCell();
                amountCell.AppendChild(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = amountColWidth }));
                amountCell.AppendChild(new Paragraph(new Run(new Text(item.Amount.ToString("F")))));
                row.AppendChild(amountCell);

                // Append to Table
                table.AppendChild(row);
            }

            using (WordprocessingDocument doc = WordprocessingDocument.Open(docxPath, true))
            {
                // Append the table to the document.
                doc.MainDocumentPart.Document.Body.AppendChild(table);
                doc.Save();
            }
        }
    }
}