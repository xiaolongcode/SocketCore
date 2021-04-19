using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Zeiot.Core
{
    /// <summary>
    /// 基于Docx实现的word操作
    /// </summary>
    public class WordHelper
    {
        /// <summary>
        /// 创建一个具有超链接、图像和表的文档。
        /// </summary>
        /// <param name="path">文档保存路径</param>
        /// <param name="p_w_picpathPath">加载的图片路径</param>
        public static void HyperlinksImagesTables(string path, string p_w_picpathPath)
        {
            // 创建一个文档
            using (var document = DocX.Create(path))
            {
                // 在文档中添加超链接。
                var link = document.AddHyperlink("link", new Uri("http://www.google.com"));
                // 在文档中添加一个表。
                var table = document.AddTable(2, 2);
                table.Design = TableDesign.ColorfulGridAccent2;
                table.Alignment = Alignment.center;
                table.Rows[0].Cells[0].Paragraphs[0].Append("1");
                table.Rows[0].Cells[1].Paragraphs[0].Append("2");
                table.Rows[1].Cells[0].Paragraphs[0].Append("3");
                table.Rows[1].Cells[1].Paragraphs[0].Append("4");
                var newRow = table.InsertRow(table.Rows[1]);
                newRow.ReplaceText("4", "5");
                // 将图像添加到文档中。    
                var p_w_picpath = document.AddImage(p_w_picpathPath);
                //创建一个图片（一个自定义视图的图像）。
                var picture = p_w_picpath.CreatePicture();
                picture.Rotation = 10;
                picture.SetPictureShape(BasicShapes.cube);
                // 在文档中插入一个新段落。
                var title = document.InsertParagraph().Append("Test").FontSize(20).Font("Comic Sans MS");
                title.Alignment = Alignment.center;
                // 在文档中插入一个新段落。
                var p1 = document.InsertParagraph();
                // 附加内容到段落
                p1.AppendLine("This line contains a ").Append("bold").Bold().Append(" word.");
                p1.AppendLine("Here is a cool ").AppendHyperlink(link).Append(".");
                p1.AppendLine();
                p1.AppendLine("Check out this picture ").AppendPicture(picture).Append(" its funky don't you think?");
                p1.AppendLine();
                p1.AppendLine("Can you check this Table of figures for me?");
                p1.AppendLine();
                // 在第1段后插入表格。
                p1.InsertTableAfterSelf(table);
                // 在文档中插入一个新段落。
                Paragraph p2 = document.InsertParagraph();
                // 附加内容到段落。
                p2.AppendLine("Is it correct?");
                // 保存当前文档
                document.Save();
            }
        }


        /// <summary>
        /// 设置文档的标题和页脚
        /// </summary>
        /// <param name="path">文档的路径</param>
        public static bool HeadersAndFooters(string path)
        {
            try
            {
                // 创建新文档
                using (var document = DocX.Create(path))
                {
                    // 这个文档添加页眉和页脚。
                    document.AddHeaders();
                    document.AddFooters();
                    // 强制第一个页面有一个不同的头和脚。
                    document.DifferentFirstPage = true;
                    // 奇偶页页眉页脚不同
                    document.DifferentOddAndEvenPages = true;
                    // 获取本文档的第一个、奇数和甚至是头文件。
                    Header headerFirst = document.Headers.First;
                    Header headerOdd = document.Headers.Odd;
                    Header headerEven = document.Headers.Even;
                    // 获取此文档的第一个、奇数和甚至脚注。
                    Footer footerFirst = document.Footers.First;
                    Footer footerOdd = document.Footers.Odd;
                    Footer footerEven = document.Footers.Even;
                    // 将一段插入到第一个头。
                    Paragraph p0 = headerFirst.InsertParagraph();
                    p0.Append("Hello First Header.").Bold();
                    // 在奇数头中插入一个段落。
                    Paragraph p1 = headerOdd.InsertParagraph();
                    p1.Append("Hello Odd Header.").Bold();
                    // 插入一个段落到偶数头中。
                    Paragraph p2 = headerEven.InsertParagraph();
                    p2.Append("Hello Even Header.").Bold();
                    // 将一段插入到第一个脚注中。
                    Paragraph p3 = footerFirst.InsertParagraph();
                    p3.Append("Hello First Footer.").Bold();
                    // 在奇数脚注中插入一个段落。
                    Paragraph p4 = footerOdd.InsertParagraph();
                    p4.Append("Hello Odd Footer.").Bold();
                    // 插入一个段落到偶数头中。
                    Paragraph p5 = footerEven.InsertParagraph();
                    p5.Append("Hello Even Footer.").Bold();
                    // 在文档中插入一个段落。
                    Paragraph p6 = document.InsertParagraph();
                    p6.AppendLine("Hello First page.");
                    // 创建一个第二个页面，显示第一个页面有自己的头和脚。
                    p6.InsertPageBreakAfterSelf();
                    // 在页面中断后插入一段。
                    Paragraph p7 = document.InsertParagraph();
                    p7.AppendLine("Hello Second page.");
                    // 创建三分之一页面显示，奇偶页不同的页眉和页脚。
                    p7.InsertPageBreakAfterSelf();
                    // 在页面中断后插入一段。
                    Paragraph p8 = document.InsertParagraph();
                    p8.AppendLine("Hello Third page.");
                    // 将属性保存入文档
                    document.Save();
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //从内存中释放此文档。
        }


        /// <summary>
        /// 自定义表格输出
        /// </summary>
        /// <param name="path"></param>
        public void CreatTable_Compensator(string path)
        {
            string realpath = @path + "\\补偿器.docx";
            try
            {
                if (File.Exists(realpath))
                {
                    //若存在则删除
                    File.Delete(realpath);
                }

                //File.Create(realpath).Close();

                //创建表格
                using (var document = DocX.Create(realpath))
                {


                    var title = document.InsertParagraph().Append("补偿器检定记录").Bold().FontSize(20);
                    title.Alignment = Alignment.center;

                    var table = document.AddTable(24, 6);
                    table.Design = TableDesign.TableGrid;
                    /*table.AutoFit = AutoFit.Window;*/



                    table.Alignment = Alignment.center;
                    List<Row> rows = table.Rows;


                    Row row0 = rows[0];
                    row0.Height = 50;
                    row0.MergeCells(0, 2);
                    row0.Cells[0].Paragraphs[0].Append("设备ID:" + "18635445").FontSize(16).Font("Arial").Bold().Alignment = Alignment.center;
                    row0.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row0.MergeCells(1, 3);
                    row0.Cells[1].Paragraphs[0].Append("仪器编号:" + "FH5919").FontSize(16).Font("Arial").Bold().Alignment = Alignment.center;
                    row0.Cells[1].VerticalAlignment = VerticalAlignment.Center;



                    //第二行
                    Row row1 = rows[1];
                    row1.MergeCells(0, 5);
                    row1.Cells[0].Paragraphs[0].Append("补偿范围检定记录").Bold().FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row1.Height = 60;
                    row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                    //第三行
                    Row row2 = rows[2];
                    row2.MergeCells(0, 1);
                    row2.Cells[0].Paragraphs[0].Append("次序").FontSize(11).Font("等线").Alignment = Alignment.center;
                    row2.MergeCells(1, 4);
                    row2.Cells[1].Paragraphs[0].Append("观测值显示").FontSize(11).Font("Arial").Alignment = Alignment.center;

                    //第四行
                    Row row3 = rows[3];
                    row3.MergeCells(0, 1);
                    row3.Cells[0].Paragraphs[0].Append("1").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row3.MergeCells(1, 4);
                    row3.Cells[1].Paragraphs[0].Append("3.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row3.Height = 35;
                    row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row3.Cells[1].VerticalAlignment = VerticalAlignment.Center;

                    //第五行
                    Row row4 = rows[4];
                    row4.MergeCells(0, 1);
                    row4.Cells[0].Paragraphs[0].Append("2").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row4.MergeCells(1, 4);
                    row4.Cells[1].Paragraphs[0].Append("4.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row4.Height = 35;
                    row4.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row4.Cells[1].VerticalAlignment = VerticalAlignment.Center;

                    //第六行
                    Row row5 = rows[5];
                    row5.MergeCells(0, 1);
                    row5.Cells[0].Paragraphs[0].Append("计算结果").FontSize(11).Font("Arial").Alignment = Alignment.center; ;
                    row5.MergeCells(1, 4);
                    row5.Cells[1].Paragraphs[0].Append("5.[1]").FontSize(11).Font("Arial");
                    row5.Height = 35;
                    row5.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row5.Cells[1].VerticalAlignment = VerticalAlignment.Center;

                    //第七行
                    Row row6 = rows[6];
                    row6.MergeCells(0, 5);
                    row6.Cells[0].Paragraphs[0].Append("补偿器零位误差检定记录").FontSize(11).Font("Arial").Bold().Alignment = Alignment.center;
                    row6.Height = 60;
                    row6.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                    //第八行
                    Row row7 = rows[7];
                    row7.MergeCells(0, 1);
                    row7.Cells[0].Paragraphs[0].Append("次序").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row7.Cells[1].Paragraphs[0].Append("方向").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row7.Cells[2].Paragraphs[0].Append("Ⅰ").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row7.Cells[3].Paragraphs[0].Append("Ⅱ").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row7.Cells[4].Paragraphs[0].Append("平均").FontSize(11).Font("Arial").Alignment = Alignment.center;


                    //第九行
                    Row row8 = rows[8];
                    row8.MergeCells(0, 1);
                    //.InsertParagraph()
                    row8.Cells[1].Paragraphs[0].Append("XL").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row8.Cells[2].Paragraphs[0].Append("8.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row8.Cells[3].Paragraphs[0].Append("8.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row8.Height = 30;
                    row8.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row8.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                    row8.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                    row8.Cells[0].Paragraphs[0].Append("1").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row8.Cells[4].Paragraphs[0].Append("8.4").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row8.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row8.Cells[4].VerticalAlignment = VerticalAlignment.Center;

                    //第十行
                    Row row9 = rows[9];
                    row9.MergeCells(0, 1);
                    table.MergeCellsInColumn(0, 8, 9);//列合并
                    table.MergeCellsInColumn(4, 8, 9);//列合并
                    row9.Cells[1].Paragraphs[0].Append("YL").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row9.Cells[2].Paragraphs[0].Append("9.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row9.Cells[3].Paragraphs[0].Append("9.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row9.Height = 30;
                    row9.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row9.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                    row9.Cells[3].VerticalAlignment = VerticalAlignment.Center;


                    //第十一行
                    Row row10 = rows[10];
                    row10.MergeCells(0, 1);
                    row10.Cells[1].Paragraphs[0].Append("XL").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row10.Cells[2].Paragraphs[0].Append("10.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row10.Cells[3].Paragraphs[0].Append("10.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row10.Height = 30;
                    row10.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row10.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                    row10.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                    row10.Cells[0].Paragraphs[0].Append("2").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row10.Cells[4].Paragraphs[0].Append("10.4").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row10.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row10.Cells[4].VerticalAlignment = VerticalAlignment.Center;

                    //第十二行
                    Row row11 = rows[11];
                    row11.MergeCells(0, 1);
                    table.MergeCellsInColumn(0, 10, 11);//第11和12列的第1行合并
                    table.MergeCellsInColumn(4, 10, 11);//第11和12列的第4行合并
                    row11.Cells[1].Paragraphs[0].Append("YL").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row11.Cells[2].Paragraphs[0].Append("11.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row11.Cells[3].Paragraphs[0].Append("11.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row11.Height = 30;
                    row11.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row11.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                    row11.Cells[3].VerticalAlignment = VerticalAlignment.Center;

                    //第十三行
                    Row row12 = rows[12];
                    row12.MergeCells(0, 1);
                    row12.Cells[0].Paragraphs[0].Append("计算结果").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row12.MergeCells(1, 4);
                    row12.Cells[1].Paragraphs[0].Append("12.[1]").FontSize(11).Font("Arial");
                    row12.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row12.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row12.Height = 35;


                    //第十四行
                    Row row13 = rows[13];
                    row13.MergeCells(0, 5);
                    row13.Cells[0].Paragraphs[0].Append("倾斜补偿器误差检定记录").FontSize(11).Font("Arial").Bold().Alignment = Alignment.center;
                    row13.Height = 60;
                    row13.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                    //第十五行
                    Row row14 = rows[14];
                    row14.MergeCells(0, 1);
                    row14.Cells[0].Paragraphs[0].Append("补偿").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row14.Cells[1].Paragraphs[0].Append("Ⅰ").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row14.Cells[2].Paragraphs[0].Append("Ⅱ").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row14.Cells[3].Paragraphs[0].Append("III").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row14.Cells[4].Paragraphs[0].Append("平均").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row14.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row14.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row14.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                    row14.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                    row14.Cells[4].VerticalAlignment = VerticalAlignment.Center;


                    //第十六行
                    Row row15 = rows[15];
                    row15.Cells[0].Width = 30f;
                    row15.Cells[0].Paragraphs[0].Append("竖").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[0].InsertParagraph().Append("直").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[0].InsertParagraph().Append("角").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                    row15.Cells[1].Paragraphs[0].Append("M1（水平）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[2].Paragraphs[0].Append("15.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[3].Paragraphs[0].Append("15.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[4].Paragraphs[0].Append("15.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row15.Cells[5].Paragraphs[0].Append("15.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    //第十七行
                    Row row16 = rows[16];
                    row16.Cells[1].Paragraphs[0].Append("M2（上倾）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row16.Cells[2].Paragraphs[0].Append("16.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row16.Cells[3].Paragraphs[0].Append("16.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row16.Cells[4].Paragraphs[0].Append("16.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row16.Cells[5].Paragraphs[0].Append("16.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    //第十八行

                    Row row17 = rows[17];
                    table.Rows.RemoveRange(3, 2);

                    row17.Cells[1].Paragraphs[0].Append("M3（下倾）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row17.Cells[2].Paragraphs[0].Append("17.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row17.Cells[3].Paragraphs[0].Append("17.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row17.Cells[4].Paragraphs[0].Append("17.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row17.Cells[5].Paragraphs[0].Append("17.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    //第十九行
                    Row row18 = rows[18];
                    table.MergeCellsInColumn(0, 15, 18);//列合并
                    row18.Cells[1].Paragraphs[0].Append("M4（再水平）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row18.Cells[2].Paragraphs[0].Append("18.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row18.Cells[3].Paragraphs[0].Append("18.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row18.Cells[4].Paragraphs[0].Append("18.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row18.Cells[5].Paragraphs[0].Append("18.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;


                    //第二十行
                    Row row19 = rows[19];
                    row19.Cells[0].Width = 30f;
                    row19.Cells[0].Paragraphs[0].Append("水").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[0].InsertParagraph().Append("平").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[0].InsertParagraph().Append("角").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row19.Cells[1].Paragraphs[0].Append("N1（水平）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[2].Paragraphs[0].Append("19.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[3].Paragraphs[0].Append("19.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[4].Paragraphs[0].Append("19.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row19.Cells[5].Paragraphs[0].Append("19.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    //第二十一行
                    Row row20 = rows[20];
                    row20.Cells[1].Paragraphs[0].Append("N3（下倾）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row20.Cells[2].Paragraphs[0].Append("20.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row20.Cells[3].Paragraphs[0].Append("20.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row20.Cells[4].Paragraphs[0].Append("20.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row20.Cells[5].Paragraphs[0].Append("20.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    //第二十二行
                    Row row21 = rows[21];
                    row21.Cells[1].Paragraphs[0].Append("N3（下倾）").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row21.Cells[2].Paragraphs[0].Append("21.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row21.Cells[3].Paragraphs[0].Append("21.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row21.Cells[4].Paragraphs[0].Append("21.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row21.Cells[5].Paragraphs[0].Append("21.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    //第二十三行
                    Row row22 = rows[22];
                    table.MergeCellsInColumn(0, 19, 22);//列合并
                    row22.Cells[1].Paragraphs[0].Append("N4（再水平)").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row22.Cells[2].Paragraphs[0].Append("22.[1]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row22.Cells[3].Paragraphs[0].Append("22.[2]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row22.Cells[4].Paragraphs[0].Append("22.[3]").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row22.Cells[5].Paragraphs[0].Append("22.[4]").FontSize(11).Font("Arial").Alignment = Alignment.center;


                    //第二十四行
                    Row row23 = rows[23];
                    row23.MergeCells(0, 1);
                    row23.Cells[0].Paragraphs[0].Append("计算结果").FontSize(11).Font("Arial").Alignment = Alignment.center;
                    row23.MergeCells(1, 4);
                    row23.Cells[1].Paragraphs[0].Append("row23.Cells[1]").FontSize(11).Font("Arial");
                    row23.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row23.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                    row23.Height = 35;


                    var p1 = document.InsertParagraph();
                    p1.InsertTableAfterSelf(table);
                    // 保存当前文档
                    document.Save();
                    //结果信息输出

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }

        #region 比较重要的函数和方法
        /*
        引用Docx的dll到解决方案的引用中，然后再using 其对应的命名空间。//(注意dll与命名空间不同而引发的错误)。 
        创建文件： var document = DocX.Create(realpath)       //realpath=文件创建的地址/名称.docx。
        表格大小：table.AutoFit = AutoFit.Contents;     //在不设定的时候，会默认大小，在wps中无法改动，设定了也无法改动！！office不用设定，自动A4大小
        插入标题： var title = document.InsertParagraph().Append("补偿器检定记录").Bold().FontSize(20);
        InsertParagraph插入一个段落，同理的insertRow 添加行与insertColumn添加列。
        Append在web开发的Jq中也有运用，即在被选元素的结尾(仍然在内部)插入指定内容。
        Bold()为字符加粗。
        FontSize(20)为字体大小
        Font(new FontFamily("Arial"))  　　 //设置字体样式为Arial（在office中无效，在wps中可用）。
        title.Alignment = Alignment.center;  　　//标题水平居中
        row0.Cells[1].VerticalAlignment = VerticalAlignment.Center;　　//单元格中的内容垂直居中
        创建表格： var table = document.AddTable(24, 6); 　　//创建一个24行6列的表格
        var p1 = document.InsertParagraph();
        p1.InsertTableAfterSelf(table);　　//插入创建的表格
        表格的边框样式：table.Design = TableDesign.TableGrid    //在输入TableGrid.后会有相应的提示，这里就不一一举例了。
        具体的单元格操纵：row[0].Cells[0].Paragraphs[0].....    //第一行的第一个单元格的第一个段落（从0开始，类似数组的计数）。
        单元格的横向合并：row[0].MergeCells(0, 1);　　 //第1行的第1个和第2个单元格合并，MergeCells（int ，int）起始位和终止位。
        单元格的纵向合并：table.MergeCellsInColumn(0, 10, 11);　　//第11和12列的第1行合并，道理同上。
        单元格的高度调整：row23.Height = 36;
        单元格的宽度调整：row19.Cells[0].Width = 30; //有一些问题！！
        保存文档：document.Save();
         */
        #endregion
    }
}
