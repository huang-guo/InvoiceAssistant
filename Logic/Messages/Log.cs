using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace InvoiceAssistant.Logic.Messages
{
    public static class Log
    {
        private const int MaxCount = 1000;
        private static int Count = 0;
        private static RichTextBox? textControl;
        private static InlineCollection inlines;

        //设置主控件
        public static void SetTextControl(RichTextBox _textBox)
        {
            textControl = _textBox;
            Paragraph graph = new Paragraph();
            inlines = graph.Inlines;
            textControl.Document.Blocks.Add(graph);
        }

        /// <summary>
        /// 输出黑色消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Info(string format, params object[] args)
        {
            AppendText(Brushes.Black, format, args);
        }

        /// <summary>
        /// 输出绿色消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Suc(string format, params object[] args)
        {
            AppendText(Brushes.DarkGreen, format, args);
        }

        /// <summary>
        /// 输出黄色消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Warning(string format, params object[] args)
        {
            AppendText(Brushes.DarkOrange, format, args);
        }

        /// <summary>
        /// 输出黄色消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Error(string format, params object[] args)
        {
            AppendText(Brushes.Red, format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            Count = 0;
            inlines.Clear();
            textControl?.ScrollToEnd();
        }

        private static void AppendText(Brush color, string format, params object[] args)
        {
            textControl?.BeginChange();
            StringBuilder builder = new();
            builder.Append("[");
            builder.Append(Count++);
            builder.Append("] : ");
            builder.Append(string.Format(format, args));
            builder.Append('\n');
            string str = builder.ToString();
            inlines.Add(new Run(str) { Foreground = color });
            if (inlines.Count > MaxCount)
            {
                inlines.Remove(inlines.FirstInline);
            }
            textControl?.ScrollToEnd();
            textControl?.EndChange();
        }
    }


}
